using System;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 相当于steamworks socket client的包装
/// </summary>
public partial class SteamworksClientPeer : MultiplayerPeerExtension
{
    private PeerConnectionManager PeerConnectionManager { set; get; }
    private ConnectionManager ConnectionManager { set; get; }
    private SteamworksMessagePacket? LastPacket { set; get; }
    private int TargetPeer { set; get; }

    /// <summary>
    /// 创建客户端
    /// </summary>
    /// <param name="steamId"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public static SteamworksClientPeer CreateClient(SteamId steamId, int port)
    {
        try
        {
            var steamworksPeer = new SteamworksClientPeer();
            var peerConnectionManager = new PeerConnectionManager(steamworksPeer);
            var connectionManager = SteamNetworkingSockets.ConnectRelay(steamId, port, peerConnectionManager);
            steamworksPeer.PeerConnectionManager = peerConnectionManager;
            steamworksPeer.ConnectionManager = connectionManager;
            SteamManager.AddBeforeGameQuitAction(steamworksPeer.Close);
            return steamworksPeer;
        }
        catch (Exception e)
        {
            Log.Error($"创建{nameof(SteamworksClientPeer)} 异常, {e.Message}");
            throw;
        }
    }

    public override void _Close()
    {
        ConnectionManager.Close();
        PeerConnectionManager.PacketQueue.Clear();
    }

    public override void _DisconnectPeer(int pPeer, bool pForce)
    {
        ConnectionManager.Close();
    }

    public override int _GetAvailablePacketCount() => PeerConnectionManager.PacketQueue.Count;
    public override ConnectionStatus _GetConnectionStatus() => PeerConnectionManager.ConnectionStatus;
    public override int _GetMaxPacketSize() => 512 * 1024;
    public override int _GetPacketChannel() => LastPacket?.TransferChannel ?? 0;
    public override TransferModeEnum _GetPacketMode() => TransferModeEnum.Reliable;
    public override int _GetPacketPeer() => LastPacket?.PeerId ?? 0;

    public override byte[] _GetPacketScript()
    {
        if (PeerConnectionManager.PacketQueue.TryDequeue(out var packet))
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
    public override bool _IsServer() => false;
    public override bool _IsServerRelaySupported() => false;
    public override void _Poll() => ConnectionManager.Receive();

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
            var result = ConnectionManager.Connection.SendMessage(pBuffer, sendType, (ushort)TransferChannel);
            return result switch
            {
                Result.OK => Error.Ok,
                _ => Error.Failed
            };
        }
        catch (Exception e)
        {
            Log.Error($"{nameof(SteamworksClientPeer)} 发送数据包异常, {e.Message}");
            return Error.Failed;
        }
    }

    public override void _SetRefuseNewConnections(bool pEnable) => RefuseNewConnections = pEnable;
    public override void _SetTargetPeer(int pPeer) => TargetPeer = pPeer;
    public override void _SetTransferChannel(int pChannel) => TransferChannel = pChannel;
    public override void _SetTransferMode(TransferModeEnum pMode) => TransferMode = pMode;
}