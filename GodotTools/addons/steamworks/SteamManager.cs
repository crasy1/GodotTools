using System;
using System.Threading.Tasks;
using Godot;
using GodotTools.extensions;
using GodotTools.utils;
using Steamworks;
using  GodotTools.addons.steamworks.component;
namespace GodotTools.addons.steamworks;

/// <summary>
/// 封装steam api到godot
/// <seealso href="https://partner.steamgames.com/doc/features">steamworks文档</seealso>
/// </summary>
public partial class SteamManager : Control
{
    private static readonly Lazy<SteamManager> LazyInstance = new(() =>
        GD.Load<PackedScene>("res://addons/steamworks/SteamManager.tscn").Instantiate<SteamManager>());

    public static SteamManager Instance => LazyInstance.Value;

    private SteamManager()
    {
    }

    [Export] public uint AppId { get; set; } = 480;

    /// <summary>
    /// 添加 SteamManager到全局
    /// </summary>
    /// <param name="tree"></param>
    public async void AddToSceneTree(SceneTree tree)
    {
        tree.Root.AddChild(Instance);
        tree.Root.CallDeferred(Node.MethodName.AddChild, Instance);
        await tree.ToSignal(Instance, Node.SignalName.Ready);
    }

    public override void _Ready()
    {
        SetProcess(false);
        SetPhysicsProcess(false);
        AddChild(SApp.Instance);
        AddChild(SClient.Instance);
        AddChild(SFriends.Instance);
        AddChild(SInput.Instance);
        AddChild(SInventory.Instance);
        AddChild(SMatchmaking.Instance);
        AddChild(SMatchmakingServers.Instance);
        AddChild(SMusic.Instance);
        AddChild(SNetworking.Instance);
        AddChild(SNetworkingSockets.Instance);
        AddChild(SNetworkingUtils.Instance);
        AddChild(SParental.Instance);
        AddChild(SParties.Instance);
        AddChild(SRemotePlay.Instance);
        AddChild(SRemoteStorage.Instance);
        AddChild(SScreenshots.Instance);
        AddChild(SServer.Instance);
        AddChild(SServerStats.Instance);
        AddChild(STimeline.Instance);
        AddChild(SUgc.Instance);
        AddChild(SUser.Instance);
        AddChild(SUserStats.Instance);
        AddChild(SUtil.Instance);
        AddChild(SVideo.Instance);
    }
}