using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks;

public partial class SApp : SteamComponent
{
    private static readonly Lazy<SApp> LazyInstance = new(() => new());
    public static SApp Instance => LazyInstance.Value;

    private SApp()
    {
    }

    
}