using System;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public class MyConnectionManager:IConnectionManager
{
    public void OnConnecting(ConnectionInfo info)
    {
        Log.Info("正在连接 ",info);
    }

    public void OnConnected(ConnectionInfo info)
    {
        Log.Info("已经连接 ",info);
    }

    public void OnDisconnected(ConnectionInfo info)
    {
        Log.Info("断开连接 ",info);
    }

    public void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        Log.Info("收到信息 ");
    }
}