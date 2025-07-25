using Godot;
using System;
using Steamworks;

/// <summary>
/// 使用godot组件实时播放steamworks音频流
/// </summary>
public partial class RecordStreamPlayer : AudioStreamPlayer
{
    // 音频配置
    /// 100ms 缓冲区
    [Export(PropertyHint.Range, "0.05,1,0.05")]
    public float BufferLength { get; set; } = 0.1f;

    /// <summary>
    /// 采样率
    /// </summary>
    [Export(PropertyHint.Enum, "11025,22050,24000,32000,44100,48000")]
    public int SampleRate { get; set; } = 44100;

    /// <summary>
    /// 通道数 1=单声道, 2=立体声
    /// </summary>
    [Export(PropertyHint.Enum, "1,2")]
    public int Channels { get; set; } = 1;

    /// <summary>
    /// 位数
    /// </summary>
    [Export]
    public int Bit { get; set; } = 16;

    /// <summary>
    /// 字节
    /// </summary>
    private int Byte => Bit / B;

    /// <summary>
    /// 每帧字节数
    /// </summary>
    private int FrameByte => Byte * Channels;

    private const float Pcm16 = 32768.0f;

    /// <summary>
    /// 每位
    /// </summary>
    private const int B = 8;

    private AudioStreamGenerator AudioStreamGenerator { set; get; }

    private AudioStreamGeneratorPlayback Playback { set; get; }

    public override void _Ready()
    {
        SUser.Instance.ReceiveVoiceData += OnReceiveVoiceData;
        PlayStream();
    }

    private void OnReceiveVoiceData(ulong steamId, byte[] compressData)
    {
        if (Playing && steamId != SteamClient.SteamId)
        {
            PushData(SUser.DecompressVoice(compressData));
        }
    }

    /// <summary>
    /// 播放音频流
    /// </summary>
    public void PlayStream()
    {
        AudioStreamGenerator = new AudioStreamGenerator()
        {
            MixRate = SampleRate,
            BufferLength = 0.05f
        };
        Stream = AudioStreamGenerator;
        Play();
        Playback = GetStreamPlayback() as AudioStreamGeneratorPlayback;
    }

    /// <summary>
    /// 停止音频流
    /// </summary>
    public void StopStream()
    {
        Stop();
        Playback.ClearBuffer();
    }

    // 处理音频帧
    public void PushData(byte[] decompress)
    {
        if (Playback == null || decompress.Length == 0)
        {
            return;
        }

        // 获取可用帧数
        var framesAvailable = Playback.GetFramesAvailable();
        // 计算最大可推送帧数
        var framesToPush = Math.Min(framesAvailable, decompress.Length / FrameByte);
        if (framesToPush <= 0)
        {
            return;
        }

        // 准备音频帧缓冲区
        var frameBuffer = new Vector2[framesAvailable];
        var position = 0;
        // 填充音频数据
        for (int i = 0; i < framesToPush; i++)
        {
            float left = 0f;
            float right = 0f;

            // 读取16位PCM样本 (小端字节序)
            if (Channels == 1)
            {
                // 单声道 - 复制到左右声道
                // 读取当前位置的两个字节，通过位运算(a | b << 8)组合成一个16位的short值
                var sample = (short)(decompress[position] | (decompress[position + 1] << B));
                // 将short值转换为-1.0到1.0范围的浮点数（通过除以32768.0f）
                left = right = sample / Pcm16;
                position += FrameByte;
            }
            else if (Channels == 2)
            {
                // 立体声 - 分别读取左右声道
                var leftSample = (short)(decompress[position] | (decompress[position + 1] << B));
                var rightSample = (short)(decompress[position + 2] | (decompress[position + 3] << B));
                left = leftSample / Pcm16;
                right = rightSample / Pcm16;
                position += FrameByte;
            }

            frameBuffer[i] = new Vector2(left, right);
        }

        // 推送到音频流
        Playback.PushBuffer(frameBuffer);
    }
}