using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks.component;

public partial class SUtil : SteamComponent
{
    private static readonly Lazy<SUtil> LazyInstance = new(() => new());

    public static SUtil Instance => LazyInstance.Value;

    private SUtil()
    {
    }

    
}