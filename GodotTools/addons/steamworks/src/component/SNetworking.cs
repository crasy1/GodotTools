using System;
using Steamworks;

namespace Godot;

public partial class SNetworking : SteamComponent
{
    private static readonly Lazy<SNetworking> LazyInstance = new(() => new());
    public static SNetworking Instance => LazyInstance.Value;

    private SNetworking()
    {
    }

    
}