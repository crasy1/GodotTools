using Steamworks;

namespace Godot;

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
        var accountId = SteamClient.SteamId - 0x0110000100000000;
        var steamIdAccountId = SteamClient.SteamId.AccountId;
        // 76561197960265728
        return default;
    }

    public SteamId SteamId(uint accountId) => accountId + 0x0110000100000000UL;

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