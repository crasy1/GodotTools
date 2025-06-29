namespace Godot;

/// <summary>
/// 封装steam api到godot
/// <seealso href="https://partner.steamgames.com/doc/features">steamworks文档</seealso>
/// </summary>
public partial class SteamManager : Control
{
    private static readonly Lazy<SteamManager> LazyInstance = new(() =>
        GD.Load<PackedScene>("res://addons/steamworks/src/SteamManager.tscn").Instantiate<SteamManager>());

    public static SteamManager Instance => LazyInstance.Value;

    private SteamManager()
    {
    }
    
    private const string SteamworksConfigPath = "res://Steamworks.tres";
    private const string AppId = "appId";
    public static void SaveAppId(uint appId)
    {
        var config = ResourceLoader.Exists(SteamworksConfigPath)
            ? ResourceLoader.Load(SteamworksConfigPath)
            : new Resource();

        config.SetMeta(AppId, appId);
        ResourceSaver.Save(config, SteamworksConfigPath);
    }

    public static uint GetAppId()
    {
        var config = ResourceLoader.Exists(SteamworksConfigPath)
            ? ResourceLoader.Load(SteamworksConfigPath)
            : new Resource();
        if (!config.HasMeta(AppId))
        {
            config.SetMeta(AppId, 480);
            ResourceSaver.Save(config, SteamworksConfigPath);
        }

        return (uint)config.GetMeta(AppId);
    }


    public override void _Ready()
    {
        var components = new Node();
        components.Name = "SteamComponents";
        AddChild(components);
        components.AddChild(SApp.Instance);
        components.AddChild(SClient.Instance);
        components.AddChild(SFriends.Instance);
        components.AddChild(SInput.Instance);
        components.AddChild(SInventory.Instance);
        components.AddChild(SMatchmaking.Instance);
        components.AddChild(SMatchmakingServers.Instance);
        components.AddChild(SMusic.Instance);
        components.AddChild(SNetworking.Instance);
        components.AddChild(SNetworkingSockets.Instance);
        components.AddChild(SNetworkingUtils.Instance);
        components.AddChild(SParental.Instance);
        components.AddChild(SParties.Instance);
        components.AddChild(SRemotePlay.Instance);
        components.AddChild(SRemoteStorage.Instance);
        components.AddChild(SScreenshots.Instance);
        components.AddChild(SServer.Instance);
        components.AddChild(SServerStats.Instance);
        components.AddChild(STimeline.Instance);
        components.AddChild(SUgc.Instance);
        components.AddChild(SUser.Instance);
        components.AddChild(SUserStats.Instance);
        components.AddChild(SUtil.Instance);
        components.AddChild(SVideo.Instance);
    }
}