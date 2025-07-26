namespace Godot;

using System;
using System.IO;
using System.Text;
using Steamworks;
using NAudio.Wave;

public class SteamAudio : IDisposable
{
    private readonly int _optimalSampleRate = 44100;
    private WaveOutEvent _audioOutput;
    private BufferedWaveProvider _waveProvider;
    private FileStream _audioFileStream;
    private BinaryWriter _audioWriter;
    private bool _isSaving;

    public void StartRecording() => SteamUser.VoiceRecord = true;
    public void StopRecording() => SteamUser.VoiceRecord = false;

    public void StartPlayback()
    {
        if (_audioOutput == null)
        {
            // 创建 WaveFormat (16位单声道)
            var waveFormat = WaveFormat.CreateCustomFormat(
                WaveFormatEncoding.Pcm,
                _optimalSampleRate,
                1, // 单声道
                _optimalSampleRate * 2, // 字节率 = 采样率 * 位深/8 * 通道数
                2, // 块对齐 = 位深/8 * 通道数
                16 // 位深
            );

            _waveProvider = new BufferedWaveProvider(waveFormat)
            {
                BufferDuration = TimeSpan.FromSeconds(0.5),
                DiscardOnBufferOverflow = true
            };

            _audioOutput = new WaveOutEvent();
            _audioOutput.Init(_waveProvider);
        }

        _audioOutput.Play();
    }

    public void StopPlayback() => _audioOutput?.Stop();

    public void StartSavingToFile(string filePath)
    {
        _audioFileStream = new FileStream(filePath, FileMode.Create);
        _audioWriter = new BinaryWriter(_audioFileStream);
        WriteWavHeader(_audioWriter, _optimalSampleRate);
        _isSaving = true;
    }

    public void StopSaving()
    {
        if (_isSaving)
        {
            UpdateWavHeader(_audioWriter, _audioFileStream);
            _audioWriter.Dispose();
            _audioFileStream.Dispose();
            _isSaving = false;
        }
    }

    public void ProcessAudio()
    {
        if (!SteamUser.HasVoiceData)
        {
            return;
        }

        // 检查并解压语音数据
        var dataBytes = SteamUser.ReadVoiceDataBytes();
        var memoryStream = new MemoryStream();
        var length = SteamUser.DecompressVoice(dataBytes, memoryStream);
        var decompressedBuffer = memoryStream.ToArray();

        // 处理 PCM 数据
        ProcessDecompressedAudio(decompressedBuffer, length);
    }

    private void ProcessDecompressedAudio(byte[] pcmData, int dataSize)
    {
        // 播放音频
        if (_audioOutput?.PlaybackState == PlaybackState.Playing)
            _waveProvider.AddSamples(pcmData, 0, dataSize);

        // 保存到文件
        if (_isSaving)
            _audioWriter.Write(pcmData, 0, dataSize);
    }

    private void WriteWavHeader(BinaryWriter writer, int sampleRate)
    {
        // 预留 44 字节文件头空间
        writer.Seek(44, SeekOrigin.Begin);
    }

    private void UpdateWavHeader(BinaryWriter writer, FileStream fileStream)
    {
        long fileSize = fileStream.Length;
        long dataSize = fileSize - 44;

        writer.Seek(0, SeekOrigin.Begin);
        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write((int)(fileSize - 8));
        writer.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
        writer.Write(16); // fmt 块大小
        writer.Write((short)1); // PCM 格式
        writer.Write((short)1); // 单声道
        writer.Write(_optimalSampleRate);
        writer.Write(_optimalSampleRate * 2); // 字节率
        writer.Write((short)2); // 块对齐
        writer.Write((short)16); // 位深
        writer.Write(Encoding.ASCII.GetBytes("data"));
        writer.Write((int)dataSize);
    }

    public void Dispose()
    {
        StopRecording();
        StopPlayback();
        StopSaving();
        _audioOutput?.Dispose();
    }
}