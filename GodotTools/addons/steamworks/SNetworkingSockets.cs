using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks;

public partial class SNetworkingSockets : SteamComponent
{private static readonly Lazy<SNetworkingSockets> LazyInstance = new(() => new());
    public static SNetworkingSockets Instance => LazyInstance.Value;

    private SNetworkingSockets()
    {
    }

    
}