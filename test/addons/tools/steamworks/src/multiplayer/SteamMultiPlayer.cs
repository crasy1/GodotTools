namespace Godot;

/// <summary>
/// 扩展steam多人游戏 TODO 和steam lobby结合，将lobby的逻辑从peer移到这里
/// </summary>
public partial class SteamMultiPlayer : MultiplayerApiExtension
{
    private SceneMultiplayer SceneMultiplayer = new();

    public SteamMultiPlayer()
    {
        SceneMultiplayer.PeerConnected += EmitSignalPeerConnected;
        SceneMultiplayer.PeerDisconnected += EmitSignalPeerDisconnected;
        SceneMultiplayer.ServerDisconnected += EmitSignalServerDisconnected;
        SceneMultiplayer.ConnectedToServer += EmitSignalConnectedToServer;
        SceneMultiplayer.ConnectionFailed += EmitSignalConnectionFailed;
    }

    public override void _SetMultiplayerPeer(MultiplayerPeer multiplayerPeer)
        => SceneMultiplayer.SetMultiplayerPeer(multiplayerPeer);

    public override MultiplayerPeer _GetMultiplayerPeer()
        => SceneMultiplayer.GetMultiplayerPeer();

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
        if (configuration.Obj is MultiplayerSynchronizer)
        {
            Log.Debug($"添加用于 {obj} 的同步配置。同步器：{configuration}");
        }
        else if (configuration.Obj is MultiplayerSpawner)
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

    public override Error _Poll() => SceneMultiplayer.Poll();

    /// <summary>
    /// 记录正在进行的 RPC 并将其转发到默认的多人游戏。
    /// </summary>
    public override Error _Rpc(int peer, GodotObject obj, StringName method, Collections.Array args)
        => SceneMultiplayer.Rpc(peer, obj, method, args);
}