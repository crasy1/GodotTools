using Godot;

namespace GodotTools.extensions;

public static class NodeExtensions
{
    public static void ClearAndFreeChildren(this Node node)
    {
        foreach (var child in node.GetChildren())
        {
            node.RemoveChild(child);
            child.QueueFree();
        }
    }
}