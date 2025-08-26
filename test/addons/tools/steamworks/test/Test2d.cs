using Godot;
using System;
using System.Reflection;
using Steamworks;
using Steamworks.Data;

[SceneTree]
public partial class Test2d : Node2D
{
    private bool IsServer { set; get; }
    private NormalServer NormalServer { set; get; }
    private NormalClient NormalClient { set; get; }

    public Test2dPlayer LocalPlayer { set; get; }
    public Test2dPlayer OtherPlayer { set; get; }

    private Friend ChooseFriend { set; get; }

    public override void _Ready()
    {
        Create.Pressed += () =>
        {
            NormalServer = new NormalServer(5000);
            Create.AddChild(NormalServer);
            NormalServer.ReceiveMessage += OnReceiveMessage;
            NormalServer.Connected += (steamId) =>
            {
                Log.Info($"[服务端]已连接：{steamId}");
                OtherPlayer = Test2dPlayer.Instantiate();
                OtherPlayer.IsLocal = false;
                AddChild(OtherPlayer);
                OtherPlayer.Position = new Vector2(750, 300);
            };
            NormalServer.Disconnected += (steamId) => { Log.Info($"[服务端]已断开：{steamId}"); };
            NormalServer.Create();
            LocalPlayer = Test2dPlayer.Instantiate();
            LocalPlayer.IsLocal = true;
            AddChild(LocalPlayer);
            LocalPlayer.Position = new Vector2(150, 300);
            LocalPlayer.SteamSocket = NormalServer;
            IsServer = true;
            Menu.Hide();
        };
        Search.Pressed += () => { };
        Join.Pressed += () =>
        {
            NormalClient = new NormalClient("127.0.0.1", 5000);
            Join.AddChild(NormalClient);
            NormalClient.ReceiveMessage += OnReceiveMessage;
            NormalClient.Connected += (steamId) =>
            {
                Log.Info($"[客户端]已连接：{steamId}");
                OtherPlayer = Test2dPlayer.Instantiate();
                OtherPlayer.IsLocal = false;
                AddChild(OtherPlayer);
                OtherPlayer.Position = new Vector2(150, 300);

                LocalPlayer = Test2dPlayer.Instantiate();
                LocalPlayer.IsLocal = true;
                AddChild(LocalPlayer);
                LocalPlayer.Position = new Vector2(750, 300);
                LocalPlayer.SteamSocket = NormalClient;
                IsServer = false;
                Menu.Hide();
            };
            NormalClient.Disconnected += (steamId) => { Log.Info($"[客户端]已断开：{steamId}"); };
            NormalClient.Connect();
        };
        foreach (var (steamId, friend) in SFriends.Friends)
        {
            if (friend.IsOnline)
            {
                var steamUserInfo = SteamUserInfo.Instantiate(friend);
                Friends.AddChild(steamUserInfo);
                steamUserInfo.GuiInput += (@event) =>
                {
                    if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
                    {
                        ChooseFriend = friend;
                        Log.Info(friend.Id);
                    }
                };
            }
        }
    }

    private void OnReceiveMessage(ulong steamId, GodotObject msg)
    {
        var protoBufMsg = msg as ProtoBufMsg;
        if (protoBufMsg.Type == typeof(TestMsg))
        {
            var testMsg = protoBufMsg.Deserialize<TestMsg>();
            OtherPlayer.TestMsg = testMsg;
        }
    }


    public void Send(ProtoBufMsg msg, SendType sendType = SendType.Reliable)
    {
        if (IsServer)
        {
            NormalServer?.Send(msg, sendType);
        }
        else
        {
            NormalClient?.Send(msg, sendType);
        }
    }
}