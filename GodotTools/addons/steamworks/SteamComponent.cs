using System;
using Godot;
using GodotTools.extensions;
using GodotTools.utils;

namespace GodotTools.addons.steamworks;

public partial class SteamComponent : Node
{
    public override void _Ready()
    {
        SetProcess(false);
        SetPhysicsProcess(false);
    }
}