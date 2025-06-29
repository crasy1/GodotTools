using System;
using Steamworks;

namespace Godot;

public partial class SMusic : SteamComponent
{private static readonly Lazy<SMusic> LazyInstance = new(() => new());
    public static SMusic Instance => LazyInstance.Value;

    private SMusic()
    {
    }

    
}