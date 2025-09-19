using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Steamworks;

namespace Godot;

public static class SteamUtil
{
    private const string PathEnv = "PATH";
    private static string SteamLibPath { set; get; }

    /// <summary>
    /// 
    /// 初始化steam api 环境
    /// 会自动将steam api文件添加到环境变量中
    /// 
    /// </summary>
    /// <returns>初始化是否成功</returns>
    public static bool InitEnvironment(uint appId, string libDir = "res://addons/tools/steamworks/assets/lib")
    {
        var is64Bit = System.Environment.Is64BitOperatingSystem;
        var pathVariable = System.Environment.GetEnvironmentVariable(PathEnv);
        System.Environment.SetEnvironmentVariable(PathEnv,
            string.IsNullOrEmpty(pathVariable) ? OS.GetUserDataDir() : pathVariable + ";" + OS.GetUserDataDir(),
            EnvironmentVariableTarget.Process);
        string? resourceName = null;
        string? libName = null;
        var platform = OS.GetName();
        var result = false;
        switch (platform)
        {
            case PlatformName.Windows:
                libName = is64Bit ? "steam_api64.dll" : "steam_api.dll";
                resourceName = Path.Combine(libDir, (is64Bit ? "win64" : "win32"), libName);
                SteamLibPath = Path.Combine(OS.GetUserDataDir(), libName);
                result = CopyFile(resourceName, SteamLibPath);
                break;
            case PlatformName.MacOS:
                libName = "libsteam_api.dylib";
                resourceName = Path.Combine(libDir, "osx", libName);
                SteamLibPath = Path.Combine(OS.GetUserDataDir(), libName);
                result = CopyFile(resourceName, SteamLibPath);
                CreateSteamAppFile(appId);
                break;
            case PlatformName.Linux:
            case PlatformName.FreeBSD:
            case PlatformName.NetBSD:
            case PlatformName.OpenBSD:
            case PlatformName.BSD:
                libName = "libsteam_api.so";
                NativeLibrary.SetDllImportResolver(typeof(SteamClient).Assembly, DllImportResolver);
                resourceName = Path.Combine(libDir, (is64Bit ? "linux64" : "linux32"), libName);
                SteamLibPath = Path.Combine(OS.GetUserDataDir(), libName);
                result = CopyFile(resourceName, SteamLibPath);
                CreateSteamAppFile(appId);
                break;
            case PlatformName.Android:
            case PlatformName.IOS:
            case PlatformName.Web:
                break;
        }

        return result;
    }

    private static void CreateSteamAppFile(uint appId)
    {
        var steamAppid = Path.Combine(Path.GetDirectoryName(OS.GetExecutablePath()), "steam_appid.txt");
        try
        {
            FileUtil.WriteFileBytes(steamAppid,
                Encoding.UTF8.GetBytes(appId.ToString()));
        }
        finally
        {
            Log.Info($"appId:{appId},steam_appid.txt exists:{FileUtil.FileExists(steamAppid)}");
        }
    }

    public static bool CopyFile(string? originFile, string targetFile)
    {
        if (!FileUtil.FileExists(originFile))
        {
            Log.Error($"未找到文件: {originFile}");
            return false;
        }

        try
        {
            FileUtil.WriteFileBytes(targetFile, FileUtil.GetFileBytes(originFile));
            return FileAccess.FileExists(targetFile);
        }
        catch (Exception e)
        {
            Log.Error($"复制文件 {originFile} 失败", e.Message);
            return false;
        }
    }

    private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName is "steam_api64" or "steam_api")
        {
            return NativeLibrary.Load(SteamLibPath, assembly, searchPath);
        }

        return IntPtr.Zero;
    }
}