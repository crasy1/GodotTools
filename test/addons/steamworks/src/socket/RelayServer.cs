using System;
using System.Linq;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public partial class RelayServer : SteamSocket
{
    public bool Started { set; get; }
    private SocketManager? SocketManager { set; get; }
    private int Port { set; get; }

    public RelayServer(int port)
    {
        Port = port;
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
            MySocketManager mySocketManager = new(this);
            SocketManager = SteamNetworkingSockets.CreateRelaySocket(Port, mySocketManager);
            SetProcess(true);
            Log.Info($"创建 relay server :port: {Port} 成功");
            Started = true;
        }
        catch (Exception e)
        {
            Log.Error($"创建 relay server :port: {Port} 异常, {e.Message}");
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
                Log.Info($"relay server :port {Port} 向 {connection.ConnectionName} 发送消息 {result}");
            }
        }
    }

    public override void Close()
    {
        SocketManager?.Close();
        SetProcess(false);
        SocketManager = null;
        Log.Info($"关闭 relay server :port {Port}");
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