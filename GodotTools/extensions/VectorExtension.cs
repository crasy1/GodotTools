using Godot;

namespace GodotTools.extensions;

public static class VectorExtension
{
    public static Vector2 RotatedDegrees(this Vector2 v, float degrees)
    {
        return v.Rotated(Mathf.DegToRad(degrees));
    }

    public static bool WithinDistance(this Vector2 v, Vector2 v2, float distance)
    {
        return v.DistanceSquaredTo(v2) <= distance * distance;
    }
}