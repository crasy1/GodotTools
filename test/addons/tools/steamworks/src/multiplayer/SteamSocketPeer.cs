using System;
using System.Linq;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 相当于 <see href="https://partner.steamgames.com/doc/api/ISteamNetworkingSockets">steamworks networking sockets</see> relay的包装
/// </summary>
public partial class SteamSocketPeer : SteamPeer
{
    public const int DefaultPort = 60937;

    private int Port { set; get; } = DefaultPort;
    private string Host { set; get; }

    /// <summary>
    /// 服务端配置
    /// </summary>
    private PeerSocketManager PeerSocketManager { set; get; }

    private SocketManager SocketManager { set; get; }

    /// <summary>
    /// 客户端配置
    /// </summary>
    private PeerConnectionManager PeerConnectionManager { set; get; }

    private ConnectionManager ConnectionManager { set; get; }

    private SteamSocketPeer(int peerId, SteamSocketType socketType) : base(peerId, socketType)
    {
    }

    /// <summary>
    /// 需要创建端口
    /// </summary>
    /// <param name="port"></param>
    /// <param name="maxUser"></param>
    /// <returns></returns>
    public static async Task<SteamSocketPeer> CreateRelayServer(int port, int maxUser = 4)
    {
        var peer = new SteamSocketPeer(ServerPeerId, SteamSocketType.Relay);
        try
        {
            await CreateLobby(maxUser);
            var peerSocketManager = new PeerSocketManager(peer);
            var socketManager = SteamNetworkingSockets.CreateRelaySocket(port, peerSocketManager);
            peer.PeerSocketManager = peerSocketManager;
            peer.SocketManager = socketManager;
            return peer;
        }
        catch (Exception e)
        {
            Log.Error($"创建 relay peer 服务端异常, {e.Message}");
            peer.Lobby?.Leave();
            throw;
        }
    }

    public static async Task<SteamSocketPeer> CreateNormalServer(ushort port, int maxUser = 4)
    {
        var peer = new SteamSocketPeer(ServerPeerId, SteamSocketType.Normal);
        try
        {
            await CreateLobby(maxUser);
            var peerSocketManager = new PeerSocketManager(peer);
            var socketManager = SteamNetworkingSockets.CreateNormalSocket(NetAddress.AnyIp(port), peerSocketManager);
            peer.PeerSocketManager = peerSocketManager;
            peer.SocketManager = socketManager;
            return peer;
        }
        catch (Exception e)
        {
            Log.Error($"创建 normal peer 服务端异常, {e.Message}");
            peer.Lobby?.Leave();
            throw;
        }
    }

    public static async Task<SteamSocketPeer> CreateRelayClient(Lobby lobby, int port)
    {
        var peer = new SteamSocketPeer((int)SteamClient.SteamId.AccountId, SteamSocketType.Relay);
        try
        {
            await JoinLobby(lobby);
            var lobbyOwner = lobby.Owner;
            var peerConnectionManager = new PeerConnectionManager(peer);
            var connectionManager =
                SteamNetworkingSockets.ConnectRelay(lobbyOwner.Id, port, peerConnectionManager);
            peer.PeerConnectionManager = peerConnectionManager;
            peer.ConnectionManager = connectionManager;
            return peer;
        }
        catch (Exception e)
        {
            Log.Error($"创建 relay peer 客户端异常, {e.Message}");
            peer.Lobby?.Leave();
            throw;
        }
    }

    public static async Task<SteamSocketPeer> CreateNormalClient(string host, ushort port, Lobby lobby)
    {
        var peer = new SteamSocketPeer((int)SteamClient.SteamId.AccountId, SteamSocketType.Normal);
        try
        {
            await JoinLobby(lobby);
            var peerConnectionManager = new PeerConnectionManager(peer);
            var connectionManager =
                SteamNetworkingSockets.ConnectNormal(NetAddress.From(host, port), peerConnectionManager);
            peer.PeerConnectionManager = peerConnectionManager;
            peer.ConnectionManager = connectionManager;
            return peer;
        }
        catch (Exception e)
        {
            Log.Error($"创建 normal peer 客户端异常, {e.Message}");
            peer.Lobby?.Leave();
            throw;
        }
    }

    protected override void OnCreate()
    {
    }

    protected override void OnClose()
    {
        if (_IsServer())
        {
            foreach (var connection in PeerSocketManager.ConnectionDict.Keys)
            {
                connection.Close();
            }

            SocketManager.Close();
        }
        else
        {
            ConnectionManager.Close();
        }
    }

    public override void ReceiveData(ulong steamId, int channel, byte[] data)
    {
        var peerId = Lobby!.Value.IsOwnedBy(steamId) ? ServerPeerId : (int)((SteamId)steamId).AccountId;
        ProcessData(peerId, steamId, channel, data);
    }

    protected override bool SendMsg(SteamId steamId, byte[] data, Channel channel = Channel.Msg,
        SendType sendType = SendType.Reliable)
    {
        if (_IsServer())
        {
            if (PeerSocketManager.SteamIdDict.TryGetValue(steamId, out var connection))
            {
                return connection.SendMessage(data, sendType, (ushort)channel) == Result.OK;
            }

            return false;
        }
        else
        {
            return ConnectionManager.Connection.SendMessage(data, sendType, (ushort)channel) == Result.OK;
        }
    }

    protected override void OnPeerDisconnect(SteamId steamId)
    {
        if (_IsServer())
        {
            if (PeerSocketManager.SteamIdDict.TryGetValue(steamId, out var connection))
            {
                connection.Close();
            }
        }
        else
        {
            ConnectionManager.Close();
        }
    }

    protected override bool ServerRelaySupported()
    {
        return true;
    }

    protected override void Receive()
    {
        if (_IsServer())
        {
            SocketManager.Receive();
        }
        else
        {
            ConnectionManager.Receive();
        }
    }
}