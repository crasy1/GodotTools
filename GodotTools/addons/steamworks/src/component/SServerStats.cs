using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks.src.component;

public partial class SServerStats : SteamComponent
{
    private static readonly Lazy<SServerStats> LazyInstance = new(() => new());
    public static SServerStats Instance => LazyInstance.Value;

    private SServerStats()
    {
    }

    
}