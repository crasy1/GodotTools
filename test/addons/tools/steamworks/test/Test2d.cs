using Godot;
using System;
using System.Reflection;
using Steamworks;
using Steamworks.Data;

[SceneTree]
public partial class Test2d : Node2D
{
    private bool IsServer { set; get; }
    public Test2dPlayer LocalPlayer { set; get; }
    public Test2dPlayer OtherPlayer { set; get; }

    private Friend ChooseFriend { set; get; }

    public override void _Ready()
    {
        var multiplayerApi = GetTree().GetMultiplayer();
        Create.Pressed += () =>
        {
            multiplayerApi.PeerConnected += (id) =>
            {
                Log.Info($"[服务端]已连接：{id}");
                OtherPlayer = Test2dPlayer.Instantiate();
                OtherPlayer.IsLocal = false;
                AddChild(OtherPlayer);
                OtherPlayer.Position = new Vector2(750, 300);
            };
            multiplayerApi.PeerDisconnected += (id) => { Log.Info($"[服务端]已断开：{id}"); };
            multiplayerApi.SetMultiplayerPeer(SteamworksServerPeer.CreateServer(5000));
            LocalPlayer = Test2dPlayer.Instantiate();
            LocalPlayer.IsLocal = true;
            AddChild(LocalPlayer);
            LocalPlayer.Position = new Vector2(150, 300);
            IsServer = true;
            Menu.Hide();
        };
        Search.Pressed += () => { };
        Join.Pressed += () =>
        {
            multiplayerApi.PeerConnected += (id) =>
            {
                Log.Info($"[客户端]已连接：{id}");
                OtherPlayer = Test2dPlayer.Instantiate();
                OtherPlayer.IsLocal = false;
                AddChild(OtherPlayer);
                OtherPlayer.Position = new Vector2(150, 300);

                LocalPlayer = Test2dPlayer.Instantiate();
                LocalPlayer.IsLocal = true;
                AddChild(LocalPlayer);
                LocalPlayer.Position = new Vector2(750, 300);
                IsServer = false;
                Menu.Hide();
            };
            multiplayerApi.PeerDisconnected += (id) => { Log.Info($"[客户端]已断开：{id}"); };
            multiplayerApi.SetMultiplayerPeer(SteamworksClientPeer.CreateClient(SteamClient.SteamId, 5000));
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
      
    }
}