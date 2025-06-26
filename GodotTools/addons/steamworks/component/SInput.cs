using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks.component;

public partial class SInput:SteamComponent
{
    private static readonly Lazy<SInput> LazyInstance = new(() => new());
    public static SInput Instance => LazyInstance.Value;

    private SInput()
    {
    }

    
}