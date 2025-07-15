using Godot;
using System;
using Steamworks;
using Steamworks.Data;

[SceneTree]
public partial class SteamTest : Node2D
{
    private SteamUserInfo SteamUserInfo { set; get; }

    public override void _Ready()
    {
        base._Ready();
        SNetworking.ReceiveData += (steamId, data) =>
        {
            Log.Info($"收到信息时间：{Time.GetUnixTimeFromSystem()}");
            P2PReceiveText.AppendText(data);
        };
        ShowFriend.Pressed += () =>
        {
            ShowFriends();
        };
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
        // NormalIp,NormalPort,NormalClientText,NormalClientReceiveText
        CreateNormal.Pressed += () =>
        {
            SNetworkingSockets.Instance.CreateNormal();
        };
        CloseNormal.Pressed += () =>
        {
            SNetworkingSockets.Instance.NormalServer?.Close();
        };
        SendToClientNormal.Pressed += () =>
        {
            if (SNetworkingSockets.Instance.NormalServer!=null)
            {
                foreach (var connection in SNetworkingSockets.Instance.NormalServer.Connected)
                {
                    var result = connection.SendMessage(NormalText.Text);
                    Log.Info($"向 {connection} 发送消息 {result}");
                }
            }
        };
        // NormalIp,NormalPort,NormalClientText,NormalClientReceiveText
        ConnectNormal.Pressed += () =>
        {
            var host = NormalIp.Text;
            var port = (ushort)NormalPort.Value;
            
            SNetworkingSockets.Instance.ConnectNormal(NetAddress.From(host,port));
        };
        DisconnectNormal.Pressed += () =>
        {
            SNetworkingSockets.Instance.NormalClient?.Close();
        };
        SendToNormalServer.Pressed += () =>
        {
            // SNetworkingSockets.Instance.NormalClient?.SendMessages(NormalClientText.Text);
        };
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