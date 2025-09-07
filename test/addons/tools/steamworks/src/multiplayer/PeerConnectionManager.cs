using System;
using System.Runtime.InteropServices;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public class PeerConnectionManager : IConnectionManager
{
    private string Name { set; get; }

    public SteamId SteamId { private set; get; }

    public MultiplayerPeer.ConnectionStatus ConnectionStatus { private set; get; } =
        MultiplayerPeer.ConnectionStatus.Connecting;

    public PeerConnectionManager()
    {
    }

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

    public void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        var msg = Marshal.PtrToStringUTF8(data, size);
    }
}