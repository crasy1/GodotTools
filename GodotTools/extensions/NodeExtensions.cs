using Godot;
using GodotTools.utils;

namespace GodotTools.extensions;

public static class NodeExtensions
{
    public static async Task AddToAutoLoad(this Node node, SceneTree tree)
    {
        tree.Root.CallDeferred(Node.MethodName.AddChild, node);
        await tree.ToSignal(node, Node.SignalName.Ready);
    }

    public static T? GetAutoLoad<T>(this Node node, T t) where T : Node
    {
        return node.GetNode<T>($"/root/{typeof(T).Name}");
    }

    public static IList<T> GetChildren<T>(this Node node) where T : Node
    {
        return node.GetChildren().OfType<T>().ToList();
    }

    public static async void AddChildDeferred(this Node node, Node child)
    {
        try
        {
            node.CallDeferred(Node.MethodName.AddChild, child);
            await child.ToSignal(child, Node.SignalName.Ready);
        }
        catch (Exception e)
        {
            Log.Error($"{nameof(AddChildDeferred)} failed: {e}");
        }
    }

    public static void ClearAndFreeChildren(this Node node)
    {
        foreach (var child in node.GetChildren())
        {
            node.RemoveChild(child);
            child.QueueFree();
        }
    }

    public static Node? FirstChild(this Node n)
    {
        return n.GetChildCount() == 0 ? null : n.GetChild(0);
    }

    public static Node? LastChild(this Node n)
    {
        var count = n.GetChildCount();
        return count == 0 ? null : n.GetChild(count - 1);
    }

    public static void AddToGroup(this Node node)
    {
        node.AddToGroup(node.GetType().Name);
    }
    public static async Task NextProcessFrame(this Node node)
    {
        await node.ToSignal(node.GetTree(), SceneTree.SignalName.ProcessFrame);
    }
    public static async Task NextPhysicsFrame(this Node node)
    {
        await node.ToSignal(node.GetTree(), SceneTree.SignalName.PhysicsFrame);
    }
}