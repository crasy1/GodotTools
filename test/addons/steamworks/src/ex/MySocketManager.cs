using System;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public class MySocketManager : ISocketManager
{
    public void OnConnecting(Connection connection, ConnectionInfo info)
    {
        Log.Info("正在连接 ", connection, info);
    }

    public void OnConnected(Connection connection, ConnectionInfo info)
    {
        Log.Info("已连接 ", connection, info);
    }

    public void OnDisconnected(Connection connection, ConnectionInfo info)
    {
        Log.Info("断开连接 ", connection, info);
    }

    public void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum,
        long recvTime,
        int channel)
    {
        Log.Info("收到消息 ", connection, identity);
    }
}