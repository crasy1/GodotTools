using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks.component;

public partial class SUserStats : SteamComponent
{private static readonly Lazy<SUserStats> LazyInstance = new(() => new());
    public static SUserStats Instance => LazyInstance.Value;

    private SUserStats()
    {
    }

    
}