using ProtoBuf;

namespace Godot;

[ProtoContract]
public class ProtoBufMsg : GodotObject
{
    /// <summary>
    /// 数据类型
    /// 必须是protobuf支持的类型
    /// </summary>
    [ProtoMember(1)]
    public Type Type { get; set; }

    /// <summary>
    /// 数据
    /// </summary>
    [ProtoMember(2)]
    public byte[] Data { get; set; }

    /// <summary>
    /// 转换对象为ProtoBufMsg
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ProtoBufMsg From<T>(T obj) where T : class
    {
        using var stream = new MemoryStream();
        Serializer.Serialize(stream, obj);
        return new ProtoBufMsg()
        {
            Type = obj.GetType(),
            Data = stream.ToArray()
        };
    }

    /// <summary>
    /// ptr转换为ProtoBufMsg
    /// </summary>
    /// <param name="ptr"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static ProtoBufMsg From(IntPtr ptr, int length)
    {
        using var stream = ptr.UnmanagedMemoryStream(length);
        return Serializer.Deserialize<ProtoBufMsg>(stream);
    }

    /// <summary>
    /// 检测是否是指定类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool Is<T>() => typeof(T) == Type;

    /// <summary>
    /// 序列化为ToStream 记得关闭MemoryStream
    /// </summary>
    /// <returns>MemoryStream</returns>
    public MemoryStream SerializeToStream()
    {
        var stream = new MemoryStream();
        Serializer.Serialize(stream, this);
        return stream;
    }

    public unsafe (IntPtr, long) SerializeToPtr()
    {
        var stream = new MemoryStream();
        Serializer.Serialize(stream, this);
        fixed (byte* ptr = stream.GetBuffer())
        {
            return ((IntPtr)ptr, stream.Length);
        }
    }

    /// <summary>
    /// 序列化为字节数组
    /// </summary>
    /// <returns></returns>
    public byte[] Serialize()
    {
        using var stream = SerializeToStream();
        return stream.ToArray();
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public object Deserialize()
    {
        using var stream = new MemoryStream(Data);
        return Serializer.Deserialize(Type, stream) ?? throw new Exception($"ProtoMsg {Type} 反序列化错误");
    }

    /// <summary>
    /// 反序列化为指定类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public T Deserialize<T>() where T : class, new()
    {
        return Deserialize() as T ?? throw new Exception($"ProtoMsg {Type} 反序列化为 {nameof(T)} 错误");
    }
}