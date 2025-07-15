using System;
using Steamworks;

namespace Godot;

public partial class SInventory: SteamComponent
{private static readonly Lazy<SInventory> LazyInstance = new(() => new());
    public static SInventory Instance => LazyInstance.Value;

    private SInventory()
    {
    }

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