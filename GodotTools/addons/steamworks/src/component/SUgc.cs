using System;
using Steamworks;

namespace Godot;

public partial class SUgc : SteamComponent
{
    private static readonly Lazy<SUgc> LazyInstance = new(() => new());
    public static SUgc Instance => LazyInstance.Value;

    private SUgc()
    {
    }
}