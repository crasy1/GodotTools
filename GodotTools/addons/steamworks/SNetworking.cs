using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks;

public partial class SNetworking : SteamComponent
{
    private static readonly Lazy<SNetworking> LazyInstance = new(() => new());
    public static SNetworking Instance => LazyInstance.Value;

    private SNetworking()
    {
    }

    
}