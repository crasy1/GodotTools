using Godot;
using System;
using Steamworks;

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
            ReceiveText.AppendText(data);
        };
        ShowFriend.Pressed += () =>
        {
            ShowFriends();
        };
        Send.Pressed += () =>
        {
            if (SteamUserInfo == null)
            {
                return;
            }

            SNetworking.SendP2P(SteamUserInfo.Friend.Id, SendText.Text);
            Log.Info($"发送信息时间：{Time.GetUnixTimeFromSystem()}");
            // SendText.Text = "";
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