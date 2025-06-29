using System;
using Steamworks;

namespace Godot;

public partial class SParties : SteamComponent
{
    private static readonly Lazy<SParties> LazyInstance = new(() => new());
    public static SParties Instance => LazyInstance.Value;

    private SParties()
    {
    }

    
}