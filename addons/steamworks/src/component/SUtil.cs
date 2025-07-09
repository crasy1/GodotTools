using System;
using Steamworks;

namespace Godot;

public partial class SUtil : SteamComponent
{
    private static readonly Lazy<SUtil> LazyInstance = new(() => new());

    public static SUtil Instance => LazyInstance.Value;

    private SUtil()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        SteamUtils.OnGamepadTextInputDismissed += (submitted) =>
        {
            Log.Info($"游戏手柄文本输入 {(submitted ? "已提交" : "已取消")}");
        };
        SteamUtils.OnIpCountryChanged += () => { Log.Info($"ip地址改变，当前ip地址为 {SteamUtils.IpCountry}"); };
        SteamUtils.OnLowBatteryPower += (minutesLeft) => { Log.Info($"电量低，还剩 {minutesLeft} 分钟"); };
        SteamUtils.OnSteamShutdown += () => { Log.Info($"Steam 关闭"); };
        GetInfo();
    }

    public void GetInfo()
    {
        Log.Info($@"
----    {nameof(SteamUtils)}    ----
CurrentBatteryPower:        {SteamUtils.CurrentBatteryPower}
IpCountry:                  {SteamUtils.IpCountry}
IsSteamChinaLauncher:       {SteamUtils.IsSteamChinaLauncher}
SecondsSinceAppActive:      {SteamUtils.SecondsSinceAppActive}
SecondsSinceComputerActive: {SteamUtils.SecondsSinceComputerActive}
SteamUILanguage:            {SteamUtils.SteamUILanguage}
SteamServerTime:            {SteamUtils.SteamServerTime}
----    {nameof(SteamUtils)}    ----
");
    }
}