using System;
using System.Linq;
using Steamworks;
using Steamworks.Data;

namespace Godot;
/// <summary>
/// 通过friend steamId 连接，走steam中继网络，延迟较高
/// </summary>
public partial class RelayServer : SteamSocket
{
    public bool Started { set; get; }
    public SocketManager? SocketManager { set; get; }
    public MySocketManager? ISocketManager { set; get; }
    private int Port { set; get; }

    public RelayServer(int port)
    {
        Port = port;
        SocketName = $"[RelayServer] {Port}";
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
            SocketManager = SteamNetworkingSockets.CreateRelaySocket(Port, ISocketManager);
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