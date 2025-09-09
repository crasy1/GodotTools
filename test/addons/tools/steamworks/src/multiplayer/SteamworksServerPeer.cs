using System;
using System.Linq;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 相当于steamworks socket server的包装
/// </summary>
public partial class SteamworksServerPeer : MultiplayerPeerExtension
{
    private SteamworksServerPeer(PeerSocketManager peerSocketManager, SocketManager socketManager)
    {
        PeerSocketManager = peerSocketManager;
        PeerSocketManager.SteamworksServerPeer = this;
        SocketManager = socketManager;
    }

    private PeerSocketManager PeerSocketManager { get; }
    private SocketManager SocketManager { get; }

    private int TargetPeer { set; get; }
    private SteamMessage? LastPacket { set; get; }

    /// <summary>
    /// 创建服务端
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public static SteamworksServerPeer CreateServer(int port)
    {
        try
        {
            var peerSocketManager = new PeerSocketManager();
            var socketManager = SteamNetworkingSockets.CreateRelaySocket(port, peerSocketManager);
            var steamworksPeer = new SteamworksServerPeer(peerSocketManager, socketManager);
            return steamworksPeer;
        }
        catch (Exception e)
        {
            Log.Error($"创建{nameof(SteamworksServerPeer)} 异常, {e.Message}");
            throw;
        }
    }

    public override void _Close()
    {
        SocketManager.Close();
    }

    public override void _DisconnectPeer(int pPeer, bool pForce)
    {
        var connection = PeerSocketManager.Connections.FirstOrDefault(kv => (uint)pPeer == kv.Value.AccountId).Key;
        connection.Close();
    }

    public override int _GetAvailablePacketCount() => PeerSocketManager.PacketQueue.Count;

    public override ConnectionStatus _GetConnectionStatus() => PeerSocketManager.ConnectionStatus;

    public override int _GetMaxPacketSize() => 52 * 1024;

    public override int _GetPacketChannel()
    {
        if (PeerSocketManager.PacketQueue.TryPeek(out var packet))
        {
            return packet.TransferChannel;
        }

        return default;
    }

    public override TransferModeEnum _GetPacketMode()
    {
        return default;
    }

    public override int _GetPacketPeer()
    {
        if (PeerSocketManager.PacketQueue.TryPeek(out var packet))
        {
            return packet.PeerId;
        }

        return default;
    }

    public override byte[] _GetPacketScript()
    {
        if (PeerSocketManager.PacketQueue.TryDequeue(out var packet))
        {
            LastPacket = packet;
            return packet.Data;
        }

        return [];
    }

    public override int _GetTransferChannel() => TransferChannel;

    public override TransferModeEnum _GetTransferMode() => TransferMode;

    public override int _GetUniqueId()
    {
        return 1;
    }

    public override bool _IsRefusingNewConnections() => RefuseNewConnections;

    public override bool _IsServer() => true;

    public override bool _IsServerRelaySupported()
    {
        return true;
    }

    public override void _Poll()
    {
        SocketManager.Receive();
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
            var connections = PeerSocketManager.Connections;
            if (TargetPeer == 0)
            {
                foreach (var connection in connections.Keys)
                {
                    connection.SendMessage(pBuffer, sendType, (ushort)TransferChannel);
                }
            }
            else if (TargetPeer > 1)
            {
                foreach (var (connection, steamId) in connections)
                {
                    if (steamId.AccountId == TargetPeer)
                    {
                        connection.SendMessage(pBuffer, sendType, (ushort)TransferChannel);
                    }
                }
            }
            else if (TargetPeer < 0)
            {
                foreach (var (connection, steamId) in connections)
                {
                    if (steamId.AccountId != -TargetPeer)
                    {
                        connection.SendMessage(pBuffer, sendType, (ushort)TransferChannel);
                    }
                }
            }

            return Error.Ok;
        }
        catch (Exception e)
        {
            Log.Error($"{nameof(SteamworksServerPeer)} 发送数据包异常, {e.Message}");
            return Error.Failed;
        }
    }

    public override void _SetRefuseNewConnections(bool pEnable) => RefuseNewConnections = pEnable;

    public override void _SetTargetPeer(int pPeer)
    {
        TargetPeer = pPeer;
        SetTargetPeer(pPeer);
    }

    public override void _SetTransferChannel(int pChannel) => TransferChannel = pChannel;

    public override void _SetTransferMode(TransferModeEnum pMode) => TransferMode = pMode;
}