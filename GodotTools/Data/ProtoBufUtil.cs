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
            .Add(2, nameof(Vector2.Y));
        RuntimeTypeModel.Default
            .Add(typeof(Vector2I), false)
            .Add(1, nameof(Vector2.X))
            .Add(2, nameof(Vector2.Y));
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
            .Add(2, nameof(Vector2.Y));
        RuntimeTypeModel.Default
            .Add(typeof(Rect2), false)
            .Add(1, nameof(Rect2.Position))
            .Add(2, nameof(Rect2.Size))
            .Add(3, nameof(Rect2.End));
        RuntimeTypeModel.Default
            .Add(typeof(Rect2I), false)
            .Add(1, nameof(Rect2I.Position))
            .Add(2, nameof(Rect2I.Size))
            .Add(3, nameof(Rect2I.End));
        
    }
}