#if TOOLS
using System;
namespace Godot;

[Tool]
public partial class SteamworksPlugin : EditorPlugin
{
    private static readonly CompressedTexture2D SteamIcon =
        GD.Load<CompressedTexture2D>("res://addons/steamworks/assets/steam_icon.png");

    private static readonly PackedScene SteamworksEditorScene =
        GD.Load<PackedScene>("res://addons/steamworks/src/SteamworksEditor.tscn");

    private const string SteamManagerPath = "res://addons/steamworks/src/SteamManager.tscn";
    private const string PluginName = "Steamworks";
    private SteamworksEditor SteamworksEditor { set; get; }

    public override void _EnterTree()
    {
        SteamworksEditor = SteamworksEditorScene.Instantiate<SteamworksEditor>();
        AddAutoloadSingleton(nameof(SteamManager), SteamManagerPath);
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