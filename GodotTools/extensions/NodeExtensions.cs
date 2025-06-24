using Godot;

namespace GodotTools.extensions;

public static class NodeExtensions
{
    public static async Task AddToGlobal(this Node node, SceneTree tree)
    {
        tree.Root.CallDeferred(Node.MethodName.AddChild, node);
        await tree.ToSignal(node, Node.SignalName.Ready);
    }

    public static T GetGlobal<T>(this Node node, T t) where T : Node
    {
        return node.GetTree().Root.GetNode<T>(nameof(T));
    }

    public static void ClearAndFreeChildren(this Node node)
    {
        foreach (var child in node.GetChildren())
        {
            node.RemoveChild(child);
            child.QueueFree();
        }
    }
}