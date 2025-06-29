namespace Godot;

public static class ControlExtension
{
    public static void CenterPivotOffset(this Control control)
    {
        control.PivotOffset = control.Size / 2f;
    }
}