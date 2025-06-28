using Godot;
using System;
using GodotTools.addons.steamworks.src;
using GodotTools.utils;

[Tool]
public partial class SteamworksEditor : Control
{
    private SpinBox AppIdSpinBox => GetNode<SpinBox>("%AppIdSpinBox");
    private Button SetEnvButton => GetNode<Button>("%SetEnvButton");
    private Button UserDataButton => GetNode<Button>("%UserDataButton");


    public override void _Ready()
    {
        SteamworksUtil.InitEnvironment();
        SetEnvButton.Pressed += () => { SteamworksUtil.InitEnvironment(); };
        UserDataButton.Pressed += () => { OS.ShellOpen(OS.GetUserDataDir()); };
        AppIdSpinBox.ValueChanged += (value) => { SteamManager.SaveAppId((uint)value); };
        AppIdSpinBox.Value = SteamManager.GetAppId();
    }


    public override void _ExitTree()
    {
    }
}