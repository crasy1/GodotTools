#if TOOLS
using System;

namespace Godot;

[Tool]
public partial class SteamworksPlugin : EditorPlugin
{
    private static readonly CompressedTexture2D SteamIcon =
        GD.Load<CompressedTexture2D>("res://addons/tools/steamworks/assets/steam_icon.png");

    private const string PluginName = "steamworks";
    private SteamworksEditor SteamworksEditor { set; get; }

    public override void _EnterTree()
    {
        SteamworksEditor = GD.Load<PackedScene>(SteamworksEditor.TscnFilePath).Instantiate<SteamworksEditor>();
        AddAutoloadSingleton(nameof(SteamManager), SteamManager.TscnFilePath);
        EditorInterface.Singleton.GetEditorMainScreen().AddChild(SteamworksEditor);
        _MakeVisible(false);
    }

    public override void _ExitTree()
    {
        RemoveAutoloadSingleton(nameof(SteamManager));
        EditorInterface.Singleton.GetEditorMainScreen().RemoveChild(SteamworksEditor);
        SteamworksEditor?.QueueFree();
    }

    public override void _MakeVisible(bool visible) => SteamworksEditor.Visible = visible;
    public override Texture2D _GetPluginIcon() => SteamIcon;
    public override string _GetPluginName() => PluginName;
    public override bool _HasMainScreen() => true;
}
#endif