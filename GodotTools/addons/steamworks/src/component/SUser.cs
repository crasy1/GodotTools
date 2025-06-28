using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks.src.component;

public partial class SUser : SteamComponent
{
    private static readonly Lazy<SUser> LazyInstance = new(() => new());
    public static SUser Instance => LazyInstance.Value;

    private SUser()
    {
    }

     
}