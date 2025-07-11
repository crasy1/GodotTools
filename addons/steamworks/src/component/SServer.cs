using System;
using Steamworks;

namespace Godot;

public partial class SServer : SteamComponent
{
    private static readonly Lazy<SServer> LazyInstance = new(() => new());
    public static SServer Instance => LazyInstance.Value;

    private SServer()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        SteamServer.OnSteamNetAuthenticationStatus += (status) => { Log.Info($"服务器网络验证 {status}"); };
        SteamServer.OnSteamServersConnected += () => { Log.Info("服务器连接成功"); };
        SteamServer.OnSteamServerConnectFailure += (result, s) => { Log.Info($"服务器连接失败 {result},{s}"); };
        SteamServer.OnSteamServersDisconnected += (result) => { Log.Info($"服务器断开连接 {result}"); };
        SteamServer.OnValidateAuthTicketResponse += (steamId1, steamId2, result) =>
        {
            Log.Info($"服务器身份验证 {steamId1},{steamId2},{result}");
        };
    }

    public void StartServer(string modDir, string desc)
    {
        var serverInit = new SteamServerInit(modDir, desc)
        {
            VersionString = ProjectSettings.GetSetting("config/version").AsString()
        };
        try
        {
            SteamServer.Init(SteamConfig.AppId, serverInit);
            Log.Info("启动steam服务器");
        }
        catch (Exception e)
        {
            Log.Error("创建steam服务器失败", e);
        }
    }

    public void StopServer()
    {
        SteamServer.Shutdown();
        Log.Info("退出steam server");
    }

    public override void _Process(double delta)
    {
        if (SteamServer.IsValid)
        {
            SteamServer.RunCallbacks();
        }
    }

    public override void _Notification(int what)
    {
        if (NotificationWMCloseRequest == what)
        {
            StopServer();
        }
    }
}