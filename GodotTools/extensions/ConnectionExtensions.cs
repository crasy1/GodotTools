using ProtoBuf;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public static class ConnectionExtensions
{
    /// <summary>
    /// 发送 protobuf 消息
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="msg"></param>
    /// <param name="sendType"></param>
    /// <returns></returns>
    public static unsafe Result SendMsg(this Connection connection, ProtoBufMsg msg,
        SendType sendType = SendType.Reliable)
    {
        using var stream = new MemoryStream();
        Serializer.Serialize(stream, msg);
        fixed (byte* ptr = stream.GetBuffer())
        {
            return connection.SendMessage((IntPtr)ptr, (int)stream.Length, sendType);
        }
    }
}