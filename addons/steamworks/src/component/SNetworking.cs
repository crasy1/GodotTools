using System;
using Steamworks;

namespace Godot;

public partial class SNetworking : SteamComponent
{
    private static readonly Lazy<SNetworking> LazyInstance = new(() => new());
    public static SNetworking Instance => LazyInstance.Value;

    private SNetworking()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        SteamNetworking.OnP2PSessionRequest += (steamId) => { Log.Info($"p2p连接请求 {steamId}"); };
        SteamNetworking.OnP2PConnectionFailed += (steamId, error) => { Log.Info($"p2p连接失败 {steamId},{error}"); };
    }
}
