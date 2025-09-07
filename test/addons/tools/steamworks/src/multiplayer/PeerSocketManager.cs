using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public class PeerSocketManager : ISocketManager
{
    private string Name { set; get; }

    public readonly Dictionary<Connection, SteamId> Connections = new();

    public MultiplayerPeer.ConnectionStatus ConnectionStatus { private set; get; } =
        MultiplayerPeer.ConnectionStatus.Connected;

    public PeerSocketManager()
    {
    }

    public void OnConnecting(Connection connection, ConnectionInfo info)
    {
        connection.Accept();
    }

    public void OnConnected(Connection connection, ConnectionInfo info)
    {
        if (info.Identity.IsSteamId)
        {
            Connections.TryAdd(connection, info.Identity.SteamId);
        }
    }

    public void OnDisconnected(Connection connection, ConnectionInfo info)
    {
        if (info.Identity.IsSteamId)
        {
            Connections.Remove(connection);
        }
    }

    public void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum,
        long recvTime,
        int channel)
    {
        if (identity.IsSteamId)
        {
            var msg = Marshal.PtrToStringUTF8(data, size);
        }
    }
}