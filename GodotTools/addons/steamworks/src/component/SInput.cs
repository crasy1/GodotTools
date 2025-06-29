using System;
using Steamworks;

namespace Godot;

public partial class SInput:SteamComponent
{
    private static readonly Lazy<SInput> LazyInstance = new(() => new());
    public static SInput Instance => LazyInstance.Value;

    private SInput()
    {
    }

    public override void _Ready()
    {
        base._Ready();
    }
}