using System;
using Steamworks;

namespace Godot;

public partial class SMatchmaking : SteamComponent
{private static readonly Lazy<SMatchmaking> LazyInstance = new(() => new());
    public static SMatchmaking Instance => LazyInstance.Value;

    private SMatchmaking()
    {
    }

    
}