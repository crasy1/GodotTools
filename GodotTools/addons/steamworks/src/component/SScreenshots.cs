using System;
using Steamworks;

namespace Godot;

public partial class SScreenshots : SteamComponent
{
    private static readonly Lazy<SScreenshots> LazyInstance = new(() => new());
    public static SScreenshots Instance => LazyInstance.Value;

    private SScreenshots()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        SteamScreenshots.OnScreenshotReady += (screenshot) => { Log.Info($"准备截图 {screenshot}"); };
        SteamScreenshots.OnScreenshotFailed += (result) => { Log.Info($"截图失败{result}"); };
        SteamScreenshots.OnScreenshotRequested += () => { Log.Info("请求截图"); };
    }
}