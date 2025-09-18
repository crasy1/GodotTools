using System;
using System.Linq;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 相当于steamworks socket normal server的包装
/// </summary>
public partial class NormalServerPeer : MultiplayerPeerExtension
{
    private PeerSocketManager PeerSocketManager { set; get; }
    private SocketManager SocketManager { set; get; }
    private int TargetPeer { set; get; }

    /// <summary>
    /// 创建服务端
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public static NormalServerPeer CreateServer(int port)
    {
        try
        {
            var normalPeer = new NormalServerPeer();
            var peerSocketManager = new PeerSocketManager(normalPeer);
            var socketManager =
                SteamNetworkingSockets.CreateNormalSocket(NetAddress.AnyIp((ushort)port), peerSocketManager);
            normalPeer.PeerSocketManager = peerSocketManager;
            normalPeer.SocketManager = socketManager;
            SteamManager.AddBeforeGameQuitAction(normalPeer.Close);
            Log.Info($"创建 {nameof(NormalServerPeer)}");
            return normalPeer;
        }
        catch (Exception e)
        {
            Log.Error($"创建{nameof(NormalServerPeer)} 异常, {e.Message}");
            throw;
        }
    }

    public override void _Close()
    {
        foreach (var connection in PeerSocketManager.Connections.Keys)
        {
            connection.Close();
        }

        PeerSocketManager.PacketQueue.Clear();
        SocketManager.Close();
    }

    public override void _DisconnectPeer(int pPeer, bool pForce)
    {
        var connection = PeerSocketManager.Connections.FirstOrDefault(kv => (uint)pPeer == kv.Value.AccountId).Key;
        connection.Close();
    }

    public override int _GetAvailablePacketCount()
    {
        return PeerSocketManager.PacketQueue.Count;
    }

    public override ConnectionStatus _GetConnectionStatus()
    {
        return PeerSocketManager.ConnectionStatus;
    }

    public override int _GetMaxPacketSize()
    {
        return SNetworking.MaxPacketSize;
    }

    public override int _GetPacketChannel()
    {
        return PeerSocketManager.PacketQueue.TryPeek(out var packet) ? packet.TransferChannel : 0;
    }

    public override TransferModeEnum _GetPacketMode()
    {
        return TransferModeEnum.Reliable;
    }

    public override int _GetPacketPeer()
    {
        return PeerSocketManager.PacketQueue.TryPeek(out var packet) ? packet.PeerId : 0;
    }


    public override int _GetUniqueId()
    {
        return 1;
    }

    public override bool _IsServer()
    {
        return true;
    }

    // TODO 中继
    public override bool _IsServerRelaySupported()
    {
        return false;
    }

    public override void _SetTargetPeer(int pPeer)
    {
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


    public override void _Poll() => SocketManager.Receive();

    public override byte[] _GetPacketScript()
    {
        if (PeerSocketManager.PacketQueue.TryDequeue(out var packet))
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
                TransferModeEnum.Reliable => SendType.Reliable,
                TransferModeEnum.Unreliable => SendType.Unreliable,
                TransferModeEnum.UnreliableOrdered => SendType.NoNagle,
                _ => SendType.Reliable
            };
            if (TargetPeer == 0)
            {
                foreach (var connection in PeerSocketManager.Connections.Keys)
                {
                    connection.SendMessage(pBuffer, sendType, (ushort)TransferChannel);
                }
            }
            else if (TargetPeer < 0)
            {
                foreach (var (connection, steamId) in PeerSocketManager.Connections)
                {
                    if (steamId.AccountId != Mathf.Abs(TargetPeer))
                    {
                        connection.SendMessage(pBuffer, sendType, (ushort)TransferChannel);
                    }
                }
            }
            else
            {
                foreach (var (connection, steamId) in PeerSocketManager.Connections)
                {
                    if (steamId.AccountId == TargetPeer)
                    {
                        connection.SendMessage(pBuffer, sendType, (ushort)TransferChannel);
                    }
                }
            }

            return Error.Ok;
        }
        catch (Exception e)
        {
            Log.Error($"{nameof(NormalServerPeer)} 发送数据包异常, {e.Message}");
            return Error.Failed;
        }
    }
}