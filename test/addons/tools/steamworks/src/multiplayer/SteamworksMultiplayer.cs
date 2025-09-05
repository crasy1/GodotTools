namespace Godot.multiplayer;

public partial class SteamworksMultiplayer : MultiplayerApiExtension
{
    
    /// <summary>
    /// <para>Called when the <see cref="Godot.MultiplayerApi.MultiplayerPeer"/> is retrieved.</para>
    /// </summary>
    public override MultiplayerPeer _GetMultiplayerPeer()
    {
        return default;
    }

    /// <summary>
    /// <para>Callback for <see cref="Godot.MultiplayerApi.GetPeers()"/>.</para>
    /// </summary>
    public override int[] _GetPeerIds()
    {
        return default;
    }

    /// <summary>
    /// <para>Callback for <see cref="Godot.MultiplayerApi.GetRemoteSenderId()"/>.</para>
    /// </summary>
    public override int _GetRemoteSenderId()
    {
        return default;
    }

    /// <summary>
    /// <para>Callback for <see cref="Godot.MultiplayerApi.GetUniqueId()"/>.</para>
    /// </summary>
    public override int _GetUniqueId()
    {
        return default;
    }

    /// <summary>
    /// <para>Callback for <see cref="Godot.MultiplayerApi.ObjectConfigurationAdd(GodotObject, Variant)"/>.</para>
    /// </summary>
    public override Error _ObjectConfigurationAdd(GodotObject @object, Variant configuration)
    {
        return default;
    }

    /// <summary>
    /// <para>Callback for <see cref="Godot.MultiplayerApi.ObjectConfigurationRemove(GodotObject, Variant)"/>.</para>
    /// </summary>
    public override Error _ObjectConfigurationRemove(GodotObject @object, Variant configuration)
    {
        return default;
    }

    /// <summary>
    /// <para>Callback for <see cref="Godot.MultiplayerApi.Poll()"/>.</para>
    /// </summary>
    public override Error _Poll()
    {
        return default;
    }

    /// <summary>
    /// <para>Callback for <see cref="Godot.MultiplayerApi.Rpc(int, GodotObject, StringName, Godot.Collections.Array)"/>.</para>
    /// </summary>
    public override Error _Rpc(int peer, GodotObject @object, StringName method, Godot.Collections.Array args)
    {
        return default;
    }

    /// <summary>
    /// <para>Called when the <see cref="Godot.MultiplayerApi.MultiplayerPeer"/> is set.</para>
    /// </summary>
    public override void _SetMultiplayerPeer(MultiplayerPeer multiplayerPeer)
    {
    }

}