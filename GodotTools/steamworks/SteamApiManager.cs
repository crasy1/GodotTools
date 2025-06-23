using System.Reflection;
using Godot;
using GodotTools.utils;
using Steamworks;
using Steamworks.Data;
using Image = Godot.Image;

namespace GodotTools.steamworks;

/// <summary>
/// 封装steam api
/// </summary>
public static class SteamApiManager
{
    private const string Path = "PATH";
    private const string AssemblyLib = "GodotTools.lib";
    public static bool IsValid => SteamClient.IsValid;

    /// <summary>
    /// 
    /// 初始化steam api
    /// 会自动将steam api文件添加到环境变量中
    /// 
    /// </summary>
    /// <param name="appId"></param>
    /// <returns>初始化是否成功</returns>
    public static bool Init(int appId)
    {
        var is64Bit = System.Environment.Is64BitOperatingSystem;
        var pathVariable = System.Environment.GetEnvironmentVariable(Path);
        System.Environment.SetEnvironmentVariable(Path, pathVariable + ";" + OS.GetUserDataDir(),
            EnvironmentVariableTarget.Process);
        string? resourceName = null;
        string? libName = null;
        var platform = OS.GetName();
        switch (platform)
        {
            case "Windows":
                libName = is64Bit ? "steam_api64.dll" : "steam_api.dll";
                resourceName = $"{AssemblyLib}.{(is64Bit ? "win64" : "win32")}.{libName}";
                break;
            case "macOS":
                libName = "libsteam_api.dylib";
                resourceName = $"{AssemblyLib}.osx.{libName}";
                break;
            case "Linux":
            case "FreeBSD":
            case "NetBSD":
            case "OpenBSD":
            case "BSD":
                libName = "libsteam_api.so";
                resourceName = $"{AssemblyLib}.{(is64Bit ? "linux64" : "linux32")}.{libName}";
                break;
            case "Android":
            case "iOS":
            case "Web":
                break;
        }

        if (resourceName == null || libName == null)
        {
            Log.Info($"{platform} 不支持 steam api");
            return false;
        }

        var assembly = Assembly.GetExecutingAssembly();
        // var resourceNames = assembly.GetManifestResourceNames();
        var libPath = System.IO.Path.Combine(OS.GetUserDataDir(), libName);
        try
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    Log.Error($"无法找到嵌入资源: {resourceName}");
                    return false;
                }

                using (var fileStream = File.Create(libPath))
                {
                    stream.CopyTo(fileStream);
                }
            }

            SteamClient.Init((uint)appId);
            return true;
        }
        catch (Exception e)
        {
            Log.Error("初始化steam api失败", e.Message);
            return false;
        }
    }

    public static void Shutdown()
    {
        SteamClient.Shutdown();
    }


    public static async Task<Image?> Avatar(SteamId steamId, int size = 0)
    {
        Steamworks.Data.Image? avatar = null;
        if (size < 0)
        {
            avatar = await SteamFriends.GetSmallAvatarAsync(steamId);
        }
        else if (size > 0)
        {
            avatar = await SteamFriends.GetLargeAvatarAsync(steamId);
        }
        else
        {
            avatar = await SteamFriends.GetMediumAvatarAsync(steamId);
        }

        if (!avatar.HasValue)
        {
            return null;
        }

        var image = avatar.Value;
        return Image.CreateFromData((int)image.Width, (int)image.Height, false, Image.Format.Rgba8, image.Data);
    }

    public static void ServerLists()
    {
        // using ( var list = new ServerList.Internet() )
        // {
        //     list.AddFilter( "map", "de_dust" );
        //     await list.RunQueryAsync();
        //
        //     foreach ( var server in list.Responsive )
        //     {
        //         Console.WriteLine( $"{server.Address} {server.Name}" );
        //     }
        // }
    }


    public static void UnlockAchievement(string achievement)
    {
        var ach = new Achievement(achievement);
        ach.Trigger();
    }


    public static void Record(bool record)
    {
        SteamUser.VoiceRecord = record;
    }

    public static void ReadVoice(Stream stream)
    {
        if (SteamUser.HasVoiceData)
        {
            var bytesrwritten = SteamUser.ReadVoiceData(stream);
            // Send Stream Data To Server or Something
        }
    }

    public static bool WriteToCloud(string filename, byte[] fileContents)
    {
        return SteamRemoteStorage.FileWrite(filename, fileContents);
    }

    public static byte[] ReadFromCloud(string filename)
    {
        return SteamRemoteStorage.FileRead(filename);
    }
}