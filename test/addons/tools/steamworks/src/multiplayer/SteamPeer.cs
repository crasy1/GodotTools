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
///
/// 目前测试 MultiplayerPeer
/// 速度      enet>p2p>normal>relay
/// 稳定性     enet>normal>p2p>relay
/// </summary>
public abstract partial class SteamPeer : MultiplayerPeerExtension
{
    public const int ServerPeerId = 1;
    public const int MaxPacketSize = 512 * 1024;
    protected int TargetPeer { set; get; }
    protected int PeerId { get; }
    protected SteamSocketType SteamSocketType { get; }
    public ConnectionStatus ConnectionStatus { set; get; } = ConnectionStatus.Connecting;

    protected readonly Queue<SteamPacket> PacketQueue = new();

    /// <summary>
    /// 大厅中的peer
    /// </summary>
    protected readonly Dictionary<int, SteamId> ConnectedPeers = new();

    protected string Type { set; get; }

    protected SteamPeer(int peerId, SteamSocketType socketType)
    {
        PeerId = peerId;
        SteamSocketType = socketType;
        Type = GetType().Name;
        Log.Info($"创建 {Type}");
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

    public int SteamIdToPeerId(ulong steamId)
    {
        return SteamIdToPeerId((SteamId)steamId);
    }

    public int SteamIdToPeerId(SteamId steamId)
    {
        return _IsServer() ? (int)steamId.AccountId : ServerPeerId;
    }

    public void OnSocketConnected(ulong steamId)
    {
        var peerId = SteamIdToPeerId(steamId);
        if (ConnectedPeers.TryAdd(peerId, steamId))
        {
            ConnectionStatus = ConnectionStatus.Connected;
            EmitSignalPeerConnected(peerId);
        }
    }

    public void OnSocketDisconnected(ulong steamId)
    {
        var peerId = SteamIdToPeerId(steamId);
        DisconnectPeer(peerId);
    }

    /// <summary>
    /// 实现接收数据，并传到ProcessData
    /// </summary>
    /// <param name="steamId"></param>
    /// <param name="channel"></param>
    /// <param name="data"></param>
    public abstract void ReceiveData(ulong steamId, int channel, byte[] data);

    /// <summary>
    /// 在receiveData中处理数据
    /// </summary>
    /// <param name="peerId"></param>
    /// <param name="steamId"></param>
    /// <param name="channel"></param>
    /// <param name="data"></param>
    protected void ProcessData(int peerId, SteamId steamId, int channel, byte[] data)
    {
        var steamMessage = new SteamPacket
        {
            SteamId = steamId,
            Data = data,
            TransferChannel = channel,
            PeerId = peerId
        };
        PacketQueue.Enqueue(steamMessage);
    }

    protected bool SendMsg(SteamId steamId, string data, Channel channel = Channel.Msg,
        SendType sendType = SendType.Reliable)
    {
        return SendMsg(steamId, Encoding.UTF8.GetBytes(data), channel, sendType);
    }

    protected abstract bool SendMsg(SteamId steamId, byte[] data, Channel channel = Channel.Msg,
        SendType sendType = SendType.Reliable);


    public override void _Close()
    {
        foreach (var peerId in ConnectedPeers.Keys)
        {
            DisconnectPeer(peerId);
        }

        OnClose();
        ConnectionStatus = ConnectionStatus.Disconnected;
        PacketQueue.Clear();
        ConnectedPeers.Clear();
    }

    public override async void _DisconnectPeer(int pPeer, bool pForce)
    {
        try
        {
            if (!_IsServer())
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
            }

            if (ConnectedPeers.Remove(pPeer, out var steamId))
            {
                await OnPeerDisconnect(steamId);
                EmitSignalPeerDisconnected(pPeer);
            }
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    protected abstract Task OnPeerDisconnect(SteamId steamId);

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
        return MaxPacketSize;
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
                        foreach (var steamId in ConnectedPeers.Values)
                        {
                            SendMsg(steamId, pBuffer, channel, sendType);
                        }

                        break;
                    }
                    case < 0:
                    {
                        foreach (var (peerId, steamId) in ConnectedPeers.Where(kv => kv.Key != Mathf.Abs(TargetPeer)))
                        {
                            SendMsg(steamId, pBuffer, channel, sendType);
                        }

                        break;
                    }
                    default:
                    {
                        foreach (var (peerId, steamId) in ConnectedPeers.Where(kv => kv.Key == TargetPeer))
                        {
                            SendMsg(steamId, pBuffer, channel, sendType);
                        }

                        break;
                    }
                }
            }
            else
            {
                if (ConnectedPeers.TryGetValue(ServerPeerId, out var steamId))
                {
                    SendMsg(steamId, pBuffer, channel, sendType);
                }
            }

            return Error.Ok;
        }
        catch (Exception e)
        {
            Log.Error($"{Type} {PeerId} 发送数据包异常, {e.Message}");
            return Error.Failed;
        }
    }
}