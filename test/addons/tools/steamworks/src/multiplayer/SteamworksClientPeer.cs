using System;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 相当于steamworks socket client的包装
/// </summary>
public partial class SteamworksClientPeer : MultiplayerPeerExtension
{
    private SteamworksClientPeer(PeerConnectionManager peerConnectionManager, ConnectionManager connectionManager)
    {
        PeerConnectionManager = peerConnectionManager;
        ConnectionManager = connectionManager;
    }

    private PeerConnectionManager PeerConnectionManager { get; }
    private ConnectionManager ConnectionManager { get; }

    private SteamMessage? LastPacket { set; get; }

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
            var peerConnectionManager = new PeerConnectionManager();
            var connectionManager = SteamNetworkingSockets.ConnectRelay(steamId, port, peerConnectionManager);
            var steamworksPeer = new SteamworksClientPeer(peerConnectionManager, connectionManager);
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
    }

    public override void _DisconnectPeer(int pPeer, bool pForce)
    {
        ConnectionManager.Close();
    }

    public override int _GetAvailablePacketCount()
    {
        return PeerConnectionManager.PacketQueue.Count;
    }

    public override ConnectionStatus _GetConnectionStatus()
    {
        return PeerConnectionManager.ConnectionStatus;
    }

    public override int _GetMaxPacketSize()
    {
        return 512 * 1024;
    }

    public override int _GetPacketChannel()
    {
        if (PeerConnectionManager.PacketQueue.TryPeek(out var packet))
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
        if (PeerConnectionManager.PacketQueue.TryPeek(out var packet))
        {
            return packet.PeerId;
        }

        return default;
    }

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

    public override int _GetUniqueId()
    {
        return (int)SteamClient.SteamId.AccountId;
    }

    public override bool _IsRefusingNewConnections()
    {
        return RefuseNewConnections;
    }

    public override bool _IsServer() => false;

    public override bool _IsServerRelaySupported()
    {
        return true;
    }

    public override void _Poll()
    {
        var receive = ConnectionManager.Receive();
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

    public override void _SetRefuseNewConnections(bool pEnable)
    {
        RefuseNewConnections = pEnable;
    }


    public override void _SetTargetPeer(int pPeer)
    {
        SetTargetPeer(pPeer);
    }

    public override void _SetTransferChannel(int pChannel)
    {
        TransferChannel = pChannel;
    }

    public override void _SetTransferMode(TransferModeEnum pMode)
    {
        TransferMode = pMode;
    }
}