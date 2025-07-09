using System;
using Steamworks;

namespace Godot;

public partial class SNetworkingUtils : SteamComponent
{
    private static readonly Lazy<SNetworkingUtils> LazyInstance = new(() => new());
    public static SNetworkingUtils Instance => LazyInstance.Value;

    private SNetworkingUtils()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        SteamNetworkingUtils.OnDebugOutput += (output, s) => { Log.Info($"接收调试网络信息 {output},{s}"); };
    }
}