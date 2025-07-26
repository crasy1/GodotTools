using System;
using NAudio.Wave;

namespace Godot;

/// <summary>
/// 使用NAudio组件实时播放steamworks音频流
/// </summary>
[GlobalClass]
public partial class StreamPlayer : AudioStreamPlayer
{
    /// <summary>
    /// 采样率
    /// </summary>
    [Export(PropertyHint.Enum, "11025,22050,24000,32000,44100,48000")]
    public int SampleRate = 44100;

    /// <summary>
    /// 音量
    /// </summary>
    [Export(PropertyHint.Range, "0,1,0.05")]
    private float _volume = 1;

    public float Volume
    {
        set
        {
            _volume = Mathf.Clamp(value, 0, 1);
            if (WaveOutEvent != null)
            {
                WaveOutEvent.Volume = _volume;
            }
        }
        get => _volume;
    }

    private BufferedWaveProvider BufferedWaveProvider { set; get; }
    private WaveOutEvent WaveOutEvent { set; get; }

    public override void _Ready()
    {
        base._Ready();
        var waveFormat = new WaveFormat(SampleRate, 16, 1);
        BufferedWaveProvider = new BufferedWaveProvider(waveFormat)
        {
            // 避免内存溢出
            DiscardOnBufferOverflow = true,
            // 0.5秒缓冲区
            BufferDuration = TimeSpan.FromSeconds(0.5)
        };
        WaveOutEvent = new WaveOutEvent
        {
            // 播放延迟
            DesiredLatency = 50,
            // 越多稳定性越好但延迟越高
            NumberOfBuffers = 2,
            Volume = Volume
        };
        WaveOutEvent.PlaybackStopped += (sender, args) =>
        {
            BufferedWaveProvider.ClearBuffer();
            Log.Info($"播放停止");
        };
    }

    public void Play()
    {
        WaveOutEvent.Init(BufferedWaveProvider);
        WaveOutEvent.Play();
    }

    public new void Stop()
    {
        WaveOutEvent.Stop();
    }

    public void AddSamples(byte[] data)
    {
        BufferedWaveProvider.AddSamples(data, 0, data.Length);
    }
}