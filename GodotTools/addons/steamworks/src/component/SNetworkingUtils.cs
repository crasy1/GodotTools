using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks.src.component;

public partial class SNetworkingUtils : SteamComponent
{private static readonly Lazy<SNetworkingUtils> LazyInstance = new(() => new());
    public static SNetworkingUtils Instance => LazyInstance.Value;

    private SNetworkingUtils()
    {
    }

    
}