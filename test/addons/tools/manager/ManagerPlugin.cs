namespace Godot;

[Tool]
public partial class ManagerPlugin : EditorPlugin
{
    private const string PluginName = "manager";


    public override void _EnterTree()
    {
        AddAutoloadSingleton(nameof(SceneManager), SceneManager.TscnFilePath);
    }

    public override void _ExitTree()
    {
        RemoveAutoloadSingleton(nameof(SceneManager));
    }

    public override string _GetPluginName() => PluginName;
}