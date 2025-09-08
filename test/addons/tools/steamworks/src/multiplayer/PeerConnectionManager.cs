using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Steamworks;
using Steamworks.Data;
using Test.addons.tools.steamworks.multiplayer;

namespace Godot;

public class PeerConnectionManager : IConnectionManager
{
    public SteamId SteamId { private set; get; }

    public readonly Queue<SteamMessage> PacketQueue = new();

    public MultiplayerPeer.ConnectionStatus ConnectionStatus { private set; get; } =
        MultiplayerPeer.ConnectionStatus.Connecting;

    public void OnConnecting(ConnectionInfo info)
    {
        ConnectionStatus = MultiplayerPeer.ConnectionStatus.Connecting;
    }

    public void OnConnected(ConnectionInfo info)
    {
        ConnectionStatus = MultiplayerPeer.ConnectionStatus.Connected;
        if (info.Identity.IsSteamId)
        {
            SteamId = info.Identity.SteamId;
        }
    }

    public void OnDisconnected(ConnectionInfo info)
    {
        ConnectionStatus = MultiplayerPeer.ConnectionStatus.Disconnected;
    }

    public unsafe void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
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