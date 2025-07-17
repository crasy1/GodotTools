using System;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public partial class SUserStats : SteamComponent
{
    private static readonly Lazy<SUserStats> LazyInstance = new(() => new());
    public static SUserStats Instance => LazyInstance.Value;

    public static event Action<Achievement> AchievementUnlocked;

    private SUserStats()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        SteamUserStats.OnAchievementProgress += (achievement, current, max) =>
        {
            if (current == 0 && max == 0)
            {
                Log.Info($"成就 {achievement} 已解锁");
                AchievementUnlocked?.Invoke(achievement);
            }
            else
            {
                Log.Info($"成就 {achievement} 进度： {current}/{max}");
            }
        };
        SteamUserStats.OnUserStatsStored += (result) => { Log.Info($"保存用户统计信息 {result}"); };
        SteamUserStats.OnUserStatsReceived += (steamId, result) => { Log.Info($"收到用户 {steamId} 统计信息 {result}"); };
        SteamUserStats.OnUserStatsUnloaded += (steamId) => { Log.Info($"用户 {steamId} 统计信息已卸载"); };
    }
}