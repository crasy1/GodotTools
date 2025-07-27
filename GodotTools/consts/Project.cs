namespace Godot;

public static class Project
{
    public static string GameName => ProjectSettings.GetSetting("application/config/name").AsString();

    public static readonly Godot.Collections.Dictionary<string, string> GameNameLocalized = ProjectSettings.GetSetting(
        "application/config/name_localized").AsGodotDictionary<string, string>();

    public static string Version => ProjectSettings.GetSetting("application/config/version").AsString();

    public static int ViewportWidth
    {
        set => ProjectSettings.GetSetting("display/window/size/viewport_width", value);
        get => ProjectSettings.GetSetting("display/window/size/viewport_width").AsInt32();
    }

    public static int ViewportHeight
    {
        set => ProjectSettings.GetSetting("display/window/size/viewport_height", value);
        get => ProjectSettings.GetSetting("display/window/size/viewport_height").AsInt32();
    }

    public static Vector2I ViewportSize => new(ViewportWidth, ViewportHeight);

    public static string BusLayout
    {
        set => ProjectSettings.SetSetting("audio/buses/default_bus_layout", value);
        get => ProjectSettings.GetSetting("audio/buses/default_bus_layout").AsString();
    }
}