using System.Runtime.InteropServices;

namespace Godot;

public static class IntPtrExtensions
{
    /// <summary>
    /// 将 IntPtr 转换为 byte[]
    /// </summary>
    /// <param name="ptr">非托管内存指针</param>
    /// <param name="length">数据长度（字节）</param>
    public static byte[] Bytes(this IntPtr ptr, int length)
    {
        var bytes = new byte[length];
        Marshal.Copy(ptr, bytes, 0, length);
        return bytes;
    }

    /// <summary>
    /// 将 IntPtr 转换为 MemoryStream
    /// </summary>
    /// <param name="ptr">非托管内存指针</param>
    /// <param name="length">数据长度（字节）</param>
    public static MemoryStream MemoryStream(this IntPtr ptr, int length)
    {
        return new MemoryStream(Bytes(ptr, length));
    }

    /// <summary>
    /// 将 IntPtr 转换 UnmanagedMemoryStream
    /// </summary>
    /// <param name="ptr">非托管内存指针</param>
    /// <param name="length">数据长度（字节）</param>
    public static unsafe UnmanagedMemoryStream UnmanagedMemoryStream(this IntPtr ptr, int length)
    {
        // 创建非托管内存流（可读写）
        return new UnmanagedMemoryStream((byte*)ptr, length, length, System.IO.FileAccess.Read);
    }
}