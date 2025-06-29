using System;
using Steamworks;

namespace Godot;

public partial class SUtil : SteamComponent
{
    private static readonly Lazy<SUtil> LazyInstance = new(() => new());

    public static SUtil Instance => LazyInstance.Value;

    private SUtil()
    {
    }

    
}