using System;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace Godot;

[Singleton]
public partial class SServerStats : SteamComponent
{
    public async override void _Ready()
    {
        base._Ready();

    }
}