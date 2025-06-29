using System;
using Steamworks;

namespace Godot;

public partial class SMusic : SteamComponent
{
    private static readonly Lazy<SMusic> LazyInstance = new(() => new());
    public static SMusic Instance => LazyInstance.Value;

    private SMusic()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        SteamMusic.OnVolumeChanged += (volume) =>
        {
            Log.Info( $"steam 音乐音量已改变为 {volume}");
        };
        SteamMusic.OnPlaybackChanged += () =>
        {
            Log.Info($"steam 音乐播放状态已改变");
        };
    }
}