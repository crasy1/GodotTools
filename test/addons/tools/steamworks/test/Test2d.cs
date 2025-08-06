using Godot;
using System;

[SceneTree]
public partial class Test2d : Node2D
{
    public override void _Ready()
    {
        Create.Pressed += () =>
        {
            var normalServer = new NormalServer(5000);
            Create.AddChild(normalServer);
            normalServer.ReceiveMessage += OnServerReceiveMessage;
            normalServer.Connected += (steamId) =>
            {
                Log.Info($"[服务端]已连接：{steamId}");
                Menu.Hide();
            };
            normalServer.Disconnected += (steamId) => { Log.Info($"[服务端]已断开：{steamId}"); };
            normalServer.Create();
        };
        Search.Pressed += () => { };
        Join.Pressed += () =>
        {
            var normalClient = new NormalClient("127.0.0.1", 5000);
            Join.AddChild(normalClient);
            normalClient.ReceiveMessage += OnClientReceiveMessage;
            normalClient.Connected += (steamId) =>
            {
                Log.Info($"[客户端]已连接：{steamId}");
                Menu.Hide();
            };
            normalClient.Disconnected += (steamId) => { Log.Info($"[客户端]已断开：{steamId}"); };
            normalClient.Connect();
        };
    }


    private void OnServerReceiveMessage(ulong steamId, GodotObject msg)
    {
        var protoBufMsg = msg as ProtoBufMsg;
        var deserialize = protoBufMsg.Deserialize();
    }

    private void OnClientReceiveMessage(ulong steamId, GodotObject msg)
    {
        var protoBufMsg = msg as ProtoBufMsg;
        var deserialize = protoBufMsg.Deserialize();
    }
}