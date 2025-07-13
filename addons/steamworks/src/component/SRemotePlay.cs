using System;
using Steamworks;

namespace Godot;

public partial class SRemotePlay : SteamComponent
{
    private static readonly Lazy<SRemotePlay> LazyInstance = new(() => new());
    public static SRemotePlay Instance => LazyInstance.Value;

    private SRemotePlay()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        SteamRemotePlay.OnSessionConnected += session => { Log.Info($"steam远程同乐 已连接 {session}"); };
        SteamRemotePlay.OnSessionDisconnected += session => { Log.Info($"steam远程同乐 断开连接 {session}"); };
    }

    public static void Invite(SteamId steamId)
    {
        SteamRemotePlay.SendInvite(steamId);
    }
}