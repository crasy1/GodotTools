using System;

namespace Godot;

[Tool]
[SceneTree]
public partial class SteamworksEditor : Control
{
    public override void _Ready()
    {
        SteamUtil.InitEnvironment();
        SetEnvButton.Pressed += () => { SteamUtil.InitEnvironment(); };
        UserDataButton.Pressed += () => { OS.ShellOpen(OS.GetUserDataDir()); };
        AppIdSpinBox.ValueChanged += (value) => { SteamConfig.AppId = (uint)value; };
        AppIdSpinBox.Value = SteamConfig.AppId;
        DebugUI.Toggled += (value) => { SteamConfig.Debug = value; };
        DebugUI.ButtonPressed = SteamConfig.Debug;
        AsServer.Toggled += (value) => { SteamConfig.AsServer = value; };
        AsServer.ButtonPressed = SteamConfig.AsServer;
    }

    
    public override void _ExitTree()
    {
    }
}