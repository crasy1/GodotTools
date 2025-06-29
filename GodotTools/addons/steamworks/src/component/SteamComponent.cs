using System;

namespace Godot;

public partial class SteamComponent : Node
{
    public override void _Ready()
    {
        Name = GetType().Name;
        SetProcess(false);
        SetPhysicsProcess(false);
    }
}