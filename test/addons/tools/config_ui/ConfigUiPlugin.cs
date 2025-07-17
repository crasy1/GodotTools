#if TOOLS
using System;

namespace Godot;

[Tool]
public partial class ConfigUiPlugin : EditorPlugin
{
    private const string PluginName = "config_ui";


    public override void _EnterTree()
    {
        AddAutoloadSingleton(nameof(ConfigUi), ConfigUi.TscnFilePath);
    }

    public override void _ExitTree()
    {
        RemoveAutoloadSingleton(nameof(ConfigUi));
    }

    public override string _GetPluginName() => PluginName;
}
#endif