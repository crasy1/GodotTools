using System;
using Steamworks;
namespace Godot;

public partial class SRemotePlay : SteamComponent
{private static readonly Lazy<SRemotePlay> LazyInstance = new(() => new());
    public static SRemotePlay Instance => LazyInstance.Value;

    private SRemotePlay()
    {
    }

    
}