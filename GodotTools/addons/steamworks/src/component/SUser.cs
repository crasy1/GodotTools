using System;
using Steamworks;

namespace Godot;

public partial class SUser : SteamComponent
{
    private static readonly Lazy<SUser> LazyInstance = new(() => new());
    public static SUser Instance => LazyInstance.Value;

    private SUser()
    {
    }

     
}