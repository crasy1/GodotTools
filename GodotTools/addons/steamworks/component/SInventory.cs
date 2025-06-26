using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks.component;

public partial class SInventory: SteamComponent
{private static readonly Lazy<SInventory> LazyInstance = new(() => new());
    public static SInventory Instance => LazyInstance.Value;

    private SInventory()
    {
    }

    
}