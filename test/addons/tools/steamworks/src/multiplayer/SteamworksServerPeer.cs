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
    private PeerSocketManager PeerSocketManager { set; get; }
    private SocketManager SocketManager { set; get; }
    private SteamworksMessagePacket? LastPacket { set; get; }
    private int TargetPeer { set; get; }

    /// <summary>
    /// 创建服务端
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public static SteamworksServerPeer CreateServer(int port)
    {
        try
        {
            var steamworksPeer = new SteamworksServerPeer();
            var peerSocketManager = new PeerSocketManager(steamworksPeer);
            var socketManager = SteamNetworkingSockets.CreateRelaySocket(port, peerSocketManager);
            steamworksPeer.PeerSocketManager = peerSocketManager;
            steamworksPeer.SocketManager = socketManager;
            SteamManager.AddBeforeGameQuitAction(steamworksPeer.Close);
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

    public override int _GetAvailablePacketCount() => PeerSocketManager.PacketQueue.Count;
    public override ConnectionStatus _GetConnectionStatus() => PeerSocketManager.ConnectionStatus;
    public override int _GetMaxPacketSize() => 512 * 1024;
    public override int _GetPacketChannel() => LastPacket?.TransferChannel ?? 0;
    public override TransferModeEnum _GetPacketMode() => TransferModeEnum.Reliable;
    public override int _GetPacketPeer() => LastPacket?.PeerId ?? 0;

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
    public override int _GetUniqueId() => (int)SteamClient.SteamId.AccountId;
    public override bool _IsRefusingNewConnections() => RefuseNewConnections;
    public override bool _IsServer() => true;

    // TODO 中继
    public override bool _IsServerRelaySupported() => false;
    public override void _Poll() => SocketManager.Receive();

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
            Log.Error($"{nameof(SteamworksServerPeer)} 发送数据包异常, {e.Message}");
            return Error.Failed;
        }
    }

    public override void _SetRefuseNewConnections(bool pEnable) => RefuseNewConnections = pEnable;
    public override void _SetTargetPeer(int pPeer) => TargetPeer = pPeer;
    public override void _SetTransferChannel(int pChannel) => TransferChannel = pChannel;
    public override void _SetTransferMode(TransferModeEnum pMode) => TransferMode = pMode;
}