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

    [Signal]
    public delegate void SteamClientConnectedEventHandler();

    [Signal]
    public delegate void SteamClientDisconnectedEventHandler();

    public override void _Ready()
    {
        base._Ready();
        SteamClientConnected += () =>
        {
            SetProcess(true);
            Log.Info("steam client 连接成功");
        };
        SteamClientDisconnected += () =>
        {
            SetProcess(false);
            Log.Info("steam client 断开连接");
        };
    }

    public void Connect()
    {
        var appId = SteamConfig.AppId;
        try
        {
            if (OS.IsDebugBuild())
            {
                SteamClient.Init(appId);
                EmitSignalSteamClientConnected();
            }
            else
            {
                // 通过steam启动游戏
                if (!SteamClient.RestartAppIfNecessary(appId))
                {
                    SteamClient.Init(appId);
                    EmitSignalSteamClientConnected();
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"steam client 初始化错误: {e.Message}");
            EmitSignalSteamClientDisconnected();
        }
    }

    public void Disconnect()
    {
        SteamClient.Shutdown();
        EmitSignalSteamClientDisconnected();
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
            Disconnect();
        }
    }
}