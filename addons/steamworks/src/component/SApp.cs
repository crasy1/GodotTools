using System;
using Steamworks;

namespace Godot;

public partial class SApp : SteamComponent
{
    private static readonly Lazy<SApp> LazyInstance = new(() => new());
    public static SApp Instance => LazyInstance.Value;

    private SApp()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        SteamApps.OnDlcInstalled += (appId) => { Log.Info($"{appId} DLC已安装"); };
        SteamApps.OnNewLaunchParameters += () =>
        {
            GD.Print($"启动命令:  {SteamApps.CommandLine}");
        };
    }

    public void AppInfo()
    {
        Log.Info(@$"
AppId:  {SteamConfig.AppId}
AvailableLanguages: {SteamApps.AvailableLanguages}
GameLanguage:  {SteamApps.GameLanguage}
Owner:  {SteamApps.AppOwner}
AppInstallDir:  {SteamApps.AppInstallDir()}
                 ");
    }
}