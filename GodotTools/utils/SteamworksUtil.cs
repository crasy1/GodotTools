using System.Reflection;
using Godot;
using Steamworks;

namespace GodotTools.utils;

/// <summary>
/// steamworks工具类
/// </summary>
public static class SteamworksUtil
{
    private const string Path = "PATH";
    private const string AssemblyLib = "GodotTools.lib";

    /// <summary>
    /// 
    /// 初始化steam api 环境
    /// 会自动将steam api文件添加到环境变量中
    /// 
    /// </summary>
    /// <param name="appId"></param>
    /// <returns>初始化是否成功</returns>
    public static bool InitEnvironment()
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

            return true;
        }
        catch (Exception e)
        {
            Log.Error("初始化steam api失败", e.Message);
            return false;
        }
    }
}