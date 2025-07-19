using System;
using Steamworks;

namespace Godot;

[Singleton]
public partial class SInventory: SteamComponent
{
    public override void _Ready()
    {
        base._Ready();
        SteamInventory.OnInventoryUpdated+= (result) =>
        {
            Log.Info($"库存更新 {result}");
        };
        SteamInventory.OnDefinitionsUpdated+= () =>
        {
            Log.Info($"定义更新");
        };
    }
}