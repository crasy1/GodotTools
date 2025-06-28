using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks.src.component;

public partial class SVideo : SteamComponent
{
    private static readonly Lazy<SVideo> LazyInstance = new(() => new());
    public static SVideo Instance => LazyInstance.Value;

    private SVideo()
    {
    }

    
}