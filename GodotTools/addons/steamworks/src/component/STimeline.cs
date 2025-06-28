using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks.src.component;

public partial class STimeline : SteamComponent
{private static readonly Lazy<STimeline> LazyInstance = new(() => new());
    public static STimeline Instance => LazyInstance.Value;

    private STimeline()
    {
    }

    
}