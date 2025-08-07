using ProtoBuf;

namespace Godot;

/// <summary>
/// 测试godot类型
/// </summary>
[ProtoContract]
public class GodotMsg
{
    [ProtoMember(1)] private Vector2 Vector2 { set; get; }
    [ProtoMember(2)] private Vector2I Vector2I { set; get; }
    [ProtoMember(3)] private Vector3 Vector3 { set; get; }
    [ProtoMember(4)] private Vector3I Vector3I { set; get; }
    [ProtoMember(5)] private Vector4 Vector4 { set; get; }
    [ProtoMember(6)] private Vector4I Vector4I { set; get; }
    [ProtoMember(7)] private Rect2 Rect2 { set; get; }
    [ProtoMember(8)] private Rect2I Rect2I { set; get; }
    [ProtoMember(9)] private Transform2D Transform2D { set; get; }
    [ProtoMember(10)] private Plane Plane { set; get; }
    [ProtoMember(11)] private Quaternion Quaternion { set; get; }
    [ProtoMember(12)] private Aabb Aabb { set; get; }
    [ProtoMember(13)] private Basis Basis { set; get; }
    [ProtoMember(14)] private Projection Projection { set; get; }
    [ProtoMember(15)] private Transform3D Transform3D { set; get; }
    [ProtoMember(16)] private Color Color { set; get; }

    public static GodotMsg TestInstance()
    {
        return new GodotMsg()
        {
            Vector2 = new Vector2(1, 2),
            Vector2I = new Vector2I(1, 2),
            Vector3 = new Vector3(1, 2, 3),
            Vector3I = new Vector3I(1, 2, 3),
            Vector4 = new Vector4(1, 2, 3, 4),
            Vector4I = new Vector4I(1, 2, 3, 4),
            Rect2 = new Rect2(1, 2, 3, 4),
            Rect2I = new Rect2I(1, 2, 3, 4),
            Transform2D = new Transform2D(1, 2, 3, 4, 5, 6),
            Plane = new Plane(1, 2, 3, 4),
            Quaternion = new Quaternion(1, 2, 3, 4),
            Aabb = new Aabb(Vector3.One, 2, 3, 4),
            Basis = new Basis(1, 2, 3, 4, 5, 6, 7, 8, 9),
            Projection = new Projection(Transform3D.Identity),
            Transform3D = new Transform3D(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12),
            Color = new Color(1, 2, 3, 4),
        };
    }
}