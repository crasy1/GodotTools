using System;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public class PeerConnectionManager(SteamworksClientPeer steamworksClientPeer) : IConnectionManager
{
    public SteamId SteamId { private set; get; }

    public readonly Queue<SteamworksMessagePacket> PacketQueue = new();

    public MultiplayerPeer.ConnectionStatus ConnectionStatus { private set; get; } =
        MultiplayerPeer.ConnectionStatus.Connecting;

    public void OnConnecting(ConnectionInfo info)
    {
        ConnectionStatus = MultiplayerPeer.ConnectionStatus.Connecting;
        Log.Info($"{info.Identity.SteamId} 正在连接");
    }

    public void OnConnected(ConnectionInfo info)
    {
        ConnectionStatus = MultiplayerPeer.ConnectionStatus.Connected;
        Log.Info($"{info.Identity.SteamId} 已连接");
        if (info.Identity.IsSteamId)
        {
            SteamId = info.Identity.SteamId;
        }
        steamworksClientPeer.EmitSignal(MultiplayerPeer.SignalName.PeerConnected, 1);
    }

    public void OnDisconnected(ConnectionInfo info)
    {
        ConnectionStatus = MultiplayerPeer.ConnectionStatus.Disconnected;
        Log.Info($"{info.Identity.SteamId} 断开连接");
        steamworksClientPeer.EmitSignal(MultiplayerPeer.SignalName.PeerDisconnected, 1);
    }

    public unsafe void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        Log.Debug($"从 {SteamId} 收到消息");
        var span = new Span<byte>((byte*)data.ToPointer(), size);
        var steamMessage = new SteamworksMessagePacket
        {
            SteamId = SteamId,
            Data = span.ToArray(),
            TransferChannel = channel,
            PeerId = 1
        };
        PacketQueue.Enqueue(steamMessage);
    }
}