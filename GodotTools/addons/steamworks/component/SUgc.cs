using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks.component;

public partial class SUgc : SteamComponent
{
    private static readonly Lazy<SUgc> LazyInstance = new(() => new());
    public static SUgc Instance => LazyInstance.Value;

    private SUgc()
    {
    }
}