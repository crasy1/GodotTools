namespace Godot;

public static class Project
{
    public static readonly string AddonsDir = "res://addons";
    public static readonly string GameName = ProjectSettings.GetSetting("application/config/name").AsString();

    public static readonly Godot.Collections.Dictionary<string, string> GameNameLocalized = ProjectSettings.GetSetting(
        "application/config/name_localized").AsGodotDictionary<string, string>();

    public static readonly string Version = ProjectSettings.GetSetting("application/config/version").AsString();

    public static readonly int ViewportWidth =
        ProjectSettings.GetSetting("display/window/size/viewport_width").AsInt32();

    public static readonly int ViewportHeight =
        ProjectSettings.GetSetting("display/window/size/viewport_height").AsInt32();

    public static readonly Vector2I ViewportSize = new(ViewportWidth, ViewportHeight);
}