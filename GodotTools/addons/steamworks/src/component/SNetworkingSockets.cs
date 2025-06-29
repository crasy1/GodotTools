using System;
using Steamworks;

namespace Godot;

public partial class SNetworkingSockets : SteamComponent
{private static readonly Lazy<SNetworkingSockets> LazyInstance = new(() => new());
    public static SNetworkingSockets Instance => LazyInstance.Value;

    private SNetworkingSockets()
    {
    }

    
}