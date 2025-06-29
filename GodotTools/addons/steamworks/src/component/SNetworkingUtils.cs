using System;
using Steamworks;

namespace Godot;

public partial class SNetworkingUtils : SteamComponent
{private static readonly Lazy<SNetworkingUtils> LazyInstance = new(() => new());
    public static SNetworkingUtils Instance => LazyInstance.Value;

    private SNetworkingUtils()
    {
    }

    
}