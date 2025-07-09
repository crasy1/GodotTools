using System;
using Steamworks;

namespace Godot;

public partial class SNetworkingSockets : SteamComponent
{
    private static readonly Lazy<SNetworkingSockets> LazyInstance = new(() => new());
    public static SNetworkingSockets Instance => LazyInstance.Value;

    private SNetworkingSockets()
    {
    }


    public override void _Ready()
    {
        base._Ready();
        SteamNetworkingSockets.OnConnectionStatusChanged += (c, ci) => { Log.Info($"连接状态改变 {c},{ci}"); };
        SteamNetworkingSockets.OnFakeIPResult += (na) => { Log.Info($"steam fake ip result {na}"); };
    }
}