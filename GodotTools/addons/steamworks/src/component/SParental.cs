using System;
using Steamworks;

namespace Godot;

public partial class SParental : SteamComponent
{private static readonly Lazy<SParental> LazyInstance = new(() => new());
    public static SParental Instance => LazyInstance.Value;

    private SParental()
    {
    }

    
}