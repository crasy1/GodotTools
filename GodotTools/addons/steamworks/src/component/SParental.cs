using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks.src.component;

public partial class SParental : SteamComponent
{private static readonly Lazy<SParental> LazyInstance = new(() => new());
    public static SParental Instance => LazyInstance.Value;

    private SParental()
    {
    }

    
}