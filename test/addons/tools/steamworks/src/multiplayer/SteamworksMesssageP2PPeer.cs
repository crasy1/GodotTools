using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 相当于steamworks networking message p2p的包装
/// https://partner.steamgames.com/doc/api/ISteamNetworkingMessages
/// </summary>
public partial class SteamworksMessageP2PPeer : MultiplayerPeerExtension
{
    private int TargetPeer { set; get; }
    private const string HandShakeSend = "[HANDSHAKE_SEND]";
    private const string HandShakeReply = "[HANDSHAKE_REPLY]";

    private readonly Queue<SteamworksMessagePacket> PacketQueue = new();
    private readonly Dictionary<int, NetIdentity> Connected = new();

    public SteamworksMessageP2PPeer()
    {
        SteamNetworkingMessages.OnSessionRequest += OnSessionRequest;
        SteamNetworkingMessages.OnSessionFailed += OnSessionFailed;
        SteamNetworkingMessages.OnMessage += OnMessage;
        Log.Info($"创建 {nameof(SteamworksMessageP2PPeer)}");
        SteamManager.AddBeforeGameQuitAction(Close);
    }

    private unsafe void OnMessage(NetIdentity identity, IntPtr data, int size, int channel)
    {
        var steamId = identity.SteamId;
        var span = new Span<byte>((byte*)data.ToPointer(), size);
        var steamMessage = new SteamworksMessagePacket
        {
            SteamId = steamId,
            Data = span.ToArray(),
            TransferChannel = channel
        };
        PacketQueue.Enqueue(steamMessage);
        // if (channel == (int)Channel.Handshake)
        // {
        //     var msg = Encoding.UTF8.GetString(steamMessage.Data);
        //     if (HandShakeSend == msg)
        //     {
        //         if (!Connected.Values.Contains(steamId))
        //         {
        //             OnUserConnected(steamId);
        //             ConnectReply(steamId);
        //         }
        //     }
        //     else if (HandShakeReply == msg)
        //     {
        //         if (!Connected.Values.Contains(steamId))
        //         {
        //             OnUserConnected(steamId);
        //         }
        //     }
        //
        //     Log.Debug($"{nameof(SteamworksMessageP2PPeer)} handshake {msg}");
        //     return;
        // }
    }

    private void OnSessionFailed(ConnectionInfo connectionInfo)
    {
        OnUserConnectFailed(connectionInfo.Identity.SteamId);
    }

    private void OnSessionRequest(NetIdentity identity)
    {
        if (!RefuseNewConnections)
        {
            SteamNetworkingMessages.AcceptSessionWithUser(ref identity);
            OnUserConnected(identity.SteamId);
        }
    }

    private void OnUserConnected(ulong steamId)
    {
        var sid = (SteamId)steamId;
        var peerId = (int)sid.AccountId;
        Connected.TryAdd(peerId, sid);
        EmitSignalPeerConnected(peerId);
    }

    private void OnUserConnectFailed(ulong steamId)
    {
        var sid = (SteamId)steamId;
        var peerId = (int)sid.AccountId;
        Connected.Remove(peerId);
        EmitSignalPeerDisconnected(peerId);
    }

    private void OnUserDisconnected(ulong steamId)
    {
        var sid = (SteamId)steamId;
        var peerId = (int)sid.AccountId;
        Connected.Remove(peerId);
        EmitSignalPeerDisconnected(peerId);
    }

    public void Connect(NetIdentity steamId)
    {
        var result = SendMsg(steamId, HandShakeSend, Channel.Handshake);
    }

    public void ConnectReply(NetIdentity steamId)
    {
        SendMsg(steamId, HandShakeReply, Channel.Handshake);
    }

    public Result SendMsg(NetIdentity steamId, string data, Channel channel = Channel.Msg,
        SendType sendType = SendType.Reliable)
    {
        return SendMsg(steamId, Encoding.UTF8.GetBytes(data), channel, sendType);
    }

    public Result SendMsg(NetIdentity steamId, byte[] data, Channel channel = Channel.Msg,
        SendType sendType = SendType.Reliable)
    {
        return SteamNetworkingMessages.SendMessageToUser(ref steamId, data, data.Length, (int)channel, sendType);
    }


    public override void _Close()
    {
        foreach (var steamId in Connected.Values)
        {
            var netIdentity = steamId;
            SteamNetworkingMessages.CloseSessionWithUser(ref netIdentity);
        }

        PacketQueue.Clear();
    }

    public override void _DisconnectPeer(int pPeer, bool pForce)
    {
        if (Connected.TryGetValue(pPeer, out var steamId))
        {
            SteamNetworkingMessages.CloseSessionWithUser(ref steamId);
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
        foreach (var channel in SNetworking.Channels)
        {
            // 处理每个通道的消息
            SteamNetworkingMessages.Receive(channel);
        }
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
                TransferModeEnum.Unreliable => SendType.Unreliable,
                _ => SendType.Reliable
            };
            var channel = (Channel)TransferChannel;
            if (TargetPeer == 0)
            {
                foreach (var (peerId, steamId) in Connected)
                {
                    SendMsg(steamId, pBuffer, channel, sendType);
                }
            }
            else if (TargetPeer < 0)
            {
                foreach (var (peerId, steamId) in Connected)
                {
                    if (peerId != Mathf.Abs(TargetPeer))
                    {
                        SendMsg(steamId, pBuffer, channel, sendType);
                    }
                }
            }
            else
            {
                foreach (var (peerId, steamId) in Connected)
                {
                    if (peerId == TargetPeer)
                    {
                        SendMsg(steamId, pBuffer, channel, sendType);
                    }
                }
            }

            return Error.Ok;
        }
        catch (Exception e)
        {
            Log.Error($"{nameof(SteamworksMessageP2PPeer)} 发送数据包异常, {e.Message}");
            return Error.Failed;
        }
    }
}