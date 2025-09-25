using System;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 扩展steam多人游戏
/// 服务端用法，创建一个SteamMultiPlayer对象，调用CreateLobbyAsync方法创建一个大厅，
/// 创建 MultiplayerPeer server 赋值给 SteamMultiPlayer，正常连接就能使用了，要退出的话调用 LeaveLobby 离开大厅并关闭 MultiplayerPeer
/// 客户端用法，创建一个SteamMultiPlayer对象，然后调用JoinLobbyAsync方法加入一个大厅，
/// 创建 MultiplayerPeer client 赋值给 SteamMultiPlayer，正常连接就能使用了，要退出的话调用 LeaveLobby 离开大厅并关闭 MultiplayerPeer
/// </summary>
public partial class SteamMultiPlayer : MultiplayerApiExtension
{
    private SceneMultiplayer SceneMultiplayer = new();
    private OfflineMultiplayerPeer OfflinePeer = new();
    private Lobby Lobby { set; get; }

    public SteamMultiPlayer()
    {
        SMatchmaking.Instance.LobbyCreated += OnLobbyCreated;
        SMatchmaking.Instance.LobbyEntered += OnLobbyEntered;
        SMatchmaking.Instance.LobbyLeaved += OnLobbyLeaved;
        SMatchmaking.Instance.LobbyInvite += OnLobbyInvite;
        SMatchmaking.Instance.LobbyMemberJoined += OnLobbyMemberJoined;
        SMatchmaking.Instance.LobbyMemberLeave += OnLobbyMemberLeave;
        SMatchmaking.Instance.LobbyMemberDisconnected += OnLobbyMemberDisconnected;
        SMatchmaking.Instance.LobbyMemberDataChanged += OnLobbyMemberDataChanged;
        SMatchmaking.Instance.LobbyDataChanged += OnLobbyDataChanged;
        SMatchmaking.Instance.LobbyChatMessage += OnLobbyChatMessage;
        SMatchmaking.Instance.LobbyMemberKick += OnLobbyMemberKick;

        SceneMultiplayer.PeerConnected += OnPeerConnected;
        SceneMultiplayer.PeerDisconnected += OnPeerDisconnected;
        SceneMultiplayer.ServerDisconnected += OnServerDisconnected;
        SceneMultiplayer.ConnectedToServer += OnConnectedToServer;
        SceneMultiplayer.ConnectionFailed += OnConnectionFailed;
    }

    private void OnConnectionFailed()
    {
        LeaveLobby();
        EmitSignalConnectionFailed();
    }

    private void OnConnectedToServer()
    {
        EmitSignalConnectedToServer();
    }

    private void OnServerDisconnected()
    {
        LeaveLobby();
        EmitSignalServerDisconnected();
    }

    private void OnPeerDisconnected(long id)
    {
        EmitSignalPeerDisconnected(id);
    }

    private void OnPeerConnected(long id)
    {
        EmitSignalPeerConnected(id);
    }

    private void OnLobbyMemberKick(ulong lobbyId, ulong steamId)
    {
    }

    private void OnLobbyChatMessage(ulong lobbyId, ulong steamId, string message)
    {
    }

    private void OnLobbyDataChanged(ulong lobbyId)
    {
    }

    private void OnLobbyMemberDataChanged(ulong lobbyId, ulong steamId)
    {
    }

    private void OnLobbyMemberDisconnected(ulong lobbyId, ulong steamId)
    {
    }

    private void OnLobbyMemberLeave(ulong lobbyId, ulong steamId)
    {
    }

    private void OnLobbyMemberJoined(ulong lobbyId, ulong steamId)
    {
    }

    private void OnLobbyInvite(ulong lobbyId, ulong steamId)
    {
    }

    private void OnLobbyEntered(ulong lobbyId)
    {
        Lobby = new Lobby(lobbyId);
    }

    private void OnLobbyLeaved(ulong lobbyId)
    {
        Lobby = new Lobby();
    }

    private void OnLobbyCreated(int result, ulong lobbyId)
    {
        if (result == (int)Result.OK)
        {
            Lobby = new Lobby(lobbyId);
        }
    }

    /// <summary>
    /// 退出大厅，并关闭MultiplayerPeer
    /// </summary>
    public void LeaveLobby()
    {
        SMatchmaking.LeaveLobby();
        if (IsMultiplayerPeerValid())
        {
            MultiplayerPeer.Close();
            MultiplayerPeer = OfflinePeer;
        }
    }

    private bool IsMultiplayerPeerValid()
    {
        return MultiplayerPeer is not null && MultiplayerPeer is not OfflineMultiplayerPeer;
    }

    /// <summary>
    /// 创建一个大厅，如果失败则不允许用多人游戏
    /// </summary>
    /// <param name="maxUser"></param>
    /// <exception cref="Exception"></exception>
    public async Task CreateLobbyAsync(int maxUser)
    {
        var lobby = await SMatchmaking.CreateLobbyAsync(maxUser);
        if (!lobby.HasValue)
        {
            LeaveLobby();
            throw new Exception($"{nameof(SteamMultiPlayer)} 创建大厅异常");
        }

        Lobby = lobby.Value;
    }

    /// <summary>
    /// 加入好友大厅
    /// </summary>
    /// <param name="friend"></param>
    /// <exception cref="Exception"></exception>
    public async Task JoinLobbyAsync(Friend friend)
    {
        var lobby = friend.GameInfo?.Lobby;
        if (!lobby.HasValue)
        {
            throw new Exception($"{nameof(SteamMultiPlayer)} 未找到大厅");
        }

        await JoinLobbyAsync(lobby.Value);
    }

    /// <summary>
    /// 加入指定大厅
    /// </summary>
    /// <param name="lobby"></param>
    /// <exception cref="Exception"></exception>
    public async Task JoinLobbyAsync(Lobby lobby)
    {
        var result = await SMatchmaking.JoinLobbyAsync(lobby);
        if (!RoomEnter.Success.Equals(result))
        {
            SceneMultiplayer.MultiplayerPeer = OfflinePeer;
            Log.Debug($"{nameof(MultiplayerPeer)} OfflinePeer");
            throw new Exception($"{nameof(SteamMultiPlayer)} 加入大厅异常 {result}");
        }

        Lobby = lobby;
    }

    public override void _SetMultiplayerPeer(MultiplayerPeer multiplayerPeer)
    {
        if (multiplayerPeer is OfflineMultiplayerPeer)
        {
            SceneMultiplayer.MultiplayerPeer = multiplayerPeer;
            return;
        }

        var isServer = multiplayerPeer.GetUniqueId() == SteamPeer.ServerPeerId;
        try
        {
            if (!Lobby.IsValid())
            {
                if (isServer)
                {
                    throw new Exception("请先创建一个大厅");
                }

                throw new Exception("请先加入一个大厅");
            }
            else
            {
                if (isServer && !Lobby.IsOwnedBy(SteamClient.SteamId))
                {
                    throw new Exception("大厅成员不能作为服务端");
                }

                if (!isServer && Lobby.IsOwnedBy(SteamClient.SteamId))
                {
                    throw new Exception("大厅拥有者不能作为客户端");
                }

                SceneMultiplayer.MultiplayerPeer = multiplayerPeer;
            }
        }
        catch (Exception e)
        {
            LeaveLobby();
            throw;
        }
    }

    public override MultiplayerPeer _GetMultiplayerPeer()
    {
        return SceneMultiplayer.MultiplayerPeer;
    }

    public override int[] _GetPeerIds()
        => SceneMultiplayer.GetPeers();

    public override int _GetRemoteSenderId()
        => SceneMultiplayer.GetRemoteSenderId();

    public override int _GetUniqueId()
        => SceneMultiplayer.GetUniqueId();

    /// <summary>
    /// 记录配置添加。例如，根路径（nullptr、NodePath），复制（Node、Spawner|Synchronizer），自定义
    /// </summary>
    public override Error _ObjectConfigurationAdd(GodotObject obj, Variant configuration)
    {
        if (configuration.Obj is MultiplayerSynchronizer synchronizer)
        {
            Log.Debug($"添加用于 {obj} 的同步配置。同步器：{configuration}");
        }
        else if (configuration.Obj is MultiplayerSpawner spawner)
        {
            Log.Debug($"将节点 {obj} 添加到出生列表。出生器：{configuration}");
        }

        return SceneMultiplayer.ObjectConfigurationAdd(obj, configuration);
    }

    /// <summary>
    /// 记录配置移除。例如，根路径（nullptr、NodePath），复制（Node、Spawner|Synchronizer），自定义。
    /// </summary>
    public override Error _ObjectConfigurationRemove(GodotObject obj, Variant configuration)
    {
        if (configuration.Obj is MultiplayerSynchronizer)
        {
            Log.Debug($"移除用于 {obj} 的同步配置。同步器：{configuration}");
        }
        else if (configuration.Obj is MultiplayerSpawner)
        {
            Log.Debug($"将节点 {obj} 移除到出生列表。出生器：{configuration}");
        }

        return SceneMultiplayer.ObjectConfigurationRemove(obj, configuration);
    }

    public override Error _Poll()
    {
        return SceneMultiplayer.Poll();
    }

    /// <summary>
    /// 记录正在进行的 RPC 并将其转发到默认的多人游戏。
    /// </summary>
    public override Error _Rpc(int peer, GodotObject obj, StringName method, Collections.Array args)
        => SceneMultiplayer.Rpc(peer, obj, method, args);
}