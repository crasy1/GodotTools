using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

[SceneTree]
public partial class SteamLobby : Control
{
    public static SteamLobby MySteamLobby { set; get; }
    private int MaxUser { set; get; }
    private Lobby Lobby { set; get; }

    [OnInstantiate]
    private void Init(int maxUser)
    {
        MaxUser = maxUser;
    }

    public override void _Ready()
    {
        SteamMatchmaking.OnLobbyCreated += (result, lobby) =>
        {
            Log.Info($"创建房间结果 {result} , {lobby}");
            if (result == Result.OK)
            {
                Lobby = lobby;
                Lobby.SetPrivate();
                Lobby.SetJoinable(false);
                Lobby.SetData("version", ProjectSettings.GetSetting("config/version").AsString());
                UpdateLobbyData();
            }
        };
        SteamMatchmaking.OnLobbyInvite += (friend, lobby) => { Log.Info($"收到房间邀请 {friend} {lobby}"); };
        SteamMatchmaking.OnLobbyEntered += (lobby) => { Log.Info($"进入房间 {lobby}"); };
        SteamMatchmaking.OnLobbyGameCreated += (lobby, i, s, steamId) =>
        {
            Log.Info($"房间游戏已创建 {lobby} {i} {s} {steamId}");
        };
        SteamMatchmaking.OnLobbyDataChanged += (lobby) =>
        {
            Log.Info($"房间数据已改变 {lobby}");
            UpdateLobbyData();
        };
        SteamMatchmaking.OnChatMessage += (lobby, friend, message) =>
        {
            Log.Info($"房间聊天消息 {lobby} {friend} {message}");
        };
        SteamMatchmaking.OnLobbyMemberJoined += (lobby, friend) =>
        {
            Log.Info($"房间成员加入 {lobby} {friend}");
            UpdateLobbyData();
        };
        SteamMatchmaking.OnLobbyMemberLeave += (lobby, friend) =>
        {
            Log.Info($"房间成员离开 {lobby} {friend}");
            UpdateLobbyData();
        };
        SteamMatchmaking.OnLobbyMemberBanned += (lobby, friend, friend2) =>
        {
            Log.Info($"房间成员被禁言 {lobby} {friend} {friend2}");
        };
        SteamMatchmaking.OnLobbyMemberDataChanged += (lobby, friend) =>
        {
            Log.Info($"房间成员数据改变 {lobby} {friend}");
            UpdateLobbyData();
        };
        SteamMatchmaking.OnLobbyMemberDisconnected += (lobby, friend) =>
        {
            Log.Info($"房间成员断开连接 {lobby} {friend}");
            UpdateLobbyData();
        };
        SteamMatchmaking.OnLobbyMemberKicked += (lobby, friend, friend2) =>
        {
            Log.Info($"房间成员被踢 {lobby} {friend} {friend2}");
            UpdateLobbyData();
        };
        Joinable.Pressed += () => { Lobby.SetJoinable(Joinable.ButtonPressed); };
        LobbyType.ItemSelected += index =>
        {
            switch (index)
            {
                case 0:
                    Lobby.SetPrivate();
                    break;
                case 1:
                    Lobby.SetFriendsOnly();
                    break;
                case 2:
                    Lobby.SetPublic();
                    break;
                case 3:
                    Lobby.SetInvisible();
                    break;
            }
        };
        Join.Pressed += async () =>
        {
            var result = await Lobby.Join();
            if (result == RoomEnter.Success)
            {
                Log.Info("进入大厅");
            }
            else
            {
                Log.Info($"进入大厅失败 {result}");
            }
        };
        Exit.Pressed += () =>
        {
            Lobby.Leave();
            Log.Info("退出大厅");
            QueueFree();
            MySteamLobby = null;
        };
    }

    public void Create()
    {
        SteamMatchmaking.CreateLobbyAsync(MaxUser);
        MySteamLobby = this;
    }

    public static async Task<List<Lobby>> Search(int minSlots = 1, int maxResult = 10)
    {
        var lobbyQuery = SteamMatchmaking.LobbyList.WithMaxResults(maxResult);
        lobbyQuery = lobbyQuery.WithSlotsAvailable(minSlots);
        // lobbyQuery = lobbyQuery.WithKeyValue("version", ProjectSettings.GetSetting("config/version").AsString());
        var lobbies = await lobbyQuery.RequestAsync();
        return lobbies == null ? [] : lobbies.ToList();
    }

    private void UpdateLobbyData()
    {
        Friends.ClearAndFreeChildren();
        SteamId.Text = $"{Lobby.Id}";
        CurrentUserLabel.Text = $"{Lobby.MemberCount}";
        MaxUserLabel.Text = $"{Lobby.MaxMembers}";
        foreach (var lobbyMember in Lobby.Members)
        {
            if (Lobby.IsOwnedBy(lobbyMember.Id))
            {
                Log.Info($"房主是: {lobbyMember.Name}");
            }

            Friends.AddChild(SteamUserInfo.Instantiate(lobbyMember));
        }
    }

    public void Invite(SteamId steamId)
    {
        Lobby.InviteFriend(steamId);
    }
}