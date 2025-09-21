using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// steamworks peer 
/// </summary>
public abstract partial class SteamPeer : MultiplayerPeerExtension
{
    protected int TargetPeer { set; get; }
    protected int PeerId { set; get; }
    protected ConnectionStatus ConnectionStatus { set; get; } = ConnectionStatus.Connecting;
    protected Lobby? Lobby { set; get; }
    protected Friend? LobbyOwner { set; get; }

    protected readonly Queue<SteamworksMessagePacket> PacketQueue = new();

    /// <summary>
    /// 大厅中的peer
    /// </summary>
    protected readonly Dictionary<int, Friend> ConnectedPeers = new();

    /// <summary>
    /// 通信没有握手的玩家，握手成功后才去掉
    /// 如果在大厅中，但是未握手，场景同步时则会异常，所以确保都握手，server再创建玩家节点
    /// </summary>
    protected readonly Dictionary<int, SteamId> NotHandShakeSteamIds = new();

    /**
     * 服务器新的peer,和其他玩家未握手的玩家
     */
    protected readonly Dictionary<int, SteamId> ServerNewMemberPeers = new();

    protected const string NotHandShakeIds = "NOT_HANDSHAKE_IDS";

    protected SteamPeer()
    {
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyMemberDataChanged += OnLobbyMemberDataChanged;
        Log.Info($"创建 {nameof(SteamPeer)}");
    }

    private void OnLobbyMemberDataChanged(Lobby lobby, Friend friend)
    {
        if (_IsServer())
        {
            Dictionary<SteamId, List<int>> lobbyMemberData = new();
            foreach (var lobbyMember in lobby.Members)
            {
                var notHandShakeSteamIds = lobby.GetMemberData(lobbyMember, NotHandShakeIds);
                if (string.IsNullOrWhiteSpace(notHandShakeSteamIds))
                {
                    continue;
                }

                lobbyMemberData[lobbyMember.Id] = notHandShakeSteamIds.Split(",").Select(id => int.Parse(id)).ToList();
            }

            // 检测其他客户端有没有握手新的成员
            foreach (var newPeerId in ServerNewMemberPeers.Keys.ToList())
            {
                // 其他客户端都连接 newPeerIds 且 服务器也连接了
                var otherAllShakeHand = lobbyMemberData.Values.All(ids => !ids.Contains(newPeerId));
                if (otherAllShakeHand && NotHandShakeSteamIds.ContainsKey(newPeerId))
                {
                    ServerNewMemberPeers.Remove(newPeerId);
                    EmitSignalPeerConnected(newPeerId);
                    Log.Info($"[连接情况] 所有端都连接了{newPeerId} ，服务端同步]");
                }
            }
        }
    }

    protected void OnLobbyMemberLeave(Lobby lobby, Friend friend)
    {
        // 房主离开后会自动换新的房主
        var isLobbyOwner = friend.Id == LobbyOwner?.Id;
        var peerId = isLobbyOwner ? 1 : (int)friend.Id.AccountId;
        if (ConnectedPeers.Remove(peerId))
        {
            DisconnectPeer(peerId);
            EmitSignalPeerDisconnected(peerId);
        }

        if (_IsServer())
        {
            ServerNewMemberPeers.Remove(peerId);
        }
    }

    protected void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        var peerId = (int)friend.Id.AccountId;
        if (ConnectedPeers.TryAdd(peerId, friend))
        {
            HandShake(friend.Id);
        }

        NotHandShakeSteamIds[peerId] = friend.Id;
        if (_IsServer())
        {
            ServerNewMemberPeers[peerId] = friend.Id;
        }
    }

    protected void OnLobbyEntered(Lobby lobby)
    {
        ConnectionStatus = ConnectionStatus.Connected;
        Lobby = lobby;
        LobbyOwner = lobby.Owner;
        var members = lobby.Members.Where(f => !f.IsMe).ToList();
        foreach (var friend in members)
        {
            var peerId = lobby.IsOwnedBy(friend.Id) ? 1 : (int)friend.Id.AccountId;
            ConnectedPeers.TryAdd(peerId, friend);
            NotHandShakeSteamIds[peerId] = friend.Id;
            if (_IsServer())
            {
                ServerNewMemberPeers.Remove(peerId);
            }
        }

        lobby.SetMemberData(NotHandShakeIds, string.Join(",", NotHandShakeSteamIds.Keys));
        foreach (var friend in members)
        {
            HandShake(friend.Id);
        }
    }

    protected void ReceiveData(ulong steamId, int channel, byte[] data)
    {
        // 没有加入大厅，忽略信息
        if (!Lobby.HasValue)
        {
            return;
        }

        var peerId = Lobby.Value.IsOwnedBy(steamId) ? 1 : (int)((SteamId)steamId).AccountId;
        // 过滤掉握手包
        if (channel == (int)Channel.Handshake)
        {
            NotHandShakeSteamIds.Remove(peerId);
            // 如果不是服务器则认为已连接，服务器需要所有其他节点连接之后再连接
            if (!_IsServer())
            {
                EmitSignalPeerConnected(peerId);
            }
            Lobby.Value.SetMemberData(NotHandShakeIds, string.Join(",", NotHandShakeSteamIds.Keys));

            return;
        }

        var steamMessage = new SteamworksMessagePacket
        {
            SteamId = steamId,
            Data = data,
            TransferChannel = channel,
            PeerId = peerId
        };
        PacketQueue.Enqueue(steamMessage);
    }

    public async Task Connect(Lobby? lobby)
    {
        if (!lobby.HasValue)
        {
            Log.Error("未找到大厅");
            return;
        }

        if (_IsServer())
        {
            Log.Error("作为服务器不需要加入其他大厅");
            return;
        }

        var result = await SMatchmaking.JoinLobbyAsync(lobby.Value);
        if (!result)
        {
            ConnectionStatus = ConnectionStatus.Disconnected;
            Log.Error("加入大厅失败");
        }
    }

    public void HandShake(NetIdentity steamId)
    {
        SendMsg(steamId, Consts.SocketHandShake, Channel.Handshake);
    }

    public bool SendMsg(SteamId steamId, string data, Channel channel = Channel.Msg,
        SendType sendType = SendType.Reliable)
    {
        return SendMsg(steamId, Encoding.UTF8.GetBytes(data), channel, sendType);
    }

    public abstract bool SendMsg(SteamId steamId, byte[] data, Channel channel = Channel.Msg,
        SendType sendType = SendType.Reliable);


    public override void _Close()
    {
        Lobby?.Leave();
        foreach (var peerId in ConnectedPeers.Keys)
        {
            DisconnectPeer(peerId);
        }

        PacketQueue.Clear();
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeave;
    }

    public override void _DisconnectPeer(int pPeer, bool pForce)
    {
        if (ConnectedPeers.Remove(pPeer, out var friend))
        {
            DisconnectPeer(friend);
        }
    }

    public abstract void DisconnectPeer(Friend friend);

    public override int _GetAvailablePacketCount()
    {
        return PacketQueue.Count;
    }

    public override ConnectionStatus _GetConnectionStatus()
    {
        return ConnectionStatus;
    }

    public override int _GetMaxPacketSize()
    {
        return SNetworking.MaxPacketSize;
    }

    public override int _GetPacketChannel()
    {
        return PacketQueue.TryPeek(out var packet) ? packet.TransferChannel : 0;
    }

    public override TransferModeEnum _GetPacketMode()
    {
        return TransferModeEnum.Reliable;
    }

    public override int _GetPacketPeer()
    {
        return PacketQueue.TryPeek(out var packet) ? packet.PeerId : 1;
    }


    public override int _GetUniqueId()
    {
        return PeerId;
    }

    public override bool _IsServer()
    {
        return PeerId == 1;
    }

    public override bool _IsServerRelaySupported()
    {
        return ServerRelaySupported();
    }

    public abstract bool ServerRelaySupported();

    public override void _SetTargetPeer(int pPeer)
    {
        TargetPeer = pPeer;
    }

    public override void _SetTransferChannel(int pChannel)
    {
    }

    public override int _GetTransferChannel()
    {
        return base._GetTransferChannel();
    }

    public override void _SetTransferMode(TransferModeEnum pMode)
    {
    }

    public override TransferModeEnum _GetTransferMode()
    {
        return base._GetTransferMode();
    }

    public override void _Poll()
    {
        Receive();
    }

    public abstract void Receive();

    public override byte[] _GetPacketScript()
    {
        if (PacketQueue.TryDequeue(out var packet))
        {
            return packet.Data;
        }

        return [];
    }

    public override Error _PutPacketScript(byte[] pBuffer)
    {
        try
        {
            var sendType = TransferMode switch
            {
                TransferModeEnum.Unreliable => SendType.Unreliable,
                _ => SendType.Reliable
            };
            var channel = (Channel)TransferChannel;
            var sortedSet = ConnectedPeers.Keys.OrderBy(i=>i);
            if (TargetPeer == 0)
            {
                foreach (var peerId in sortedSet)
                {
                    
                    SendMsg(ConnectedPeers[peerId].Id, pBuffer, channel, sendType);
                }
                foreach (var friend in ConnectedPeers.Values)
                {
                }
            }
            else if (TargetPeer < 0)
            {
                foreach (var peerId in sortedSet)
                {
                    if (peerId != Mathf.Abs(TargetPeer))
                    {
                        SendMsg(ConnectedPeers[peerId].Id, pBuffer, channel, sendType);
                    }
                }
            }
            else
            {
                foreach (var peerId in sortedSet)
                {
                    if (peerId == TargetPeer)
                    {
                        SendMsg(ConnectedPeers[peerId].Id, pBuffer, channel, sendType);
                    }
                }
            }

            return Error.Ok;
        }
        catch (Exception e)
        {
            Log.Error($"{nameof(SteamPeer)} 发送数据包异常, {e.Message}");
            return Error.Failed;
        }
    }
}