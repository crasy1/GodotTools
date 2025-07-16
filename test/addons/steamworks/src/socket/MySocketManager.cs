using System;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public class MySocketManager : ISocketManager
{
    private string Name { set; get; }

    public MySocketManager(string name)
    {
        Name = name;
    }

    public void OnConnecting(Connection connection, ConnectionInfo info)
    {
        Log.Info($"server {Name} 正在连接 {connection.ConnectionName},{info.Address}");
    }

    public void OnConnected(Connection connection, ConnectionInfo info)
    {
        Log.Info($"server {Name} 已连接 {connection.ConnectionName},{info.Address}");
    }

    public void OnDisconnected(Connection connection, ConnectionInfo info)
    {
        Log.Info($"server {Name} 断开连接 {connection.ConnectionName} {info.Address}");
    }

    public void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum,
        long recvTime,
        int channel)
    {
        Log.Info($"server {Name} {connection.ConnectionName} 从 {identity} 收到消息 ");
    }
}