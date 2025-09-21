using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 相当于steamworks networking message p2p的包装
/// https://partner.steamgames.com/doc/api/ISteamNetworkingMessages
/// </summary>
public partial class SteamworksMessageP2PPeer : MultiplayerPeerExtension
{
    private int TargetPeer { set; get; }
    private int PeerId { set; get; }
    private ConnectionStatus ConnectionStatus { set; get; } = ConnectionStatus.Connecting;
    private Lobby? Lobby { set; get; }

    private readonly Queue<SteamworksMessagePacket> PacketQueue = new();
    private readonly Dictionary<int, Friend> ConnectedPeers = new();

    private SteamworksMessageP2PPeer()
    {
        SteamMatchmaking.OnLobbyEntered += (lobby) =>
        {
            ConnectionStatus = ConnectionStatus.Connected;
            Lobby = lobby;
            foreach (var friend in lobby.Members)
            {
                if (friend.IsMe)
                {
                    continue;
                }

                var peerId = lobby.IsOwnedBy(friend.Id) ? 1 : (int)friend.Id.AccountId;
                if (ConnectedPeers.TryAdd(peerId, friend))
                {
                    EmitSignalPeerConnected(peerId);
                    Connect(friend.Id);
                }
            }
        };
        SteamMatchmaking.OnLobbyMemberJoined += (lobby, friend) =>
        {
            var peerId = lobby.IsOwnedBy(friend.Id) ? 1 : (int)friend.Id.AccountId;
            if (ConnectedPeers.TryAdd(peerId, friend))
            {
                EmitSignalPeerConnected(peerId);
                Connect(friend.Id);
            }
        };
        SteamMatchmaking.OnLobbyMemberLeave += (lobby, friend) =>
        {
            var peerId = lobby.IsOwnedBy(friend.Id) ? 1 : (int)friend.Id.AccountId;
            if (ConnectedPeers.Remove(peerId))
            {
                EmitSignalPeerDisconnected(peerId);
                DisconnectPeer(peerId);
            }
        };
        SNetworkingSocketMessages.Instance.ReceiveData += ReceiveData;
        Log.Info($"创建 {nameof(SteamworksMessageP2PPeer)}");
        SteamManager.AddBeforeGameQuitAction(Close);
    }

    public static async Task<SteamworksMessageP2PPeer?> CreateServer(int maxUser = 4)
    {
        var peer = new SteamworksMessageP2PPeer();
        peer.PeerId = 1;
        var lobby = await SMatchmaking.CreateLobbyAsync(maxUser);
        if (!lobby.HasValue)
        {
            Log.Error("创建大厅失败");
            return null;
        }

        return peer;
    }

    public static async Task<SteamworksMessageP2PPeer?> CreateClient(Lobby? lobby)
    {
        if (lobby == null)
        {
            Log.Error("未找到大厅");
            return null;
        }

        var peer = new SteamworksMessageP2PPeer();
        peer.PeerId = (int)SteamClient.SteamId.AccountId;
        var result = await SMatchmaking.JoinLobbyAsync(lobby.Value);
        if (!result)
        {
            peer.ConnectionStatus = ConnectionStatus.Disconnected;
            Log.Error("加入大厅失败");
            return null;
        }

        return peer;
    }

    private void ReceiveData(ulong steamId, int channel, byte[] data)
    {
        // 过滤掉握手包
        if (channel == (int)Channel.Handshake || !Lobby.HasValue)
        {
            return;
        }

        var steamMessage = new SteamworksMessagePacket
        {
            SteamId = steamId,
            Data = data,
            TransferChannel = channel,
            PeerId = Lobby.Value.IsOwnedBy(steamId) ? 1 : (int)((SteamId)steamId).AccountId
        };
        PacketQueue.Enqueue(steamMessage);
    }


    public void Connect(NetIdentity steamId)
    {
        SendMsg(steamId, Consts.SocketHandShake, Channel.Handshake);
    }

    public bool SendMsg(SteamId steamId, string data, Channel channel = Channel.Msg,
        SendType sendType = SendType.Reliable)
    {
        return SendMsg(steamId, Encoding.UTF8.GetBytes(data), channel, sendType);
    }

    public bool SendMsg(SteamId steamId, byte[] data, Channel channel = Channel.Msg,
        SendType sendType = SendType.Reliable)
    {
        return SNetworkingSocketMessages.SendP2P(steamId, data, channel, sendType);
    }


    public override void _Close()
    {
        Lobby?.Leave();
        foreach (var peerId in ConnectedPeers.Keys)
        {
            DisconnectPeer(peerId);
        }

        PacketQueue.Clear();
    }

    public override void _DisconnectPeer(int pPeer, bool pForce)
    {
        if (ConnectedPeers.Remove(pPeer, out var friend))
        {
            SNetworkingSocketMessages.Instance.Disconnect(friend.Id);
        }
    }

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
        return false;
    }

    public override void _SetTargetPeer(int pPeer)
    {
        TargetPeer = pPeer;
    }

    public override void _SetTransferChannel(int pChannel)
    {
    }

    public override void _SetTransferMode(TransferModeEnum pMode)
    {
    }

    public override int _GetTransferChannel()
    {
        return base._GetTransferChannel();
    }

    public override TransferModeEnum _GetTransferMode()
    {
        return base._GetTransferMode();
    }


    public override void _Poll()
    {
    }

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
            if (TargetPeer == 0)
            {
                foreach (var friend in ConnectedPeers.Values)
                {
                    SendMsg(friend.Id, pBuffer, channel, sendType);
                }
            }
            else if (TargetPeer < 0)
            {
                foreach (var (peerId, friend) in ConnectedPeers)
                {
                    if (peerId != Mathf.Abs(TargetPeer))
                    {
                        SendMsg(friend.Id, pBuffer, channel, sendType);
                    }
                }
            }
            else
            {
                foreach (var (peerId, friend) in ConnectedPeers)
                {
                    if (peerId == TargetPeer)
                    {
                        SendMsg(friend.Id, pBuffer, channel, sendType);
                    }
                }
            }

            return Error.Ok;
        }
        catch (Exception e)
        {
            Log.Error($"{nameof(SteamworksMessageP2PPeer)} 发送数据包异常, {e.Message}");
            return Error.Failed;
        }
    }
}