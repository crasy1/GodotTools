#nullable enable
using System;
using System.Linq;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public partial class NormalServer : SteamSocket
{
    public bool Started { set; get; }
    private SocketManager? SocketManager { set; get; }
    private ushort Port { set; get; }
    public NetAddress NetAddress { set; get; }

    public NormalServer(ushort port)
    {
        Port = port;
        NetAddress = NetAddress.AnyIp(Port);
    }

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(double delta)
    {
        SocketManager?.Receive();
    }

    public void Create()
    {
        try
        {
            MySocketManager mySocketManager = new(nameof(NormalServer));
            SocketManager = SteamNetworkingSockets.CreateNormalSocket(NetAddress, mySocketManager);
            SetProcess(true);
            Log.Info($"创建 normal server :{NetAddress} 成功");
            Started = true;
        }
        catch (Exception e)
        {
            Log.Error($"创建 normal server :{NetAddress} 异常, {e.Message}");
            Started = false;
        }
    }

    public void Send(string content, SendType sendType = SendType.Reliable)
    {
        if (SocketManager != null && Started)
        {
            foreach (var connection in SocketManager.Connected)
            {
                var result = connection.SendMessage(content, sendType);
                Log.Info($"normal server :{NetAddress} 向 {connection.ConnectionName} 发送消息 {result}");
            }
        }
    }

    public override void Close()
    {
        SocketManager?.Close();
        SetProcess(false);
        SocketManager = null;
        Log.Info($"关闭 normal server :{NetAddress}");
        Started = false;
    }

    public override void _Notification(int what)
    {
        if (new[] { NotificationPredelete }.Contains(what))
        {
            Close();
        }
    }
}