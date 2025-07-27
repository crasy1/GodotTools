using System.Collections.Generic;
using Steamworks;

namespace Godot;

[Singleton]
public partial class TeamVoice : Node
{
    /// <summary>
    /// 队伍成员
    /// </summary>
    private readonly Dictionary<SteamId, VoiceStreamPlayer> _teamMembers = new();

    public override void _Ready()
    {
        Name = GetType().Name;
        SetProcessMode(ProcessModeEnum.Always);
        SetProcess(false);
        SetPhysicsProcess(false);
        SNetworking.Instance.ReceiveVoice += OnReceiveVoice;
        SUser.Instance.RecordVoiceData += OnRecordVoiceData;
    }

    /// <summary>
    /// 获取该用户的播放器
    /// </summary>
    /// <param name="steamId"></param>
    /// <returns></returns>
    public VoiceStreamPlayer GetTeamMember(SteamId steamId)
    {
        return _teamMembers.GetValueOrDefault(steamId);
    }

    /// <summary>
    /// 取消用户静音
    /// </summary>
    /// <param name="steamId"></param>
    public void Play(SteamId steamId)
    {
        if (!IsPlaying(steamId))
        {
            GetTeamMember(steamId)?.Play();
            Log.Info($"队伍语音取消静音 {steamId}");
        }
    }

    /// <summary>
    /// 静音用户
    /// </summary>
    /// <param name="steamId"></param>
    public void Mute(SteamId steamId)
    {
        if (IsPlaying(steamId))
        {
            GetTeamMember(steamId)?.Stop();
            Log.Info($"队伍语音静音 {steamId}");
        }
    }

    /// <summary>
    /// 用户是否在播放
    /// </summary>
    /// <param name="steamId"></param>
    /// <returns></returns>
    public bool IsPlaying(SteamId steamId)
    {
        return GetTeamMember(steamId)?.IsPlaying() ?? false;
    }

    /// <summary>
    /// 移除成员和播放器
    /// </summary>
    /// <param name="steamId"></param>
    public void RemoveTeamMember(SteamId steamId)
    {
        GetTeamMember(steamId)?.RemoveAndQueueFree();
        _teamMembers.Remove(steamId);
        Log.Info($"队伍语音移除 {steamId}");
    }

    /// <summary>
    /// 移除所有成员和播放器
    /// </summary>
    /// <param name="steamId"></param>
    public void RemoveAllTeamMember()
    {
        foreach (var steamId in _teamMembers.Keys)
        {
            GetTeamMember(steamId)?.RemoveAndQueueFree();
            _teamMembers.Remove(steamId);
        }

        Log.Info($"退出队伍语音");
    }

    /// <summary>
    /// 添加成员和播放器
    /// </summary>
    /// <param name="steamId"></param>
    public void AddTeamMember(SteamId steamId)
    {
        if (steamId != SteamClient.SteamId && _teamMembers.ContainsKey(steamId))
        {
            return;
        }

        var voiceStreamPlayer = new VoiceStreamPlayer();
        voiceStreamPlayer.Name = steamId.ToString();
        AddChild(voiceStreamPlayer);
        _teamMembers.TryAdd(steamId, voiceStreamPlayer);
        voiceStreamPlayer.Play();
        Log.Info($"队伍语音添加 {steamId}");
    }


    // 录音并发送给所有玩家
    private void OnRecordVoiceData(ulong steamId, byte[] compressData)
    {
        foreach (var memberId in _teamMembers.Keys)
        {
            SNetworking.Instance.SendP2P(memberId, compressData, Channel.Voice, P2PSend.UnreliableNoDelay);
        }
    }


    // 接收其他玩家录音,根据id使用不同的播放器播放
    private void OnReceiveVoice(ulong steamId, byte[] data)
    {
        if (_teamMembers.TryGetValue(steamId, out var voiceStreamPlayer))
        {
            Log.Info($"收到语音数据 steamId:{steamId} , 数据长度:{data.Length}");
            voiceStreamPlayer?.ReceiveRecordVoiceData(steamId, data);
        }
    }
}