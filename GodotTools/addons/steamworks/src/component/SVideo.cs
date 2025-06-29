using System;
using Steamworks;

namespace Godot;

public partial class SVideo : SteamComponent
{
    private static readonly Lazy<SVideo> LazyInstance = new(() => new());
    public static SVideo Instance => LazyInstance.Value;

    private SVideo()
    {
    }

    
}