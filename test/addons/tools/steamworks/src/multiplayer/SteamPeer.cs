using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// steamworks 的godot MultiplayerPeer 基类
/// </summary>
public abstract partial class SteamPeer : MultiplayerPeerExtension
{
    public const int ServerPeerId = 1;
    private int TargetPeer { set; get; }
    protected int PeerId { set; get; }
    public ConnectionStatus ConnectionStatus { set; get; } = ConnectionStatus.Connecting;
    private Lobby? Lobby { set; get; }
    private Friend? LobbyOwner { set; get; }

    private readonly Queue<SteamPacket> PacketQueue = new();

    /// <summary>
    /// 大厅中的peer
    /// </summary>
    private readonly Dictionary<int, SteamId> ConnectedPeers = new();

    protected SteamPeer()
    {
        Log.Info($"创建 {GetType().Name}");
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
        OnCreate();
    }

    /// <summary>
    /// 初始化时操作，子类的信号连接等
    /// </summary>
    protected abstract void OnCreate();

    /// <summary>
    /// 关闭时操作，子类的信号断开等
    /// </summary>
    protected abstract void OnClose();

    private void OnLobbyEntered(Lobby lobby)
    {
        ConnectionStatus = ConnectionStatus.Connected;
        Lobby = lobby;
        LobbyOwner = lobby.Owner;
        if (!_IsServer())
        {
            // 给服务器发送握手包
            HandShake(lobby.Owner.Id);
        }
    }

    protected void OnHandShakeFailed(ulong steamId)
    {
        HandShake(steamId);
    }

    private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        // 服务器给新加入的peer发送握手包
        if (_IsServer())
        {
            HandShake(friend.Id);
        }
    }

    private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
    {
        // 房主离开后会自动换新的房主
        var isLobbyOwnerLeave = friend.Id == LobbyOwner?.Id;
        var peerId = isLobbyOwnerLeave ? ServerPeerId : (int)friend.Id.AccountId;
        if (_IsServer())
        {
            // 服务器移除离开的成员
            ConnectedPeers.Remove(peerId);
            DisconnectPeer(peerId);
            EmitSignalPeerDisconnected(peerId);
        }
        else
        {
            if (isLobbyOwnerLeave)
            {
                // 客户端移除服务器
                ConnectedPeers.Remove(peerId);
                DisconnectPeer(peerId);
                EmitSignalPeerDisconnected(peerId);
            }
        }
    }

    public void ReceiveData(ulong steamId, int channel, byte[] data)
    {
        // 没有加入大厅，忽略信息
        if (!Lobby.HasValue)
        {
            return;
        }

        var peerId = Lobby.Value.IsOwnedBy(steamId) ? ServerPeerId : (int)((SteamId)steamId).AccountId;
        // 过滤掉握手包
        if (channel == (int)Channel.Handshake)
        {
            if (ConnectedPeers.TryAdd(peerId, steamId))
            {
                EmitSignalPeerConnected(peerId);
            }

            return;
        }

        var steamMessage = new SteamPacket
        {
            SteamId = steamId,
            Data = data,
            TransferChannel = channel,
            PeerId = peerId
        };
        PacketQueue.Enqueue(steamMessage);
    }

    protected async Task<Lobby> CreateLobby(int maxUser)
    {
        var lobby = await SMatchmaking.CreateLobbyAsync(maxUser);
        if (!lobby.HasValue)
        {
            throw new Exception("创建大厅失败");
        }

        return lobby.Value;
    }

    protected async Task JoinLobby(Lobby? lobby)
    {
        if (!lobby.HasValue)
        {
            throw new Exception("未找到大厅");
        }

        if (_IsServer())
        {
            throw new Exception("作为服务器不需要加入其他大厅");
        }

        var result = await SMatchmaking.JoinLobbyAsync(lobby.Value);
        if (!result)
        {
            ConnectionStatus = ConnectionStatus.Disconnected;
            throw new Exception("加入大厅失败");
        }
    }

    public void HandShake(SteamId steamId)
    {
        SendMsg(steamId, Consts.SocketHandShake, Channel.Handshake);
    }

    private bool SendMsg(SteamId steamId, string data, Channel channel = Channel.Msg,
        SendType sendType = SendType.Reliable)
    {
        return SendMsg(steamId, Encoding.UTF8.GetBytes(data), channel, sendType);
    }

    protected abstract bool SendMsg(SteamId steamId, byte[] data, Channel channel = Channel.Msg,
        SendType sendType = SendType.Reliable);


    public override void _Close()
    {
        Lobby?.Leave();
        foreach (var peerId in ConnectedPeers.Keys)
        {
            DisconnectPeer(peerId);
        }

        Lobby = null;
        LobbyOwner = null;
        ConnectionStatus = ConnectionStatus.Disconnected;
        PacketQueue.Clear();
        ConnectedPeers.Clear();
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeave;
        OnClose();
    }

    public override void _DisconnectPeer(int pPeer, bool pForce)
    {
        if (ConnectedPeers.Remove(pPeer, out var steamId))
        {
            DisconnectPeer(steamId);
        }
    }

    protected abstract void DisconnectPeer(SteamId steamId);

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
        return PacketQueue.TryPeek(out var packet) ? packet.PeerId : ServerPeerId;
    }


    public override int _GetUniqueId()
    {
        return PeerId;
    }

    public override bool _IsServer()
    {
        return PeerId == ServerPeerId;
    }


    public override bool _IsServerRelaySupported()
    {
        return ServerRelaySupported();
    }

    /// <summary>
    /// 如果支持中继，那么客户端的流量会先发给服务器，然后由服务器转发
    /// </summary>
    /// <returns></returns>
    protected abstract bool ServerRelaySupported();

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

    protected abstract void Receive();

    public override byte[] _GetPacketScript()
    {
        return PacketQueue.TryDequeue(out var packet) ? packet.Data : [];
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
            if (_IsServer())
            {
                switch (TargetPeer)
                {
                    case 0:
                    {
                        foreach (var peerId in ConnectedPeers.Keys)
                        {
                            SendMsg(ConnectedPeers[peerId], pBuffer, channel, sendType);
                        }

                        break;
                    }
                    case < 0:
                    {
                        foreach (var peerId in ConnectedPeers.Keys.Where(peerId => peerId != Mathf.Abs(TargetPeer)))
                        {
                            SendMsg(ConnectedPeers[peerId], pBuffer, channel, sendType);
                        }

                        break;
                    }
                    default:
                    {
                        foreach (var peerId in ConnectedPeers.Keys.Where(peerId => peerId == TargetPeer))
                        {
                            SendMsg(ConnectedPeers[peerId], pBuffer, channel, sendType);
                        }

                        break;
                    }
                }
            }
            else
            {
                SendMsg(ConnectedPeers[ServerPeerId], pBuffer, channel, sendType);
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