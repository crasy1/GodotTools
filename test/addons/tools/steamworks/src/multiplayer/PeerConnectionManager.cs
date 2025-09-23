using System;
using System.Collections.Generic;
using System.Text;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public class PeerConnectionManager(SteamPeer steamPeer) : IConnectionManager
{
    private SteamId SteamId { set; get; }

    public void OnConnecting(ConnectionInfo info)
    {
        steamPeer.ConnectionStatus = MultiplayerPeer.ConnectionStatus.Connecting;
    }

    public void OnConnected(ConnectionInfo info)
    {
        SteamId = info.Identity.SteamId;
        steamPeer.ConnectionStatus = MultiplayerPeer.ConnectionStatus.Connected;
        steamPeer.OnSocketConnected(SteamId);
    }

    public void OnDisconnected(ConnectionInfo info)
    {
        steamPeer.ConnectionStatus = MultiplayerPeer.ConnectionStatus.Disconnected;
        steamPeer.OnSocketDisconnected(info.Identity.SteamId);
    }

    public unsafe void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        var span = new Span<byte>((byte*)data.ToPointer(), size);
        steamPeer.ReceiveData(SteamId, channel, span.ToArray());
    }
}