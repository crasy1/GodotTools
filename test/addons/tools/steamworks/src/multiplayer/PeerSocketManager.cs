using System;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public class PeerSocketManager(SteamPeer steamPeer) : ISocketManager
{
    public readonly Dictionary<Connection, SteamId> ConnectionDict = new();
    public readonly Dictionary<SteamId, Connection> SteamIdDict = new();

    public void OnConnecting(Connection connection, ConnectionInfo info)
    {
        Log.Info($"{info.Identity.SteamId} 正在连接 {(steamPeer.RefuseNewConnections ? "拒绝" : "接受")}");
        if (!steamPeer.RefuseNewConnections)
        {
            connection.Accept();
        }
        else
        {
            connection.Close();
        }
    }

    public void OnConnected(Connection connection, ConnectionInfo info)
    {
        var steamId = info.Identity.SteamId;
        ConnectionDict.TryAdd(connection, steamId);
        SteamIdDict.TryAdd(steamId, connection);
        steamPeer.OnSocketConnected(steamId);
    }

    public void OnDisconnected(Connection connection, ConnectionInfo info)
    {var steamId = info.Identity.SteamId;
        ConnectionDict.Remove(connection);
        SteamIdDict.Remove(steamId);
        steamPeer.OnSocketDisconnected(steamId);
    }

    public unsafe void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum,
        long recvTime,
        int channel)
    {
        if (ConnectionDict.TryGetValue(connection, out var steamId))
        {
            var span = new Span<byte>((byte*)data.ToPointer(), size);
            steamPeer.ReceiveData(steamId, channel, span.ToArray());
        }
    }
}