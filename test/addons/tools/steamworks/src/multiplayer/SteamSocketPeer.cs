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
    // TODO 实现normal版本和优化peer信号
    public const string HostPort = "HostPort";

    /// <summary>
    /// 服务端配置
    /// </summary>
    private PeerSocketManager PeerSocketManager { set; get; }

    private SocketManager SocketManager { set; get; }

    /// <summary>
    /// 客户端配置
    /// </summary>
    private PeerConnectionManager PeerConnectionManager { set; get; }

    public ConnectionManager ConnectionManager { set; get; }

    public static async Task<SteamSocketPeer> CreateRelayServer(int port, int maxUser = 4)
    {
        try
        {
            var peer = new SteamSocketPeer();
            var peerSocketManager = new PeerSocketManager(peer);
            var socketManager = SteamNetworkingSockets.CreateRelaySocket(port, peerSocketManager);
            peer.PeerSocketManager = peerSocketManager;
            peer.SocketManager = socketManager;
            peer.PeerId = ServerPeerId;
            var lobby = await peer.CreateLobby(maxUser);
            lobby.SetData(HostPort, port.ToString());
            return peer;
        }
        catch (Exception e)
        {
            Log.Error($"创建{nameof(SteamSocketPeer)} relay服务端异常, {e.Message}");
            throw;
        }
    }

    public static async Task<SteamSocketPeer> CreateNormalServer(int port, int maxUser = 4)
    {
        try
        {
            var peer = new SteamSocketPeer();
            var peerSocketManager = new PeerSocketManager(peer);
            var socketManager =
                SteamNetworkingSockets.CreateNormalSocket(NetAddress.AnyIp((ushort)port), peerSocketManager);
            peer.PeerSocketManager = peerSocketManager;
            peer.SocketManager = socketManager;
            peer.PeerId = ServerPeerId;
            var lobby = await peer.CreateLobby(maxUser);
            lobby.SetData(HostPort, port.ToString());
            return peer;
        }
        catch (Exception e)
        {
            Log.Error($"创建{nameof(SteamSocketPeer)} normal服务端异常, {e.Message}");
            throw;
        }
    }

    public static async Task<SteamSocketPeer> CreateRelayClient(Lobby? lobby)
    {
        try
        {
            if (!lobby.HasValue)
            {
                throw new Exception("未找到大厅");
            }

            var hostPort = lobby.Value.GetData(HostPort);
            Log.Info($"relayserver hostPort:{hostPort}");
            var lobbyOwner = lobby.Value.Owner;
            var peer = new SteamSocketPeer();
            var peerConnectionManager = new PeerConnectionManager(peer);
            var connectionManager =
                SteamNetworkingSockets.ConnectRelay(lobbyOwner.Id, int.Parse(hostPort), peerConnectionManager);
            peer.PeerConnectionManager = peerConnectionManager;
            peer.ConnectionManager = connectionManager;
            peer.PeerId = (int)SteamClient.SteamId.AccountId;
            await peer.JoinLobby(lobby);
            return peer;
        }
        catch (Exception e)
        {
            Log.Error($"创建{nameof(SteamSocketPeer)}客户端异常, {e.Message}");
            throw;
        }
    }

    public static async Task<SteamSocketPeer> CreateNormalClient(string host, Lobby? lobby)
    {
        try
        {
            if (!lobby.HasValue)
            {
                throw new Exception("未找到大厅");
            }

            var hostPort = lobby.Value.GetData(HostPort);
            var peer = new SteamSocketPeer();
            var peerConnectionManager = new PeerConnectionManager(peer);
            var connectionManager =
                SteamNetworkingSockets.ConnectNormal(NetAddress.From(host, ushort.Parse(hostPort)),
                    peerConnectionManager);
            peer.PeerConnectionManager = peerConnectionManager;
            peer.ConnectionManager = connectionManager;
            peer.PeerId = (int)SteamClient.SteamId.AccountId;
            await peer.JoinLobby(lobby);
            return peer;
        }
        catch (Exception e)
        {
            Log.Error($"创建{nameof(SteamSocketPeer)}客户端异常, {e.Message}");
            throw;
        }
    }

    protected override void OnCreate()
    {
        SteamMatchmaking.OnLobbyDataChanged += OnLobbyDataChanged;
    }

    protected override void OnClose()
    {
        SteamMatchmaking.OnLobbyDataChanged -= OnLobbyDataChanged;
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

    private void OnLobbyDataChanged(Lobby lobby)
    {
        Log.Info($"HostPort:{lobby.GetData("HostPort")}");
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
            return PeerSocketManager.SteamIdDict[steamId].SendMessage(data, sendType, (ushort)channel) == Result.OK;
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
            PeerSocketManager.SteamIdDict[steamId].Close();
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