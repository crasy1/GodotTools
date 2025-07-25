namespace Godot;

public static class SteamUtil
{
    private const string PathEnv = "PATH";

    /// <summary>
    /// 
    /// 初始化steam api 环境
    /// 会自动将steam api文件添加到环境变量中
    /// 
    /// </summary>
    /// <returns>初始化是否成功</returns>
    public static bool InitEnvironment(string libDir="res://addons/tools/steamworks/assets/lib")
    {
        var is64Bit = System.Environment.Is64BitOperatingSystem;
        var pathVariable = System.Environment.GetEnvironmentVariable(PathEnv);
        System.Environment.SetEnvironmentVariable(PathEnv, pathVariable + ";" + OS.GetUserDataDir(),
            EnvironmentVariableTarget.Process);
        string? resourceName = null;
        string? libName = null;
        var platform = OS.GetName();
        switch (platform)
        {
            case PlatformName.Windows:
                libName = is64Bit ? "steam_api64.dll" : "steam_api.dll";
                resourceName = Path.Combine(libDir, (is64Bit ? "win64" : "win32"), libName);
                break;
            case PlatformName.MacOS:
                libName = "libsteam_api.dylib";
                resourceName = Path.Combine(libDir, "osx", libName);
                break;
            case PlatformName.Linux:
            case PlatformName.FreeBSD:
            case PlatformName.NetBSD:
            case PlatformName.OpenBSD:
            case PlatformName.BSD:
                libName = "libsteam_api.so";
                resourceName = Path.Combine(libDir, (is64Bit ? "linux64" : "linux32"), libName);
                break;
            case PlatformName.Android:
            case PlatformName.IOS:
            case PlatformName.Web:
                break;
        }

        if (resourceName == null || libName == null)
        {
            Log.Info($"{platform} 不支持 steam api");
            return false;
        }

        if (!FileAccess.FileExists(resourceName))
        {
            Log.Error($"steamworks 未找到资源文件: {resourceName}");
            return false;
        }

        try
        {
            var libPath = Path.Combine(OS.GetUserDataDir(), libName);
            var fileBytes = FileUtil.GetFileBytes(resourceName);
            FileUtil.WriteFileBytes(libPath, fileBytes);
            return FileAccess.FileExists(libPath);
        }
        catch (Exception e)
        {
            Log.Error("初始化steam api失败", e.Message);
            return false;
        }
    }

   
}