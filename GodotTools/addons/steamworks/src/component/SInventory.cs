using System;
using Steamworks;

namespace Godot;

public partial class SInventory: SteamComponent
{private static readonly Lazy<SInventory> LazyInstance = new(() => new());
    public static SInventory Instance => LazyInstance.Value;

    private SInventory()
    {
    }

    
}