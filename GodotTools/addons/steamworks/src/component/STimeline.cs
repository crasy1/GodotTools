using System;
using Steamworks;

namespace Godot;

public partial class STimeline : SteamComponent
{private static readonly Lazy<STimeline> LazyInstance = new(() => new());
    public static STimeline Instance => LazyInstance.Value;

    private STimeline()
    {
    }

    
}