using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public class PeerSocketManager(MultiplayerPeerExtension steamworksServerPeer) : ISocketManager
{
    public readonly Dictionary<Connection, SteamId> Connections = new();

    public readonly Queue<SteamworksMessagePacket> PacketQueue = new();
    private const string P2PHandShake = "[P2P_HANDSHAKE]";

    public MultiplayerPeer.ConnectionStatus ConnectionStatus { private set; get; } =
        MultiplayerPeer.ConnectionStatus.Connected;


    public void OnConnecting(Connection connection, ConnectionInfo info)
    {
        Log.Info($"{info.Identity.SteamId} 正在连接");
        if (!steamworksServerPeer.RefuseNewConnections)
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
        if (info.Identity.IsSteamId)
        {
            Connections.TryAdd(connection, info.Identity.SteamId);
        }
    }

    public void OnDisconnected(Connection connection, ConnectionInfo info)
    {
        Log.Info($"{info.Identity.SteamId} 断开连接");
        if (info.Identity.IsSteamId)
        {
            Connections.Remove(connection);
        }

        steamworksServerPeer.EmitSignal(MultiplayerPeer.SignalName.PeerDisconnected,
            (int)info.Identity.SteamId.AccountId);
    }

    public unsafe void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum,
        long recvTime,
        int channel)
    {
        // Log.Debug($"从 {identity.SteamId} 收到消息");
        if (identity.IsSteamId && Connections.TryGetValue(connection, out var steamId))
        {
            var connectionKey = Connections.Keys.First(c => c == connection);
            var span = new Span<byte>((byte*)data.ToPointer(), size);
            var bytes = span.ToArray();
            var s = Encoding.UTF8.GetString(bytes);
            if (s.StartsWith(P2PHandShake))
            {
                var peerId = s.Replace(P2PHandShake, "").ToInt();
                connectionKey.UserData = peerId;
                Log.Info($"{steamId} 已经连接{peerId}");

                steamworksServerPeer.EmitSignal(MultiplayerPeer.SignalName.PeerConnected, peerId);

                return;
            }

            var steamMessage = new SteamworksMessagePacket
            {
                SteamId = steamId,
                Data = span.ToArray(),
                TransferChannel = channel,
                PeerId = (int)connectionKey.UserData
            };
            PacketQueue.Enqueue(steamMessage);
        }
    }
}