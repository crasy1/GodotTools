using Godot;
using System;
using Steamworks;

[SceneTree]
public partial class SteamUserInfo : Control
{
    public Friend Friend { private set; get; }

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
        State.Text = Friend.State.State();
        Avatar.Texture = (await SFriends.Instance.Avatar(Friend.Id))?.Texture();
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