using System;
using Godot;
using GodotTools.extensions;
using GodotTools.utils;

namespace GodotTools.addons.steamworks.src.component;

public partial class SteamComponent : Node
{
    public override void _Ready()
    {
        Name = GetType().Name;
        SetProcess(false);
        SetPhysicsProcess(false);
    }
}