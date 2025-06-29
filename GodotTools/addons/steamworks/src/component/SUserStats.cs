using System;
using Steamworks;

namespace Godot;

public partial class SUserStats : SteamComponent
{private static readonly Lazy<SUserStats> LazyInstance = new(() => new());
    public static SUserStats Instance => LazyInstance.Value;

    private SUserStats()
    {
    }

    public override void _Ready()
    {
        base._Ready();
    }
    
}