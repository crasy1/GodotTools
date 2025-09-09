using System;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public class PeerConnectionManager : IConnectionManager
{
    public SteamId SteamId { private set; get; }

    public readonly Queue<SteamMessage> PacketQueue = new();
    public SteamworksClientPeer SteamworksClientPeer { set; get; }

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
        SteamworksClientPeer.EmitSignal(MultiplayerPeer.SignalName.PeerConnected, 1);
        Log.Info($"{info.Identity.SteamId} 已连接");
        if (info.Identity.IsSteamId)
        {
            SteamId = info.Identity.SteamId;
        }
    }

    public void OnDisconnected(ConnectionInfo info)
    {
        ConnectionStatus = MultiplayerPeer.ConnectionStatus.Disconnected;
        SteamworksClientPeer.EmitSignal(MultiplayerPeer.SignalName.PeerDisconnected, 1);
        Log.Info($"{info.Identity.SteamId} 断开连接");
    }

    public unsafe void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        Log.Info($"从 {SteamId} 收到消息");
        var span = new Span<byte>((byte*)data.ToPointer(), size);
        var steamMessage = new SteamMessage
        {
            PeerId = (int)SteamId.AccountId,
            Data = span.ToArray(),
            TransferChannel = channel
        };
        PacketQueue.Enqueue(steamMessage);
    }
}