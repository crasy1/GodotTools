using System;
using Godot;

using Steamworks;
namespace GodotTools.addons.steamworks;

public partial class SRemotePlay : SteamComponent
{private static readonly Lazy<SRemotePlay> LazyInstance = new(() => new());
    public static SRemotePlay Instance => LazyInstance.Value;

    private SRemotePlay()
    {
    }

    
}