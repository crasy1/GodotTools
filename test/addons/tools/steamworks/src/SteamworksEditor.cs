using System;

namespace Godot;

[Tool]
[SceneTree]
public partial class SteamworksEditor : Control
{
    public const string CustomBusLayoutPath = "res://addons/tools/steamworks/src/custom_bus_layout.tres";

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
        SampleRate.ItemSelected += (value) => { SteamConfig.SampleRate = SampleRate.GetItemText((int)value).ToInt(); };
        for (int i = 0; i < SampleRate.GetItemCount(); i++)
        {
            if (SampleRate.GetItemText(i).ToInt() == SteamConfig.SampleRate)
            {
                SampleRate.Selected = i;
            }
        }

        CustomBusLayout.Toggled += (value) =>
        {
            SteamConfig.CustomBusLayout = value;
            Project.BusLayout = value ? CustomBusLayoutPath : Consts.DefaultBusLayoutPath;
        };
        CustomBusLayout.ButtonPressed = SteamConfig.CustomBusLayout;
        Project.BusLayout = SteamConfig.CustomBusLayout ? CustomBusLayoutPath : Consts.DefaultBusLayoutPath;
    }


    public override void _ExitTree()
    {
    }
}