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
        ConnectionDict.TryAdd(connection, info.Identity.SteamId);
        SteamIdDict.TryAdd(info.Identity.SteamId, connection);
    }

    public void OnDisconnected(Connection connection, ConnectionInfo info)
    {
        Log.Info($"{info.Identity.SteamId} 断开连接");
        ConnectionDict.Remove(connection);
        SteamIdDict.Remove(info.Identity.SteamId);
        steamPeer.Close();
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