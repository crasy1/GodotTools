using System;
using System.Linq;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public partial class NormalClient : SteamSocket
{
    public ConnectionManager? ConnectionManager { set; get; }
    public MyConnectionManager? IConnectionManager { set; get; }
    private ushort Port { set; get; }
    private NetAddress NetAddress { set; get; }

    public NormalClient(ushort port)
    {
        Port = port;
        NetAddress = NetAddress.LocalHost(Port);
        SocketName = $"[NormalClient] {Port}";
    }

    public override void _Ready()
    {
        base._Ready();
        Disconnected += (id) => QueueFree();
    }

    public override void _Process(double delta)
    {
        ConnectionManager?.Receive();
    }

    public void Connect()
    {
        try
        {
            IConnectionManager = new(this);
            ConnectionManager = SteamNetworkingSockets.ConnectNormal(NetAddress, IConnectionManager);
            Log.Info($"{SocketName} => 创建");
        }
        catch (Exception e)
        {
            Log.Error($"{SocketName} => 创建异常, {e.Message}");
        }
    }

    public void Send(string content, SendType sendType = SendType.Reliable)
    {
        if (!string.IsNullOrEmpty(content) && ConnectionManager is { Connected: true })
        {
            var result = ConnectionManager.Connection.SendMessage(content, sendType);
            Log.Info($"{SocketName} => 向 {ConnectionManager.ConnectionInfo.Identity} 发送消息 {result}");
        }
    }

    public override void Close()
    {
        ConnectionManager?.Close();
        ConnectionManager = null;
        IConnectionManager = null;
        SetProcess(false);
        Log.Info($"{SocketName} => 关闭");
    }
}