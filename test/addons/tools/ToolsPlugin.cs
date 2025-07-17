#if TOOLS
using System;
using System.Collections.Generic;
using System.Linq;

namespace Godot;

[Tool]
public partial class ToolsPlugin : EditorPlugin
{
    private const string PluginDir = "res://addons";
    private const string PluginName = "tools";
    private readonly List<string> SubPlugins = new();

    public override void _EnablePlugin()
    {
        var directories = DirAccess.GetDirectoriesAt($"{PluginDir}/{PluginName}");
        foreach (var directory in directories)
        {
            var subPluginDir = $"{PluginName}/{directory}";
            if (FileAccess.FileExists($"{PluginDir}/{subPluginDir}/plugin.cfg"))
            {
                EditorInterface.Singleton.SetPluginEnabled(subPluginDir, true);
                SubPlugins.Add(subPluginDir);
            }
        }

        GD.Print($"开启插件：{string.Join(",", SubPlugins)}");
    }

    public override void _DisablePlugin()
    {
        foreach (var subPlugin in SubPlugins)
        {
            EditorInterface.Singleton.SetPluginEnabled(subPlugin, false);
        }

        GD.Print($"关闭插件：{string.Join(",", SubPlugins)}");

        SubPlugins.Clear();
    }
}
#endif