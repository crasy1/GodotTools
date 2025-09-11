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
            Log.Info($"创建 {nameof(SteamworksClientPeer)}");
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
        return SNetworking.MaxPacketSize;
    }

    public override int _GetPacketChannel()
    {
        return PeerConnectionManager.PacketQueue.TryPeek(out var packet) ? packet.TransferChannel : 0;
    }

    public override TransferModeEnum _GetPacketMode()
    {
        return TransferModeEnum.Reliable;
    }

    public override int _GetPacketPeer()
    {
        return PeerConnectionManager.PacketQueue.TryPeek(out var packet) ? packet.PeerId : 0;
    }


    public override int _GetUniqueId()
    {
        return (int)SteamClient.SteamId.AccountId;
    }

    public override bool _IsServer()
    {
        return false;
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


    public override void _Poll() => ConnectionManager.Receive();

    public override byte[] _GetPacketScript()
    {
        if (PeerConnectionManager.PacketQueue.TryDequeue(out var packet))
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
}