using System;
using Steamworks;

namespace Godot;

public partial class SClient : SteamComponent
{
    private static readonly Lazy<SClient> LazyInstance = new(() => new());
    public static SClient Instance => LazyInstance.Value;

    private SClient()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        var appId = SteamManager.GetAppId();
        try
        {
            if (OS.IsDebugBuild())
            {
                SteamClient.Init(appId);
            }
            else
            {
                // 通过steam启动游戏
                if (!SteamClient.RestartAppIfNecessary(appId))
                {
                    SteamClient.Init(appId);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"steam client 初始化错误: {e.Message}");
        }

        SetProcess(SteamClient.IsValid);
    }

    public override void _Process(double delta)
    {
        if (SteamClient.IsValid)
        {
            SteamClient.RunCallbacks();
        }
    }

    public override void _Notification(int what)
    {
        if (NotificationWMCloseRequest == what)
        {
            SteamClient.Shutdown();
            Log.Info("退出steam client");
        }
    }
}