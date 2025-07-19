using System;
using Steamworks;

namespace Godot;

[Singleton]
public partial class SScreenshots : SteamComponent
{

    public override void _Ready()
    {
        base._Ready();
        SteamScreenshots.OnScreenshotReady += (screenshot) => { Log.Info($"准备截图 {screenshot}"); };
        SteamScreenshots.OnScreenshotFailed += (result) => { Log.Info($"截图失败{result}"); };
        SteamScreenshots.OnScreenshotRequested += () => { Log.Info("请求截图"); };
        SClient.Instance.SteamClientConnected+=()=>
        {
            SteamScreenshots.Hooked = true;
        };
    }
}