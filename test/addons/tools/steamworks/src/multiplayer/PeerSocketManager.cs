using System;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public class PeerSocketManager : ISocketManager
{
    public readonly Dictionary<Connection, SteamId> Connections = new();

    public readonly Queue<SteamMessage> PacketQueue = new();

    public MultiplayerPeer.ConnectionStatus ConnectionStatus { private set; get; } =
        MultiplayerPeer.ConnectionStatus.Connected;


    public void OnConnecting(Connection connection, ConnectionInfo info)
    {
        Log.Info( $"{info.Identity.IsSteamId} 正在连接");
        connection.Accept();
    }

    public void OnConnected(Connection connection, ConnectionInfo info)
    {
        Log.Info( $"{info.Identity.IsSteamId} 已经连接");
        if (info.Identity.IsSteamId)
        {
            Connections.TryAdd(connection, info.Identity.SteamId);
        }
    }

    public void OnDisconnected(Connection connection, ConnectionInfo info)
    {
        Log.Info( $"{info.Identity.IsSteamId} 断开连接");
        if (info.Identity.IsSteamId)
        {
            Connections.Remove(connection);
        }
    }

    public unsafe void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum,
        long recvTime,
        int channel)
    {
        Log.Info( $"从 {identity.SteamId} 收到消息");
        if (identity.IsSteamId && Connections.TryGetValue(connection, out var steamId))
        {
            var span = new Span<byte>((byte*)data.ToPointer(), size);
            var steamMessage = new SteamMessage
            {
                PeerId = (int)steamId.AccountId,
                Data = span.ToArray(),
                TransferChannel = channel
            };
            PacketQueue.Enqueue(steamMessage);
        }
    }
}