using System;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public class MyConnectionManager : IConnectionManager
{
    private string Name { set; get; }

    public MyConnectionManager(string name)
    {
        Name = name;
    }

    public void OnConnecting(ConnectionInfo info)
    {
        Log.Info($"client {Name} {info.Identity}正在连接");
    }

    public void OnConnected(ConnectionInfo info)
    {
        Log.Info($"client {Name} {info.Identity} 已经连接 ");
    }

    public void OnDisconnected(ConnectionInfo info)
    {
        Log.Info($"client {Name} {info.Identity} 断开连接 ");
    }

    public void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        Log.Info($"client {Name} 收到信息 ");
    }
}