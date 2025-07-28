using System.Collections.Generic;
using System.Linq;

namespace Godot;

public class InputAction(StringName action)
{
    /// <summary>
    ///   动作名
    /// </summary>
    public StringName Action => action;

    /// <summary>
    ///   判断输入动作是否按下
    /// </summary>
    /// <param name="exactMatch"></param>
    /// <returns></returns>
    public bool IsPressed(bool exactMatch = false) => Input.IsActionPressed(action, exactMatch);

    /// <summary>
    ///   判断输入动作是否刚刚按下
    /// </summary>
    /// <param name="exactMatch"></param>
    /// <returns></returns>
    public bool IsJustPressed(bool exactMatch = false) => Input.IsActionJustPressed(action, exactMatch);

    /// <summary>
    ///   判断输入动作是否刚刚释放
    /// </summary>
    /// <param name="exactMatch"></param>
    /// <returns></returns>
    public bool IsJustReleased(bool exactMatch = false) => Input.IsActionJustReleased(action, exactMatch);

    /// <summary>
    ///   获取输入动作的强度
    /// </summary>
    /// <param name="exactMatch"></param>
    /// <returns></returns>
    public float Strength(bool exactMatch = false) => Input.GetActionStrength(action, exactMatch);

    /// <summary>
    ///   按下输入动作
    /// </summary>
    /// <param name="strength"></param>
    public void Press(float strength = 1F) => Input.ActionPress(action, strength);

    /// <summary>
    ///   释放输入动作
    /// </summary>
    public void Release() => Input.ActionRelease(action);

    /// <summary>
    ///   判断输入动作是否包含指定输入事件
    /// </summary>
    /// <returns></returns>
    public List<InputEvent> Events() => InputMap.ActionGetEvents(action).ToList();

    /// <summary>
    ///   判断输入动作是否包含指定输入事件
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public bool HasEvent(InputEvent @event) => InputMap.ActionHasEvent(action, @event);

    /// <summary>
    ///   添加输入事件
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public void AddEvent(InputEvent @event)
    {
        if (!InputMap.ActionHasEvent(action, @event))
        {
            InputMap.ActionAddEvent(action, @event);
        }
    }

    /// <summary>
    ///   删掉输入事件
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public void RemoveEvent(InputEvent @event)
    {
        if (InputMap.ActionHasEvent(action, @event))
        {
            InputMap.ActionEraseEvent(action, @event);
        }
    }

    /// <summary>
    /// 移除该动作所有输入事件
    /// </summary>
    public void RemoveAllEvent() => InputMap.ActionEraseEvents(action);

    /// <summary>
    /// 设置/获取输入动作的死区
    /// </summary>
    public float Deadzone
    {
        set => InputMap.ActionSetDeadzone(action, value);
        get => InputMap.ActionGetDeadzone(action);
    }
}