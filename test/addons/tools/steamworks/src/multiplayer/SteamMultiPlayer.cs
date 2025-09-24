using System;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 扩展steam多人游戏 TODO 和steam lobby结合，将lobby的逻辑从peer移到这里
/// </summary>
public partial class SteamMultiPlayer : MultiplayerApiExtension
{
    private SceneMultiplayer SceneMultiplayer = new();
    private Lobby? Lobby { set; get; }

    public SteamMultiPlayer()
    {
        Dispose();
        SceneMultiplayer.PeerConnected += EmitSignalPeerConnected;
        SceneMultiplayer.PeerDisconnected += EmitSignalPeerDisconnected;
        SceneMultiplayer.ServerDisconnected += EmitSignalServerDisconnected;
        SceneMultiplayer.ConnectedToServer += EmitSignalConnectedToServer;
        SceneMultiplayer.ConnectionFailed += EmitSignalConnectionFailed;

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
    }

    private void OnLobbyLeaved(ulong lobbyId)
    {
        if (MultiplayerPeer is null)
        {
        }
    }

    private void OnLobbyCreated(int result, ulong lobbyId)
    {
    }

    private bool IsMultiplayerPeerValid(MultiplayerPeer multiplayerPeer)
    {
        return multiplayerPeer is not null && multiplayerPeer is not OfflineMultiplayerPeer;
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
            SceneMultiplayer.SetMultiplayerPeer(new OfflineMultiplayerPeer());
            throw new Exception($"{nameof(SteamMultiPlayer)} 创建大厅异常");
        }
    }

    public async Task JoinLobbyAsync(Friend friend)
    {
        var lobby = friend.GameInfo?.Lobby;
        if (!lobby.HasValue)
        {
            throw new Exception($"{nameof(SteamMultiPlayer)} 未找到大厅");
        }

        await JoinLobbyAsync(lobby.Value);
    }

    public async Task JoinLobbyAsync(Lobby lobby)
    {
        var result = await SMatchmaking.JoinLobbyAsync(lobby);
        if (!result)
        {
            SceneMultiplayer.SetMultiplayerPeer(new OfflineMultiplayerPeer());
            throw new Exception($"{nameof(SteamMultiPlayer)} 加入大厅异常");
        }
    }

    public override void _SetMultiplayerPeer(MultiplayerPeer multiplayerPeer)
    {
        SceneMultiplayer.SetMultiplayerPeer(multiplayerPeer);
    }

    public override MultiplayerPeer _GetMultiplayerPeer()
    {
        return SceneMultiplayer.GetMultiplayerPeer();
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