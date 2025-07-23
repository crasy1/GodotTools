using System;
using System.Drawing;
using System.Numerics;
using ProtoBuf;

namespace Godot;

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
    [ProtoMember(17)] private StringName StringName { set; get; }
    [ProtoMember(18)] private NodePath NodePath { set; get; }
    [ProtoMember(19)] private Rid Rid { set; get; }
    [ProtoMember(20)] private GodotObject GodotObject { set; get; }
    [ProtoMember(21)] private Godot.Collections.Dictionary Dictionary { set; get; }
    [ProtoMember(22)] private Godot.Collections.Array Array { set; get; }
    [ProtoMember(23)] private Byte[] ByteArray { set; get; }
    [ProtoMember(24)] private Int32[] Int32Array { set; get; }
    [ProtoMember(25)] private Int64[] Int64Array { set; get; }
    [ProtoMember(26)] private Single[] Float32Array { set; get; }
    [ProtoMember(27)] private Double[] Float64Array { set; get; }
    [ProtoMember(28)] private String[] StringArray { set; get; }
    [ProtoMember(29)] private Vector2[] Vector2Array { set; get; }
    [ProtoMember(30)] private Vector3[] Vector3Array { set; get; }
    [ProtoMember(31)] private Color[] ColorArray { set; get; }
    [ProtoMember(32)] private Vector4[] Vector4Array { set; get; }

    public static void Test()
    {
        ProtoBufUtil.Init();
        var msg = new GodotMsg()
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
            StringName = new StringName("test"),
            NodePath = new NodePath("test"),
            Rid = new Rid(new Node()),
            GodotObject = new GodotObject(),
            Dictionary = new Godot.Collections.Dictionary()
                { },
            Array = new Godot.Collections.Array()
                { },
            ByteArray = new Byte[] { 1, 2, 3, 4 },
            Int32Array = new Int32[] { 1, 2, 3, 4 },
            Int64Array = new Int64[] { 1, 2, 3, 4 },
            Float32Array = new Single[] { 1, 2, 3, 4 },
            Float64Array = new Double[] { 1, 2, 3, 4 },
            StringArray = new String[] { "test", "test2" },
            Vector2Array = new Vector2[] { new Vector2(1, 2), new Vector2(3, 4) },
            Vector3Array = new Vector3[] { new Vector3(1, 2, 3), new Vector3(4, 5, 6) },
            ColorArray = new Color[] { new Color(1, 2, 3, 4), new Color(5, 6, 7, 8) },
            Vector4Array = new Vector4[] { new Vector4(1, 2, 3, 4), new Vector4(5, 6, 7, 8) },
        };

        var protoBufMsg = ProtoBufMsg.From(msg);
        protoBufMsg.Serialize();
    }
    public static void Main()
    {
        
    }
}