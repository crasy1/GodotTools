using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks;

public partial class SMatchmaking : SteamComponent
{private static readonly Lazy<SMatchmaking> LazyInstance = new(() => new());
    public static SMatchmaking Instance => LazyInstance.Value;

    private SMatchmaking()
    {
    }

    
}