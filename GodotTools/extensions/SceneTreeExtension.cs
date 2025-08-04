namespace Godot;

using System.Collections.Generic;

public static class SceneTreeExtension
{
    public static T? FirstNodeInGroup<T>(this SceneTree sceneTree, string group) where T : Node
    {
        var node = sceneTree.GetFirstNodeInGroup(group);
        return node as T;
    }

    public static T? FirstNodeInGroup<T>(this SceneTree sceneTree) where T : Node
    {
        return sceneTree.FirstNodeInGroup<T>(typeof(T).Name);
    }

    public static IList<T> NodesInGroup<T>(this SceneTree sceneTree, string group) where T : Node
    {
        return sceneTree.GetNodesInGroup(group).Cast<T>().ToList();
    }

    public static IList<T> NodesInGroup<T>(this SceneTree sceneTree) where T : Node
    {
        var name = typeof(T).Name;
        return NodesInGroup<T>(sceneTree, name);
    }

    /// <summary>
    /// 传播退出通知，推荐停止游戏的时候用这个方法，会通知其他节点
    /// </summary>
    /// <param name="sceneTree"></param>
    public static void QuitNotification(this SceneTree sceneTree)
    {
        sceneTree.Root.PropagateNotification((int)Node.NotificationWMCloseRequest);
    }
}