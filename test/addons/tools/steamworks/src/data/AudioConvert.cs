namespace Godot;

using NAudio.Wave;
using System.IO;

public class AudioConvert
{
    public static void SavePcmToWav(byte[] pcmData, string outputPath)
    {
        // 定义音频格式：44.1kHz采样率，16位，单声道
        var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
        waveFormat = new WaveFormat(44100, 16, 1); // 更精确的PCM格式指定

        using (var memoryStream = new MemoryStream(pcmData))
        using (var reader = new RawSourceWaveStream(memoryStream, waveFormat))
        using (var fileWriter = new WaveFileWriter(outputPath, reader.WaveFormat))
        {
            // 创建缓冲区（每次读取1KB）
            var buffer = new byte[1024];
            int bytesRead;

            // 从内存流读取数据并写入WAV文件
            while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileWriter.Write(buffer, 0, bytesRead);
            }
        }
    }
}