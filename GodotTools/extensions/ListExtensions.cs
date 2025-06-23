using Godot;

namespace GodotTools.extensions;

public static class ListExtensions
{
    public static T PickUpRandom<T>(this IList<T> list)
    {
        return list[GD.RandRange(0, list.Count - 1)];
    }
}