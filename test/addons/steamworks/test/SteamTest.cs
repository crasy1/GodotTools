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
            NormalServer = SNetworkingSockets.CreateNormal((ushort)NormalServerPort.Value);
            NormalServer.Create();
            NormalServerIp.Text = NormalServer.NetAddress.Address.ToString();
            NormalServerPort.Value = NormalServer.NetAddress.Port;
        };
        CloseNormalServer.Pressed += () =>
        {
            NormalServer?.QueueFree();
            NormalServer = null;
        };
        SendToClientNormal.Pressed += () => { NormalServer?.Send(NormalServerText.Text); };
        // NormalIp,NormalPort,NormalClientText,NormalClientReceiveText
        ConnectNormalClient.Pressed += () =>
        {
            var host = NormalIp.Text;
            var port = (ushort)NormalPort.Value;
            NormalClient = SNetworkingSockets.ConnectNormal(port);
            NormalClient.Connect();
        };
        DisconnectNormalClient.Pressed += () =>
        {
            NormalClient?.QueueFree();
            NormalClient = null;
        };
        SendToNormalServer.Pressed += () => { NormalClient?.Send(NormalClientText.Text); };

        // RelayServerIp,RelayServerPort,RelayServerText,RelayServerReceiveText
        CreateRelayServer.Pressed += () =>
        {
            RelayServer = SNetworkingSockets.CreateRelay((int)RelayServerPort.Value);
            RelayServer.Create();
        };
        CloseRelayServer.Pressed += () =>
        {
            RelayServer?.QueueFree();
            RelayServer = null;
        };
        SendToClientRelay.Pressed += () => { RelayServer?.Send(RelayServerText.Text); };
        // RelayIp,RelayPort,RelayClientText,RelayClientReceiveText
        ConnectRelayClient.Pressed += () =>
        {
            var host = RelayIp.Text;
            var port = (ushort)RelayPort.Value;
            // RelayClient = SNetworkingSockets.ConnectRelay(SteamUserInfo.Friend.Id, port);
            RelayClient = SNetworkingSockets.ConnectRelay(SteamClient.SteamId, port);
            RelayClient.Connect();
        };
        DisconnectRelayClient.Pressed += () =>
        {
            RelayClient?.QueueFree();
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