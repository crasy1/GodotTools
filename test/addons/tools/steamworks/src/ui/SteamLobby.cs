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
    private int MaxUser { set; get; }

    [OnInstantiate]
    private void Init(int maxUser)
    {
        MaxUser = maxUser;
    }

    public override void _Ready()
    {
        Hide();
        SteamMatchmaking.OnLobbyCreated += (result, lobby) =>
        {
            if (result == Result.OK)
            {
                UpdateLobbyData();
            }
        };
        SteamMatchmaking.OnLobbyInvite += (friend, lobby) => { Log.Info($"收到房间邀请 {friend} {lobby}"); };
        SteamMatchmaking.OnLobbyEntered += (lobby) => { UpdateLobbyData(); };
        SteamMatchmaking.OnLobbyGameCreated += (lobby, i, s, steamId) => { };
        SteamMatchmaking.OnLobbyDataChanged += (lobby) => { UpdateLobbyData(); };
        SteamMatchmaking.OnChatMessage += (lobby, friend, message) => { };
        SteamMatchmaking.OnLobbyMemberJoined += (lobby, friend) => { UpdateLobbyData(); };
        SteamMatchmaking.OnLobbyMemberLeave += (lobby, friend) => { UpdateLobbyData(); };
        SteamMatchmaking.OnLobbyMemberBanned += (lobby, friend, friend2) =>
        {
            Log.Info($"房间成员被禁言 {lobby} {friend} {friend2}");
        };
        SteamMatchmaking.OnLobbyMemberDataChanged += (lobby, friend) => { UpdateLobbyData(); };
        SteamMatchmaking.OnLobbyMemberDisconnected += (lobby, friend) => { UpdateLobbyData(); };
        SteamMatchmaking.OnLobbyMemberKicked += (lobby, friend, friend2) => { UpdateLobbyData(); };
        Joinable.Pressed += () => { SMatchmaking.Instance.Lobby?.SetJoinable(Joinable.ButtonPressed); };
        LobbyType.ItemSelected += index =>
        {
            switch (index)
            {
                case 0:
                    SMatchmaking.Instance.Lobby?.SetPrivate();
                    break;
                case 1:
                    SMatchmaking.Instance.Lobby?.SetFriendsOnly();
                    break;
                case 2:
                    SMatchmaking.Instance.Lobby?.SetPublic();
                    break;
                case 3:
                    SMatchmaking.Instance.Lobby?.SetInvisible();
                    break;
            }
        };
        Join.Pressed += async () =>
        {
            if (SMatchmaking.Instance.Lobby.HasValue)
            {
                var result = await SMatchmaking.Instance.Lobby.Value.Join();
                if (result == RoomEnter.Success)
                {
                    Log.Info("进入大厅");
                }
                else
                {
                    Log.Info($"进入大厅失败 {result}");
                }
            }
        };
        Exit.Pressed += () =>
        {
            SMatchmaking.Instance.LeaveLobby();
            Log.Info("退出大厅");
            QueueFree();
        };
    }

    public void Create()
    {
        SteamMatchmaking.CreateLobbyAsync(MaxUser);
    }

    public static async Task<List<Lobby>> Search(int minSlots = 1, int maxResult = 10)
    {
        var lobbyQuery = SteamMatchmaking.LobbyList.WithMaxResults(maxResult);
        lobbyQuery = lobbyQuery.WithSlotsAvailable(minSlots);
        // lobbyQuery = lobbyQuery.WithKeyValue("version",Project.Version);
        var lobbies = await lobbyQuery.RequestAsync();
        return lobbies == null ? [] : lobbies.ToList();
    }

    private void UpdateLobbyData()
    {
        Show();
        Friends.ClearAndFreeChildren();
        SteamId.Text = $"{SMatchmaking.Instance.Lobby?.Id}";
        CurrentUserLabel.Text = $"{SMatchmaking.Instance.Lobby?.MemberCount}";
        MaxUserLabel.Text = $"{SMatchmaking.Instance.Lobby?.MaxMembers}";
        if (SMatchmaking.Instance.Lobby.HasValue)
            foreach (var lobbyMember in SMatchmaking.Instance.Lobby.Value.Members)
            {
                if (SMatchmaking.Instance.Lobby.Value.IsOwnedBy(lobbyMember.Id))
                {
                    Log.Info($"房主是: {lobbyMember.Name}");
                }

                Friends.AddChild(SteamUserInfo.Instantiate(lobbyMember));
            }
    }
}