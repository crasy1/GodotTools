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
    private int PeerId { set; get; }

    private readonly Queue<SteamworksMessagePacket> PacketQueue = new();
    private readonly Dictionary<int, SteamId> Connected = new();

    private SteamworksP2PPeer()
    {
        SNetworking.Instance.UserConnectFailed += (steamId) =>
        {
            var peerId = Connected.FirstOrDefault(kv => kv.Value == steamId).Key;
            OnUserConnectFailed(peerId, steamId);
        };
        SNetworking.Instance.UserDisconnected += (steamId) =>
        {
            var peerId = Connected.FirstOrDefault(kv => kv.Value == steamId).Key;
            OnUserDisconnected(peerId, steamId);
        };
        SNetworking.Instance.ReceiveData += OnReceiveData;
        Log.Info($"创建 {nameof(SteamworksP2PPeer)}");
        SteamManager.AddBeforeGameQuitAction(Close);
    }

    public static SteamworksP2PPeer CreateServer()
    {
        var server = new SteamworksP2PPeer();
        server.PeerId = 1;
        return server;
    }

    public static SteamworksP2PPeer CreateClient(SteamId steamId)
    {
        var client = new SteamworksP2PPeer();
        client.PeerId = (int)client.GenerateUniqueId();
        client.Connect(steamId);
        return client;
    }

    private void OnReceiveData(ulong steamId, int channel, byte[] data)
    {
        int peerId;
        if (channel == (int)Channel.Handshake)
        {
            var msg = Encoding.UTF8.GetString(data);
            if (msg.StartsWith(Consts.SocketHandShake))
            {
                peerId = int.Parse(msg.Replace(Consts.SocketHandShake, ""));
                ConnectReply(steamId);
                OnUserConnected(peerId, steamId);
            }
            else if (msg.StartsWith(Consts.SocketHandShakeReply))
            {
                peerId = int.Parse(msg.Replace(Consts.SocketHandShakeReply, ""));
                OnUserConnected(peerId, steamId);
            }
            else if (msg.StartsWith(Consts.SocketDisconnect))
            {
                peerId = int.Parse(msg.Replace(Consts.SocketDisconnect, ""));
                OnUserDisconnected(peerId, steamId);
            }

            return;
        }

        peerId = Connected.FirstOrDefault(kv => kv.Value == steamId).Key;
        var p2Packet = new SteamworksMessagePacket()
        {
            SteamId = steamId,
            Data = data,
            TransferChannel = channel,
            PeerId = peerId
        };
        PacketQueue.Enqueue(p2Packet);
    }

    private void OnUserConnected(int peerId, SteamId steamId)
    {
        if (Connected.TryAdd(peerId, steamId))
        {
            EmitSignalPeerConnected(peerId);
        }
    }

    private void OnUserConnectFailed(int peerId, SteamId steamId)
    {
        if (Connected.Remove(peerId))
        {
            EmitSignalPeerDisconnected(peerId);
        }
    }

    private void OnUserDisconnected(int peerId, SteamId steamId)
    {
        if (Connected.Remove(peerId))
        {
            SteamNetworking.CloseP2PSessionWithUser(steamId);
            EmitSignalPeerDisconnected(peerId);
        }
    }

    public void Connect(SteamId steamId)
    {
        SNetworking.SendP2P(steamId, Consts.SocketHandShake + PeerId, Channel.Handshake);
    }

    public void ConnectReply(SteamId steamId)
    {
        SNetworking.SendP2P(steamId, Consts.SocketHandShakeReply + PeerId, Channel.Handshake);
    }

    public override void _Close()
    {
        foreach (var peerId in Connected.Keys)
        {
            DisconnectPeer(peerId);
        }

        PacketQueue.Clear();
    }

    public override void _DisconnectPeer(int pPeer, bool pForce)
    {
        if (Connected.TryGetValue(pPeer, out var steamId))
        {
            SNetworking.SendP2P(steamId, Consts.SocketDisconnect + PeerId, Channel.Handshake);
            EmitSignalPeerDisconnected(pPeer);
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
        return PeerId;
    }

    public override bool _IsServer()
    {
        return PeerId == 1;
    }

    public override bool _IsServerRelaySupported()
    {
        return true;
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