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

    public override void _Ready()
    {
        base._Ready();
        SteamParties.OnActiveBeaconsUpdated += () =>
        {
            Log.Info("steam party 活动信标列表可能已更改");
        };
        SteamParties.OnBeaconLocationsUpdated += () =>
        {
            Log.Info("steam party 信标位置列表可能发生更改");
        };
    }
}