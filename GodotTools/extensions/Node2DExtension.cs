namespace Godot;

public static class Node2DExtension
{
    public static Vector2 GetMouseDirection(this Node2D node)
    {
        return (node.GetGlobalMousePosition() - node.GlobalPosition).Normalized();
    }

    public static void Rotate(this Node2D node, double radians) => node.Rotate((float)radians);
}