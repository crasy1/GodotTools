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

    public override int _GetAvailablePacketCount()
    {
        Log.Debug("client ", nameof(_GetAvailablePacketCount));
        return PeerConnectionManager.PacketQueue.Count;
    }

    public override ConnectionStatus _GetConnectionStatus()
    {
        Log.Debug("client ", nameof(_GetConnectionStatus));
        return PeerConnectionManager.ConnectionStatus;
    }

    public override int _GetMaxPacketSize()
    {
        Log.Debug("client ", nameof(_GetMaxPacketSize));
        return 512 * 1024;
    }

    public override int _GetPacketChannel()

    {
        Log.Debug("client ", nameof(_GetPacketChannel));
        return LastPacket?.TransferChannel ?? 0;
    }

    public override TransferModeEnum _GetPacketMode()
    {
        Log.Debug("client ", nameof(_GetPacketMode));
        return TransferModeEnum.Reliable;
    }

    public override int _GetPacketPeer()
    {
        Log.Debug("client ", nameof(_GetPacketPeer));
        return LastPacket?.PeerId ?? 0;
    }


    public override int _GetUniqueId()
    {
        Log.Debug("client ", nameof(_GetUniqueId));
        return (int)SteamClient.SteamId.AccountId;
    }

    public override bool _IsServer()
    {
        Log.Debug("client ", nameof(_IsServer));
        return false;
    }

    // TODO 中继
    public override bool _IsServerRelaySupported()
    {
        Log.Debug("client ", nameof(_IsServerRelaySupported));
        return false;
    }

    public override void _SetTargetPeer(int pPeer)
    {
        Log.Debug("client ", nameof(_SetTargetPeer));
        TargetPeer = pPeer;
    }

    public override void _SetTransferChannel(int pChannel)
    {
        Log.Debug("client ", nameof(_SetTransferChannel));
    }

    public override void _SetTransferMode(TransferModeEnum pMode)
    {
        Log.Debug("client ", nameof(_SetTransferMode));
    }

    public override int _GetTransferChannel()
    {
        Log.Debug("client ", nameof(_GetTransferChannel));
        return base._GetTransferChannel();
    }

    public override TransferModeEnum _GetTransferMode()
    {
        Log.Debug("client ", nameof(_GetTransferMode));
        return base._GetTransferMode();
    }


    public override void _Poll() => ConnectionManager.Receive();

    public override byte[] _GetPacketScript()
    {
        Log.Debug("client ", nameof(_PutPacketScript));
        if (PeerConnectionManager.PacketQueue.TryDequeue(out var packet))
        {
            LastPacket = packet;
            return packet.Data;
        }

        return [];
    }

    public override Error _PutPacketScript(byte[] pBuffer)
    {
        try
        {
            Log.Debug("client ", nameof(_PutPacketScript));
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