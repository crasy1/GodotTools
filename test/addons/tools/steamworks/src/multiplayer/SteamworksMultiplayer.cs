namespace Godot.multiplayer;

public partial class SteamworksMultiplayer : MultiplayerApiExtension
{
    
    /// <summary>
    /// <para>当获取到 <see cref="Godot.MultiplayerApi.MultiplayerPeer"/> 时调用。</para>
    /// </summary>
    public override MultiplayerPeer _GetMultiplayerPeer()
    {
        return default;
    }

    /// <summary>
    /// <para><see cref="Godot.MultiplayerApi.GetPeers()"/> 的回调。</para>
    /// </summary>
    public override int[] _GetPeerIds()
    {
        return default;
    }

    /// <summary>
    /// <para><see cref="Godot.MultiplayerApi.GetRemoteSenderId()"/> 的回调。</para>
    /// </summary>
    public override int _GetRemoteSenderId()
    {
        return default;
    }

    /// <summary>
    /// <para><see cref="Godot.MultiplayerApi.GetUniqueId()"/> 的回调。</para>
    /// </summary>
    public override int _GetUniqueId()
    {
        return default;
    }

    /// <summary>
    /// <para><see cref="Godot.MultiplayerApi.ObjectConfigurationAdd(GodotObject, Variant)"/> 的回调。</para>
    /// </summary>
    public override Error _ObjectConfigurationAdd(GodotObject @object, Variant configuration)
    {
        return default;
    }

    /// <summary>
    /// <para><see cref="Godot.MultiplayerApi.ObjectConfigurationRemove(GodotObject, Variant)"/> 的回调。</para>
    /// </summary>
    public override Error _ObjectConfigurationRemove(GodotObject @object, Variant configuration)
    {
        return default;
    }

    /// <summary>
    /// <para><see cref="Godot.MultiplayerApi.Poll()"/> 的回调。</para>
    /// </summary>
    public override Error _Poll()
    {
        return default;
    }

    /// <summary>
    /// <para><see cref="Godot.MultiplayerApi.Rpc(int, GodotObject, StringName, Godot.Collections.Array)"/> 的回调。</para>
    /// </summary>
    public override Error _Rpc(int peer, GodotObject @object, StringName method, Godot.Collections.Array args)
    {
        return default;
    }

    /// <summary>
    /// <para>当设置 <see cref="Godot.MultiplayerApi.MultiplayerPeer"/> 时调用。</para>
    /// </summary>
    public override void _SetMultiplayerPeer(MultiplayerPeer multiplayerPeer)
    {
    }

}