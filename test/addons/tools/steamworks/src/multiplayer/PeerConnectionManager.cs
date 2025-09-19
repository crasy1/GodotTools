using System;
using System.Collections.Generic;
using System.Text;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public class PeerConnectionManager(MultiplayerPeerExtension steamworksClientPeer) : IConnectionManager
{
    public SteamId SteamId { private set; get; }

    public readonly Queue<SteamworksMessagePacket> PacketQueue = new();

    public MultiplayerPeer.ConnectionStatus ConnectionStatus { private set; get; } =
        MultiplayerPeer.ConnectionStatus.Connecting;

    public void OnConnecting(ConnectionInfo info)
    {
        ConnectionStatus = MultiplayerPeer.ConnectionStatus.Connecting;
        Log.Info($"{steamworksClientPeer.GetUniqueId()} 与服务器{info.Identity.SteamId} 正在连接");
    }

    public void OnConnected(ConnectionInfo info)
    {
        SteamId = info.Identity.SteamId;
        ConnectionStatus = MultiplayerPeer.ConnectionStatus.Connected;
        Log.Info($"{steamworksClientPeer.GetUniqueId()} 与服务器{SteamId} 已连接");
        steamworksClientPeer.EmitSignal(MultiplayerPeer.SignalName.PeerConnected, 1);
        if (steamworksClientPeer is NormalClientPeer normalClientPeer)
        {
            normalClientPeer.ConnectionManager.Connection.UserData = 1;
            normalClientPeer.ConnectionManager.Connection.SendMessage(
                Encoding.UTF8.GetBytes($"{Consts.SocketHandShake}{steamworksClientPeer.GetUniqueId()}"));
        }if (steamworksClientPeer is SteamworksClientPeer relayClientPeer)
        {
            relayClientPeer.ConnectionManager.Connection.UserData = 1;
            relayClientPeer.ConnectionManager.Connection.SendMessage(
                Encoding.UTF8.GetBytes($"{Consts.SocketHandShake}{steamworksClientPeer.GetUniqueId()}"));
        }
    }

    public void OnDisconnected(ConnectionInfo info)
    {
        ConnectionStatus = MultiplayerPeer.ConnectionStatus.Disconnected;
        Log.Info($"{steamworksClientPeer.GetUniqueId()} 与服务器{info.Identity.SteamId} 断开连接");
        steamworksClientPeer.EmitSignal(MultiplayerPeer.SignalName.PeerDisconnected, 1);
    }

    public unsafe void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        Log.Debug($"{steamworksClientPeer.GetUniqueId()} 从服务器 {SteamId} 收到消息");
        var span = new Span<byte>((byte*)data.ToPointer(), size);
        var steamMessage = new SteamworksMessagePacket
        {
            SteamId = SteamId,
            Data =  span.ToArray(),
            TransferChannel = channel,
            PeerId = 1
        };

        PacketQueue.Enqueue(steamMessage);
    }
}