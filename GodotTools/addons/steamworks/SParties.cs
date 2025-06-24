using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks;

public partial class SParties : SteamComponent
{
    private static readonly Lazy<SParties> LazyInstance = new(() => new());
    public static SParties Instance => LazyInstance.Value;

    private SParties()
    {
    }

    
}