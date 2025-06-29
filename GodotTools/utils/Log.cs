using System.Text;
using Serilog;

namespace Godot;

public static class Log
{
    static Log()
    {
        Serilog.Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
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
        Serilog.Log.Debug(AppendPrintParams(what));
        GD.Print(what);
    }

    public static void Info(params object[] what)
    {
        Serilog.Log.Information(AppendPrintParams(what));
        GD.Print(what);
    }

    public static void Warn(params object[] what)
    {
        Serilog.Log.Warning(AppendPrintParams(what));
        GD.PushWarning(what);
    }

    public static void Error(params object[] what)
    {
        Serilog.Log.Error(AppendPrintParams(what));
        GD.PrintErr(what);
        GD.PushError(what);
    }
}