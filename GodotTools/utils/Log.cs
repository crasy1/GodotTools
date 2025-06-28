using System.Text;
using Godot;
using Serilog;
using Serilog.Core;

namespace GodotTools.utils;

public static class Log
{
    private static Logger Logger = CreateLogger();

    private static Logger CreateLogger()
    {
        return new LoggerConfiguration().MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(ProjectSettings.GlobalizePath("user://logs/game.log"),
                rollingInterval: RollingInterval.Day,
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
        Logger.Debug(AppendPrintParams(what));
    }

    public static void Info(params object[] what)
    {
        Logger.Information(AppendPrintParams(what));
    }

    public static void Warn(params object[] what)
    {
        Logger.Warning(AppendPrintParams(what));
    }

    public static void Error(params object[] what)
    {
        Logger.Error(AppendPrintParams(what));
    }
}