using Godot;
using System;
using Steamworks;
using Steamworks.Data;

[SceneTree]
public partial class SteamTest : Node2D
{
    private SteamUserInfo SteamUserInfo { set; get; }
    private NormalServer NormalServer { set; get; }
    private NormalClient NormalClient { set; get; }
    private RelayServer RelayServer { set; get; }
    private RelayClient RelayClient { set; get; }

    public override void _Ready()
    {
        base._Ready();
        SNetworking.ReceiveData += (steamId, data) =>
        {
            Log.Info($"收到信息时间：{Time.GetUnixTimeFromSystem()}");
            P2PReceiveText.AppendText(data);
        };

        ShowFriend.Pressed += () => { ShowFriends(); };
        SendP2P.Pressed += () =>
        {
            if (SteamUserInfo == null)
            {
                return;
            }

            SNetworking.SendP2P(SteamUserInfo.Friend.Id, P2PText.Text);
            Log.Info($"发送信息时间：{Time.GetUnixTimeFromSystem()}");
            // SendText.Text = "";
        };
        // NormalServerIp,NormalServerPort,NormalServerText,NormalServerReceiveText
        CreateNormalServer.Pressed += () =>
        {
            NormalServer?.Close();
            NormalServer = SNetworkingSockets.CreateNormal((ushort)NormalServerPort.Value);
            NormalServer.Create();
            NormalServerIp.Text = NormalServer.NetAddress.Address.ToString();
            NormalServerPort.Value = NormalServer.NetAddress.Port;
            NormalServer.ReceiveMessage += (id, msg) => { NormalServerReceiveText.AddText($"{id}:{msg} \r\n"); };
            NormalServer.Connected += (id) =>
            {
                NormalServerReceiveText.AddText($"已连接：{id} \r\n");
                NormalServerConnections.AddChild(SteamUserInfo.Instantiate(SFriends.Friends[id]));
            };
            NormalServer.Disconnected += (id) =>
            {
                NormalServerReceiveText.AddText($"已断开连接：{id} \r\n");
                foreach (var steamUserInfo in NormalServerConnections.GetChildren<SteamUserInfo>())
                {
                    if (steamUserInfo.Friend.Id == id)
                    {
                        steamUserInfo.QueueFree();
                        NormalServerConnections.RemoveChild(steamUserInfo);
                    }
                }
            };
        };
        CloseNormalServer.Pressed += () =>
        {
            NormalServer?.Close();
            NormalServer = null;
        };
        SendToClientNormal.Pressed += () => { NormalServer?.Send(NormalServerText.Text); };
        // NormalIp,NormalPort,NormalClientText,NormalClientReceiveText
        ConnectNormalClient.Pressed += () =>
        {
            NormalClient?.Close();
            var host = NormalIp.Text;
            NormalClient = SNetworkingSockets.ConnectNormal((ushort)NormalPort.Value);
            NormalClient.ReceiveMessage += (id, msg) => { NormalClientReceiveText.AddText($"{id}:{msg} \r\n"); };
            NormalClient.Connected += (id) =>
            {
                NormalClientReceiveText.AddText($"已连接：{id} \r\n");
                NormalClientConnections.AddChild(SteamUserInfo.Instantiate(SFriends.Friends[id]));
            };
            NormalClient.Disconnected += (id) =>
            {
                NormalClient = null;
                NormalClientReceiveText.AddText($"已断开连接：{id} \r\n");
                foreach (var steamUserInfo in NormalClientConnections.GetChildren<SteamUserInfo>())
                {
                    if (steamUserInfo.Friend.Id == id)
                    {
                        steamUserInfo.QueueFree();
                        NormalClientConnections.RemoveChild(steamUserInfo);
                    }
                }
            };
            NormalClient.Connect();
        };
        DisconnectNormalClient.Pressed += () =>
        {
            NormalClient?.Close();
            NormalClient = null;
        };
        SendToNormalServer.Pressed += () => { NormalClient?.Send(NormalClientText.Text); };

        // RelayServerIp,RelayServerPort,RelayServerText,RelayServerReceiveText
        CreateRelayServer.Pressed += () =>
        {
            RelayServer?.Close();
            RelayServer = SNetworkingSockets.CreateRelay((int)RelayServerPort.Value);
            RelayServer.ReceiveMessage += (id, msg) => { RelayServerReceiveText.AddText($"{id}:{msg} \r\n"); };
            RelayServer.Connected += (id) =>
            {
                RelayServerReceiveText.AddText($"已连接：{id} \r\n");
                RelayServerConnections.AddChild(SteamUserInfo.Instantiate(SFriends.Friends[id]));
            };
            RelayServer.Disconnected += (id) =>
            {
                RelayServerReceiveText.AddText($"已断开连接：{id} \r\n");
                foreach (var steamUserInfo in RelayServerConnections.GetChildren<SteamUserInfo>())
                {
                    if (steamUserInfo.Friend.Id == id)
                    {
                        steamUserInfo.QueueFree();
                        RelayServerConnections.RemoveChild(steamUserInfo);
                    }
                }
            };
            RelayServer.Create();
        };
        CloseRelayServer.Pressed += () =>
        {
            RelayServer?.Close();
            RelayServer = null;
        };
        SendToClientRelay.Pressed += () => { RelayServer?.Send(RelayServerText.Text); };
        // RelayIp,RelayPort,RelayClientText,RelayClientReceiveText
        ConnectRelayClient.Pressed += () =>
        {
            RelayClient?.Close();
            var host = RelayIp.Text;
            var port = (ushort)RelayPort.Value;
            // RelayClient = SNetworkingSockets.ConnectRelay(SteamUserInfo.Friend.Id, port);
            RelayClient = SNetworkingSockets.ConnectRelay(SteamClient.SteamId, port);
            RelayClient.ReceiveMessage += (id, msg) => { RelayClientReceiveText.AddText($"{id}:{msg} \r\n"); };
            RelayClient.Connected += (id) =>
            {
                RelayClientReceiveText.AddText($"已连接：{id} \r\n");
                RelayClientConnections.AddChild(SteamUserInfo.Instantiate(SFriends.Friends[id]));
            };
            RelayClient.Disconnected += (id) =>
            {
                RelayClient = null;
                RelayClientReceiveText.AddText($"已断开连接：{id} \r\n");
                foreach (var steamUserInfo in RelayClientConnections.GetChildren<SteamUserInfo>())
                {
                    if (steamUserInfo.Friend.Id == id)
                    {
                        steamUserInfo.QueueFree();
                        RelayClientConnections.RemoveChild(steamUserInfo);
                    }
                }
            };
            RelayClient.Connect();
        };
        DisconnectRelayClient.Pressed += () =>
        {
            RelayClient?.Close();
            RelayClient = null;
        };
        SendToRelayServer.Pressed += () => { RelayClient?.Send(RelayClientText.Text); };
    }

    public void ShowFriends()
    {
        Friends.ClearAndFreeChildren();
        foreach (var friend in SteamFriends.GetFriends())
        {
            if (friend.IsOnline)
            {
                var steamUserInfo = SteamUserInfo.Instantiate(friend);
                Friends.AddChild(steamUserInfo);
                steamUserInfo.GuiInput += (@event) =>
                {
                    if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
                    {
                        SteamUserInfo = steamUserInfo;
                    }
                };
            }
        }
    }
}