using System;
using System.IO;
using Steamworks;

namespace Godot;

[Singleton]
public partial class SUser : SteamComponent
{
    /// <summary>
    /// 收到用户steamworks语音压缩数据
    /// </summary>
    [Signal]
    public delegate void ReceiveVoiceDataEventHandler(ulong steamId, byte[] compressData);

    private NVoiceStreamPlayer StreamPlayer { set; get; }

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
            SetProcessMode(ProcessModeEnum.Always);
            SetProcess(true);
            // SteamUser.SampleRate = SteamUser.OptimalSampleRate;
            SteamUser.SampleRate = (uint)SteamConfig.SampleRate;
            Log.Info($"设置音频采样率 {SteamUser.SampleRate}");
        };
        var min = Mathf.LinearToDb(0.05);
        var max = Mathf.LinearToDb(1);
        Log.Info($" min {min} max {max}");
    }


    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed(Actions.Record))
        {
            StartRecord();
        }

        if (Input.IsActionJustReleased(Actions.Record))
        {
            StopRecord();
        }
    }

    public void StartRecord()
    {
        SteamUser.VoiceRecord = true;
        Log.Info($"录音开始");
    }

    public void StopRecord()
    {
        SteamUser.VoiceRecord = false;
        Log.Info($"录音结束");
    }


    public override void _Process(double delta)
    {
        if (SteamUser.HasVoiceData)
        {
            using var memoryStream = new MemoryStream();
            var length = SteamUser.ReadVoiceData(memoryStream);
            EmitSignalReceiveVoiceData(SteamClient.SteamId, memoryStream.GetBuffer());
        }
    }

    /// <summary>
    /// 解压声音数组
    /// </summary>
    /// <param name="from"></param>
    /// <param name="length"></param>
    /// <returns>输出数据是原始单通道 16 位 PCM 音频</returns>
    public static unsafe byte[] DecompressVoice(byte[] from, int length = ushort.MaxValue)
    {
        var buffer = new byte[length];
        int writtenCount;
        fixed (byte* pCompressed = from)
        {
            fixed (byte* pDestBuffer = buffer)
            {
                writtenCount = SteamUser.DecompressVoice((IntPtr)pCompressed, from.Length, (IntPtr)pDestBuffer,
                    buffer.Length);
            }
        }

        return buffer[..writtenCount];
    }

    /// <summary>
    /// 读取并解压steamworks声音数据
    /// </summary>
    /// <returns></returns>
    public static byte[] ReadDecompressVoice()
    {
        using var memoryStream = new MemoryStream();
        SteamUser.ReadVoiceData(memoryStream);
        return DecompressVoice(memoryStream.GetBuffer());
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