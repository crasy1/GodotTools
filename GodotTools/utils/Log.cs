using System.Text;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Godot;

public static class Log
{
    private static readonly LoggingLevelSwitch LevelSwitch = new();
    private static readonly string LogPath = ProjectSettings.GlobalizePath("user://logs/game.log");
    private const int MaxLogFiles = 14; // 保留的最大日志文件数量

    /// <summary>
    /// 日志文件日志等级
    /// </summary>
    public static LogEventLevel FileLogLevel
    {
        set => LevelSwitch.MinimumLevel = value;
        get => LevelSwitch.MinimumLevel;
    }

    /// <summary>
    ///  godot打印日志等级
    /// </summary>
    public static LogEventLevel GdLogLevel { set; get; } = LogEventLevel.Information;

    static Log()
    {
        // 初始化日志等级为 Debug
        Serilog.Log.Logger = new LoggerConfiguration().MinimumLevel.ControlledBy(LevelSwitch)
            .WriteTo.File(LogPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: MaxLogFiles,
                encoding: Encoding.UTF8
            )
            .CreateLogger();
    }

    private static string AppendPrintParams(object[] parameters)
    {
        if (parameters == null)
        {
            return "null";
        }

        var stringBuilder = new StringBuilder();
        for (int index = 0; index < parameters.Length; ++index)
        {
            stringBuilder.Append(parameters[index]?.ToString() ?? "null");
        }

        return stringBuilder.ToString();
    }


    public static void Debug(params object[] what)
    {
        Serilog.Log.Debug(AppendPrintParams(what));
        if (GdLogLevel <= LogEventLevel.Debug)
        {
            GD.Print(what);
        }
    }

    public static void Info(params object[] what)
    {
        Serilog.Log.Information(AppendPrintParams(what));
        if (GdLogLevel <= LogEventLevel.Information)
        {
            GD.Print(what);
        }
    }

    public static void Warn(params object[] what)
    {
        Serilog.Log.Warning(AppendPrintParams(what));
        if (GdLogLevel <= LogEventLevel.Warning)
        {
            GD.Print(what);
        }

        GD.PushWarning(what);
    }

    public static void Error(params object[] what)
    {
        Serilog.Log.Error(AppendPrintParams(what));
        if (GdLogLevel <= LogEventLevel.Error)
        {
            GD.Print(what);
        }

        GD.PushError(what);
    }
}