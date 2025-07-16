using System;
using System.Linq;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public partial class RelayClient : SteamSocket
{
    private ConnectionManager? ConnectionManager { set; get; }
    private int Port { set; get; }
    private SteamId ServerId { set; get; }

    public RelayClient(SteamId serverId, int port)
    {
        Port = port;
        ServerId = serverId;
    }

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(double delta)
    {
        ConnectionManager?.Receive();
    }

    public void Connect()
    {
        try
        {
            MyConnectionManager myConnectionManager = new(this);
            ConnectionManager = SteamNetworkingSockets.ConnectRelay(ServerId, Port, myConnectionManager);
            SetProcess(ConnectionManager!.Connected);
            Log.Info($"创建 relay client :{ServerId} {(ConnectionManager!.Connected ? "成功" : "失败")}");
        }
        catch (Exception e)
        {
            Log.Error($"创建 relay client :{ServerId} 异常, {e.Message}");
        }
    }

    public void Send(string content, SendType sendType = SendType.Reliable)
    {
        if (ConnectionManager is { Connected: true })
        {
            var result = ConnectionManager.Connection.SendMessage(content, sendType);
            Log.Info($"relay client 向 {ConnectionManager.Connection.ConnectionName} 发送消息 {result}");
        }
    }

    public override void Close()
    {
        ConnectionManager?.Close();
        ConnectionManager = null;
        Log.Info($"关闭 relay client");
    }

    public override void _Notification(int what)
    {
        if (NotificationPredelete == what)
        {
            Close();
        }
    }
}