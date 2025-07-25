using System;
using Steamworks;

namespace Godot;

/// <summary>
/// https://partner.steamgames.com/doc/store/localization/languages
/// </summary>
[Singleton]
public partial class SApp : SteamComponent
{

    public override void _Ready()
    {
        base._Ready();
        SteamApps.OnDlcInstalled += (appId) => { Log.Info($"{appId} DLC已安装"); };
        SteamApps.OnNewLaunchParameters += () =>
        {
            GD.Print($"启动命令:  {SteamApps.CommandLine}");
            // connect_lobby
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