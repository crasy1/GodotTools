using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public class MySocketManager : ISocketManager
{
    private SteamSocket SteamSocket { set; get; }
    private string Name { set; get; }

    public readonly Dictionary<Friend, List<Connection>> Connections = new();

    public MySocketManager(SteamSocket steamSocket)
    {
        SteamSocket = steamSocket;
        Name = SteamSocket.Name;
    }

    public void OnConnecting(Connection connection, ConnectionInfo info)
    {
        connection.Accept();
    }

    public void OnConnected(Connection connection, ConnectionInfo info)
    {
        if (info.Identity.IsSteamId)
        {
            var friend = SFriends.Friends[info.Identity.SteamId];
            if (Connections.TryGetValue(friend, out var connections))
            {
                connections.Add(connection);
            }
            else
            {
                Connections.Add(friend, new List<Connection> { connection });
            }

            if (GodotObject.IsInstanceValid(SteamSocket))
            {
                SteamSocket.EmitSignal(SteamSocket.SignalName.Connected, info.Identity.SteamId.Value);
            }
        }
    }

    public void OnDisconnected(Connection connection, ConnectionInfo info)
    {
        if (info.Identity.IsSteamId)
        {
            var friend = SFriends.Friends[info.Identity.SteamId];
            if (Connections.TryGetValue(friend, out var connections))
            {
                connections.Remove(connection);
            }

            if (GodotObject.IsInstanceValid(SteamSocket))
            {
                SteamSocket.EmitSignal(SteamSocket.SignalName.Disconnected, info.Identity.SteamId.Value);
            }
        }
    }

    public void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum,
        long recvTime,
        int channel)
    {
        if (GodotObject.IsInstanceValid(SteamSocket))
        {
            if (identity.IsSteamId)
            {
                var friend = SFriends.Friends[identity.SteamId];
                var msg = Marshal.PtrToStringUTF8(data, size);
                Log.Info($"{Name} 从 {friend.Id},{friend.Name} 收到信息 {msg}");
                SteamSocket.EmitSignal(SteamSocket.SignalName.ReceiveMessage, identity.SteamId.Value, msg);
            }
        }
    }
}