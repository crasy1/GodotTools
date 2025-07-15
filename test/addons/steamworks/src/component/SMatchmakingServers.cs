using System;
using Steamworks;

namespace Godot;

public partial class SMatchmakingServers : SteamComponent
{private static readonly Lazy<SMatchmakingServers> LazyInstance = new(() => new());
    public static SMatchmakingServers Instance => LazyInstance.Value;

    private SMatchmakingServers()
    {
    }

    public override void _Ready()
    {
        base._Ready();
    }
}