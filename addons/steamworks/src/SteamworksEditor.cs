using System;

namespace Godot;

[Tool]
[SceneTree]
public partial class SteamworksEditor : Control
{
    public override void _Ready()
    {
        SteamworksUtil.InitEnvironment();
        SetEnvButton.Pressed += () => { SteamworksUtil.InitEnvironment(); };
        UserDataButton.Pressed += () => { OS.ShellOpen(OS.GetUserDataDir()); };
        AppIdSpinBox.ValueChanged += (value) => { SteamConfig.AppId = (uint)value; };
        AppIdSpinBox.Value = SteamConfig.AppId;
        DebugUI.Toggled += (value) => { SteamConfig.Debug = value; };
        DebugUI.ButtonPressed = SteamConfig.Debug;
    }


    public override void _ExitTree()
    {
    }
}