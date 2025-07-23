using ProtoBuf.Meta;

namespace Godot;

public static class ProtoBufUtil
{
    public static void Init()
    {
        // TODO 添加更多类型
        // 在程序启动时配置一次
        RuntimeTypeModel.Default
            .Add(typeof(Vector2), false)
            .Add(1, nameof(Vector2.X))
            .Add(2, nameof(Vector2.Y))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Vector2I), false)
            .Add(1, nameof(Vector2.X))
            .Add(2, nameof(Vector2.Y))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Vector3), false)
            .Add(1, nameof(Vector3.X))
            .Add(2, nameof(Vector3.Y))
            .Add(3, nameof(Vector3.Z))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Vector3I), false)
            .Add(1, nameof(Vector3I.X))
            .Add(2, nameof(Vector3I.Y))
            .Add(3, nameof(Vector3I.Z))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Vector4), false)
            .Add(1, nameof(Vector4.X))
            .Add(2, nameof(Vector4.Y))
            .Add(3, nameof(Vector4.Z))
            .Add(4, nameof(Vector4.W))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Vector4I), false)
            .Add(1, nameof(Vector4I.X))
            .Add(2, nameof(Vector4I.Y))
            .Add(3, nameof(Vector4I.Z))
            .Add(4, nameof(Vector4I.W))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Vector2I), false)
            .Add(1, nameof(Vector2.X))
            .Add(2, nameof(Vector2.Y))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Rect2), false)
            .Add(1, nameof(Rect2.Position))
            .Add(2, nameof(Rect2.Size))
            .Add(3, nameof(Rect2.End))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Rect2I), false)
            .Add(1, nameof(Rect2I.Position))
            .Add(2, nameof(Rect2I.Size))
            .Add(3, nameof(Rect2I.End))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Transform2D), false)
            .Add(1, nameof(Transform2D.X))
            .Add(2, nameof(Transform2D.Y))
            .Add(3, nameof(Transform2D.Origin))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Plane), false)
            .Add(1, nameof(Plane.Normal))
            .Add(2, nameof(Plane.D))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Quaternion), false)
            .Add(1, nameof(Quaternion.X))
            .Add(2, nameof(Quaternion.Y))
            .Add(3, nameof(Quaternion.Z))
            .Add(4, nameof(Quaternion.W))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Aabb), false)
            .Add(1, nameof(Aabb.Position))
            .Add(2, nameof(Aabb.Size))
            .Add(3, nameof(Aabb.End))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Basis), false)
            .Add(1, nameof(Basis.Row0))
            .Add(2, nameof(Basis.Row1))
            .Add(3, nameof(Basis.Row2))
            .Add(4, nameof(Basis.Column0))
            .Add(5, nameof(Basis.Column1))
            .Add(6, nameof(Basis.Column2))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Projection), false)
            .Add(1, nameof(Projection.X))
            .Add(2, nameof(Projection.Y))
            .Add(3, nameof(Projection.Z))
            .Add(4, nameof(Projection.W))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Transform3D), false)
            .Add(1, nameof(Transform3D.Basis))
            .Add(2, nameof(Transform3D.Origin))
            ;
        RuntimeTypeModel.Default
            .Add(typeof(Color), false)
            .Add(1, nameof(Color.R))
            .Add(2, nameof(Color.G))
            .Add(3, nameof(Color.B))
            .Add(4, nameof(Color.A))
            ;
        
    }
}