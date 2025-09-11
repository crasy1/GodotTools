using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 相当于steamworks p2p的包装
/// </summary>
public partial class SteamworksP2PPeer : MultiplayerPeerExtension
{
    private int TargetPeer { set; get; }
    private const string HandShakeSend = "[HANDSHAKE_SEND]";
    private const string HandShakeReply = "[HANDSHAKE_REPLY]";
    private const string Disconnect = "[DISCONNECT]";

    private readonly Queue<SteamworksMessagePacket> PacketQueue = new();
    private readonly Dictionary<int, SteamId> Connected = new();

    public SteamworksP2PPeer()
    {
        // SNetworking.Instance.UserConnected += OnUserConnected;
        SNetworking.Instance.UserConnectFailed += OnUserConnectFailed;
        // SNetworking.Instance.UserDisconnected += OnUserDisconnected;
        SNetworking.Instance.ReceiveData += OnReceiveData;
        Log.Info($"创建 {nameof(SteamworksP2PPeer)}");
        SteamManager.AddBeforeGameQuitAction(Close);
    }

    private void OnReceiveData(ulong steamId, int channel, byte[] data)
    {
        if (channel == (int)Channel.Handshake)
        {
            var msg = Encoding.UTF8.GetString(data);
            if (HandShakeSend == msg)
            {
                ConnectReply(steamId);
                if (Connected.Values.All(i => steamId != i))
                {
                    OnUserConnected(steamId);
                }
            }
            else if (HandShakeReply == msg)
            {
                if (Connected.Values.All(i => steamId != i))
                {
                    OnUserConnected(steamId);
                }
            }
            else if (Disconnect == msg)
            {
                if (Connected.Values.All(i => steamId != i))
                {
                    OnUserDisconnected(steamId);
                }
            }

            Log.Debug($"{nameof(SteamworksP2PPeer)} handshake {msg}");
            return;
        }

        var p2Packet = new SteamworksMessagePacket()
        {
            SteamId = steamId,
            Data = data,
            TransferChannel = channel
        };
        PacketQueue.Enqueue(p2Packet);
    }

    private void OnUserConnected(ulong steamId)
    {
        var peerId = (int)((SteamId)steamId).AccountId;
        Connected.TryAdd(peerId, steamId);
        EmitSignalPeerConnected(peerId);
        Connect(steamId);
    }

    private void OnUserConnectFailed(ulong steamId)
    {
        Connected.Remove((int)((SteamId)steamId).AccountId);
    }

    private void OnUserDisconnected(ulong steamId)
    {
        var peerId = (int)((SteamId)steamId).AccountId;
        Connected.Remove(peerId);
        EmitSignalPeerDisconnected(peerId);
    }

    public void Connect(SteamId steamId)
    {
        SNetworking.SendP2P(steamId, HandShakeSend, Channel.Handshake);
    }

    public void ConnectReply(SteamId steamId)
    {
        SNetworking.SendP2P(steamId, HandShakeReply, Channel.Handshake);
    }

    public override void _Close()
    {
        foreach (var steamId in Connected.Values)
        {
            SNetworking.Instance.Disconnect(steamId);
            SNetworking.SendP2P(steamId, Disconnect, Channel.Handshake);
        }

        PacketQueue.Clear();
    }

    public override void _DisconnectPeer(int pPeer, bool pForce)
    {
        if (Connected.TryGetValue(pPeer, out var steamId))
        {
            SNetworking.Instance.Disconnect(steamId);
            SNetworking.SendP2P(steamId, Disconnect, Channel.Handshake);
        }
    }

    public override int _GetAvailablePacketCount()
    {
        return PacketQueue.Count;
    }

    public override ConnectionStatus _GetConnectionStatus()
    {
        return ConnectionStatus.Connected;
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
        return PacketQueue.TryPeek(out var packet) ? packet.PeerId : 0;
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
                TransferModeEnum.Unreliable => P2PSend.Unreliable,
                _ => P2PSend.Reliable
            };
            var channel = (Channel)TransferChannel;
            if (TargetPeer == 0)
            {
                foreach (var (peerId, steamId) in Connected)
                {
                    SNetworking.SendP2P(steamId, pBuffer, channel, sendType);
                }
            }
            else if (TargetPeer < 0)
            {
                foreach (var (peerId, steamId) in Connected)
                {
                    if (peerId != Mathf.Abs(TargetPeer))
                    {
                        SNetworking.SendP2P(steamId, pBuffer, channel, sendType);
                    }
                }
            }
            else
            {
                foreach (var (peerId, steamId) in Connected)
                {
                    if (peerId == TargetPeer)
                    {
                        SNetworking.SendP2P(steamId, pBuffer, channel, sendType);
                    }
                }
            }

            return Error.Ok;
        }
        catch (Exception e)
        {
            Log.Error($"{nameof(SteamworksP2PPeer)} 发送数据包异常, {e.Message}");
            return Error.Failed;
        }
    }
}