using System;
using Steamworks;

namespace Godot;

public partial class SServerStats : SteamComponent
{
    private static readonly Lazy<SServerStats> LazyInstance = new(() => new());
    public static SServerStats Instance => LazyInstance.Value;

    private SServerStats()
    {
    }

    public override void _Ready()
    {
        base._Ready();
    }
    
}