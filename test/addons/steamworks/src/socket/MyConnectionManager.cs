using System;
using System.Runtime.InteropServices;
using System.Text;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public class MyConnectionManager : IConnectionManager
{
    private SteamSocket SteamSocket { set; get; }
    private string Name { set; get; }

    public Friend Friend { private set; get; }

    public MyConnectionManager(SteamSocket steamSocket)
    {
        SteamSocket = steamSocket;
        Name = SteamSocket.Name;
    }

    public void OnConnecting(ConnectionInfo info)
    {
        if (GodotObject.IsInstanceValid(SteamSocket))
        {
            SteamSocket.SetProcess(false);
        }
    }

    public void OnConnected(ConnectionInfo info)
    {
        if (GodotObject.IsInstanceValid(SteamSocket))
        {
            SteamSocket.SetProcess(true);
            if (info.Identity.IsSteamId)
            {
                if (SFriends.Friends.TryGetValue(info.Identity.SteamId, out var friend))
                {
                    Friend = friend;
                    Log.Info($"{Name} 已连接 {Friend.Id},{Friend.Name}");
                }
            }
        }
    }

    public void OnDisconnected(ConnectionInfo info)
    {
        if (GodotObject.IsInstanceValid(SteamSocket))
        {
            SteamSocket.SetProcess(false);
            Log.Info($"{Name} 与 {Friend.Id},{Friend.Name} 断开连接");
        }
    }

    public void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        if (GodotObject.IsInstanceValid(SteamSocket))
        {
            // Encoding.UTF8.GetString(data, size)
            var msg = Marshal.PtrToStringUTF8(data, size);
            Log.Info($"{Name} 从 {Friend.Id},{Friend.Name} 收到信息 {msg}");
            SteamSocket.EmitSignal(SteamSocket.SignalName.ReceiveMessage, Friend.Id.Value, msg);
        }
    }
}