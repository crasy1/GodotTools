using System;
using Steamworks;

namespace Godot;

public partial class SScreenshots : SteamComponent
{
    private static readonly Lazy<SScreenshots> LazyInstance = new(() => new());
    public static SScreenshots Instance => LazyInstance.Value;

    private SScreenshots()
    {
    }

    
}