namespace Godot;

/// <summary>
/// 输入文档 https://docs.godotengine.org/zh-cn/4.x/tutorials/inputs/inputevent.
/// 控制器映射 https://github.com/godotengine/godot-demo-projects/tree/4.2-31d1c0c/misc/joypads
/// </summary>
[Singleton]
public partial class InputManager : Control
{
    public override void _Ready()
    {
        base._Ready();
        InputMap.LoadFromProjectSettings();
        All();
    }

    public override void _Input(InputEvent @event)
    {
    }

    public override void _UnhandledInput(InputEvent @event)
    {
    }

    public override void _GuiInput(InputEvent @event)
    {
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
    }

    public override void _ShortcutInput(InputEvent @event)
    {
    }


    /// <summary>
    /// 手动触发动作
    /// </summary>
    /// <param name="action"></param>
    /// <param name="pressed"></param>
    public static void TriggerAction(string action, bool pressed)
    {
        var ev = new InputEventAction()
        {
            Action = action,
            Pressed = pressed
        };
        Input.ParseInputEvent(ev);
    }

    public static void All()
    {
        foreach (var action in InputMap.GetActions())
        {
            Log.Info(action);
        }
    }

    public static void ResetAll()
    {
        InputMap.LoadFromProjectSettings();
    }
}