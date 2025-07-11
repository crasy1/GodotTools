using System;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public partial class SUserStats : SteamComponent
{
    private static readonly Lazy<SUserStats> LazyInstance = new(() => new());
    public static SUserStats Instance => LazyInstance.Value;

    private SUserStats()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        SteamUserStats.OnAchievementProgress += (achievement, cu, max) =>
        {
            Log.Info( $"{achievement} 进度： {cu}/{max}");
        };
        SteamUserStats.OnUserStatsStored += (result) =>
        {
            Log.Info($"保存用户统计信息 {result}");
        };
        SteamUserStats.OnUserStatsReceived += (steamId, result) =>
        {
            Log.Info( $"获取用户统计信息 {steamId} {result}");
        };
        SteamUserStats.OnUserStatsUnloaded += (steamId) =>
        {
            Log.Info( $"用户统计信息已卸载 {steamId}");
        };
    }
}