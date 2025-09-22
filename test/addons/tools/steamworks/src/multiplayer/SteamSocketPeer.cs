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
    public const string HostHost = "HostHost";
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

    public static async Task<SteamSocketPeer> CreateServer(int port, int maxUser = 4)
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
            Log.Error($"创建{nameof(SteamSocketPeer)}服务端异常, {e.Message}");
            throw;
        }
    }

    public static async Task<SteamSocketPeer> CreateClient(Lobby lobby)
    {
        try
        {
            var hostPort = lobby.GetData(HostPort);
            var lobbyOwner = lobby.Owner;
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

    protected override void DisconnectPeer(SteamId steamId)
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