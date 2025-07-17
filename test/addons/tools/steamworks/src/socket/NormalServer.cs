#nullable enable
using System;
using System.Linq;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public partial class NormalServer : SteamSocket
{
    public bool Started { set; get; }
    public SocketManager? SocketManager { set; get; }
    public MySocketManager? ISocketManager { set; get; }
    private ushort Port { set; get; }
    public NetAddress NetAddress { set; get; }

    public NormalServer(ushort port)
    {
        Port = port;
        NetAddress = NetAddress.AnyIp(Port);
        SocketName = $"[NormalServer] {NetAddress}";
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
            ISocketManager = new(this);
            SocketManager = SteamNetworkingSockets.CreateNormalSocket(NetAddress, ISocketManager);
            SetProcess(true);
            Log.Info($"{SocketName} => 创建");
            Started = true;
        }
        catch (Exception e)
        {
            Log.Error($"{SocketName} => 创建异常, {e.Message}");
            Started = false;
        }
    }

    public void Send(string content, SendType sendType = SendType.Reliable)
    {
        if (!string.IsNullOrEmpty(content) && SocketManager != null && Started)
        {
            foreach (var connection in SocketManager.Connected)
            {
                var result = connection.SendMessage(content, sendType);
                Log.Info($"{SocketName} => 向 {connection.Id} 发送消息 {result}");
            }
        }
    }

    public void Send(SteamId steamId, string content, SendType sendType = SendType.Reliable)
    {
        if (!string.IsNullOrEmpty(content) && SocketManager != null && Started && ISocketManager != null)
        {
            var connection = ISocketManager.Connections.FirstOrDefault(kv => steamId == kv.Value.Id).Key;
            var result = connection.SendMessage(content, sendType);
            Log.Info($"{SocketName} => 向 {connection.Id} 发送消息 {result}");
        }
    }

    public override void Close()
    {
        SocketManager?.Close();
        SetProcess(false);
        SocketManager = null;
        ISocketManager = null;
        Log.Info($"{SocketName} => 关闭");
        Started = false;
    }
}