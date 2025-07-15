using Godot;
using System;
using System.Threading.Tasks;
using Steamworks;

[SceneTree]
public partial class SteamUserInfo : Control
{
    public Friend Friend { set; get; }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        return obj.GetType() == GetType() && ((SteamUserInfo)obj).Friend.Id == Friend.Id;
    }

    [OnInstantiate]
    private void InitFriend(Friend friend)
    {
        Friend = friend;
    }

    public async override void _Ready()
    {
        base._Ready();
        UserName.Text = Friend.Name;
        NickName.Text = Friend.Nickname;
        State.Text = Friend.State switch
        {
            FriendState.Offline => "离线",
            FriendState.Online => "在线",
            FriendState.Busy => "忙碌",
            FriendState.Away => "离开",
            FriendState.Snooze => "勿扰",
            FriendState.LookingToTrade => "交易",
            FriendState.LookingToPlay => "游玩",
            FriendState.Invisible => "隐身"
        };
        Avatar.Texture = (await SFriends.Avatar(Friend.Id))?.Texture();
        Avatar.GuiInput += (@event =>
        {
            if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Right, Pressed: true })
            {
                Menu.Show();
            }
        });
        InviteGame.Pressed += () => { Friend.InviteToGame("来"); };
        InviteLobby.Pressed += () => { SMatchmaking.Instance.Invite(Friend.Id); };
        RemotePlay.Pressed += () => { SRemotePlay.Invite(Friend.Id); };
        Menu.Hide();
    }
}