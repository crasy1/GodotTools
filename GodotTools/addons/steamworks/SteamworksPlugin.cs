#if TOOLS
using Godot;
using System;
using GodotTools.utils;

[Tool]
public partial class SteamworksPlugin : EditorPlugin
{
	public override void _EnterTree()
	{
		SteamworksUtil.InitEnvironment();
		Log.Info("初始化steamworks环境");
		// Initialization of the plugin goes here.
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
	}
}
#endif
