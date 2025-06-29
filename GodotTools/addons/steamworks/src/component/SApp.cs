using System;
using Steamworks;

namespace Godot;

public partial class SApp : SteamComponent
{
    private static readonly Lazy<SApp> LazyInstance = new(() => new());
    public static SApp Instance => LazyInstance.Value;

    private SApp()
    {
    }

    
}