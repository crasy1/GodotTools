using System;
using System.IO;
using Steamworks;

namespace Godot;

[Singleton]
public partial class SUser : SteamComponent
{
    public AudioEffectRecord AudioEffectRecord { set; get; }
    private AudioStreamGenerator AudioStreamGenerator { set; get; }
    private AudioStreamGeneratorPlayback AudioStreamGeneratorPlayback { set; get; }
    private float _sampleHz;
    private float _pulseHz = 440.0f; // The frequency of the sound wave.
    private double phase = 0.0;

    public override void _Ready()
    {
        base._Ready();
        SteamUser.OnClientGameServerDeny += () => { Log.Info("游戏服务器已拒绝客户端连接"); };
        SteamUser.OnDurationControl += (durationControl) => { Log.Info($"游戏时长控制"); };
        SteamUser.OnGameWebCallback += (url) => { Log.Info($"游戏网页回调 {url}"); };
        SteamUser.OnLicensesUpdated += () => { Log.Info($"已更新授权"); };
        SteamUser.OnMicroTxnAuthorizationResponse += (appId, orderId, userAuthorized) =>
        {
            Log.Info($"用户响应微事务授权请求 {appId} {orderId} {userAuthorized}");
        };
        SteamUser.OnSteamServersConnected += () => { Log.Info($"已连接到Steam服务器"); };
        SteamUser.OnSteamServersDisconnected += () => { Log.Info($"已断开Steam服务器"); };
        SteamUser.OnSteamServerConnectFailure += () => { Log.Info($"Steam服务器连接失败"); };
        SteamUser.OnValidateAuthTicketResponse += (steamId, steamId2, authResponse) =>
        {
            Log.Info($"用户验证授权 {steamId} {steamId2} {authResponse}");
        };
        SClient.Instance.SteamClientConnected += () =>
        {
            SetProcess(true);
            // SteamUser.SampleRate = 44100;
            SteamUser.SampleRate = SteamUser.OptimalSampleRate;
            Log.Info($"设置音频采样率 {SteamUser.SampleRate}");
            AudioStreamGenerator = new AudioStreamGenerator();
            var audioStreamPlayer = new AudioStreamPlayer();
            AddChild(audioStreamPlayer);
            audioStreamPlayer.Stream = AudioStreamGenerator;
            AudioStreamGeneratorPlayback = (AudioStreamGeneratorPlayback)audioStreamPlayer.GetStreamPlayback();
            AudioStreamGenerator.MixRate = SteamUser.SampleRate;
            _sampleHz = SteamUser.SampleRate;
        };
        var idx = AudioServer.GetBusIndex("Record");
        AudioEffectRecord = (AudioEffectRecord)AudioServer.GetBusEffect(idx, 0);
    }

    public void FillBuffer()
    {
        float increment = _pulseHz / _sampleHz;
        int framesAvailable = AudioStreamGeneratorPlayback.GetFramesAvailable();

        for (int i = 0; i < framesAvailable; i++)
        {
            AudioStreamGeneratorPlayback.PushFrame(Vector2.One * (float)Mathf.Sin(phase * Mathf.Tau));
            phase = Mathf.PosMod(phase + increment, 1.0);
        }
    }


    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed(Actions.Record))
        {
            SteamUser.VoiceRecord = true;
            Log.Info($"录音开始");
        }

        if (Input.IsActionJustReleased(Actions.Record))
        {
            SteamUser.VoiceRecord = false;
            Log.Info($"录音结束");
            if (SteamUser.HasVoiceData)
            {
                var voiceData = SteamUser.ReadVoiceDataBytes();
                Log.Info($"录音数据大小:{voiceData.Length}");
                var fileAccess = FileAccess.Open("user://steam_record.wav", FileAccess.ModeFlags.Write);
                fileAccess.StoreBuffer(voiceData);
                fileAccess.Close();
            }
        }
    }

    public override void _Process(double delta)
    {
        if (SteamUser.HasVoiceData)
        {
            //     // SteamUser.ReadVoiceData()
            //     // SteamUser.DecompressVoice()
            //     var voiceData = SteamUser.ReadVoiceDataBytes();
        }
    }

    public void GetInfo()
    {
        Log.Info($@"
----    {nameof(SteamUser)}    ----
SteamLevel:                         {SteamUser.SteamLevel}
SampleRate:                         {SteamUser.SampleRate}
IsBehindNAT:                        {SteamUser.IsBehindNAT}
IsPhoneIdentifying:                 {SteamUser.IsPhoneIdentifying}
IsPhoneVerified:                    {SteamUser.IsPhoneVerified}
IsPhoneRequiringVerification:       {SteamUser.IsPhoneRequiringVerification}
IsTwoFactorEnabled:                 {SteamUser.IsTwoFactorEnabled}
----    {nameof(SteamUser)}    ----
");
    }
}