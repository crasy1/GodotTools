using System;
using System.Linq;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public partial class NormalClient : SteamSocket
{
    private ConnectionManager? ConnectionManager { set; get; }
    private ushort Port { set; get; }
    private NetAddress NetAddress { set; get; }

    public NormalClient(ushort port)
    {
        Port = port;
        NetAddress = NetAddress.LocalHost(Port);
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
            MyConnectionManager myConnectionManager = new(nameof(NormalClient));
            ConnectionManager = SteamNetworkingSockets.ConnectNormal(NetAddress, myConnectionManager);
            SetProcess(ConnectionManager!.Connected);
            Log.Info($"创建 normal client :{NetAddress} {(ConnectionManager!.Connected ? "成功" : "失败")}");
        }
        catch (Exception e)
        {
            Log.Error($"创建 normal client :{NetAddress} 异常, {e.Message}");
        }
    }

    public void Send(string content, SendType sendType = SendType.Reliable)
    {
        if (ConnectionManager is { Connected: true })
        {
            var result = ConnectionManager.Connection.SendMessage(content, sendType);
            Log.Info($"normal client 向 {ConnectionManager.Connection.ConnectionName} 发送消息 {result}");
        }
    }

    public override void Close()
    {
        ConnectionManager?.Close();
        ConnectionManager = null;
        SetProcess(false);
        Log.Info($"关闭 normal client");
    }

    public override void _Notification(int what)
    {
        if (new[] { NotificationWMCloseRequest, NotificationPredelete }.Contains(what))
        {
            Close();
        }
    }
}