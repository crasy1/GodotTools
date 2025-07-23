using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace Godot;

[Singleton]
public partial class SMatchmaking : SteamComponent
{
    public int MaxUser { set; get; } = 4;
    public Lobby? Lobby { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        SteamMatchmaking.OnLobbyCreated += (result, lobby) =>
        {
            Log.Info($"创建房间结果 {result}");
            if (result == Result.OK)
            {
                Lobby = lobby;
                // 确保只能搜到客户端版本一致的大厅
                var update = Lobby?.SetData(nameof(Project.Version), Project.Version);
                Log.Info($"更新lobby {nameof(Project.Version)} {Project.Version} {update}");
            }
        };
        SteamMatchmaking.OnLobbyInvite += (friend, lobby) =>
        {
            Log.Info($"收到房间邀请 {friend} {lobby.Id}");
            Lobby = lobby;
        };
        SteamMatchmaking.OnLobbyEntered += (lobby) =>
        {
            Log.Info($"进入房间 {lobby.Id}");
            Lobby = lobby;
        };
        SteamMatchmaking.OnLobbyGameCreated += (lobby, i, s, steamId) =>
        {
            Log.Info($"房间游戏已创建 {lobby.Id} {i} {s} {steamId}");
        };
        SteamMatchmaking.OnLobbyDataChanged += (lobby) =>
        {
            Log.Info($"房间数据已改变 {lobby.Id}");
            Lobby = lobby;
        };
        SteamMatchmaking.OnChatMessage += (lobby, friend, message) =>
        {
            Log.Info($"房间聊天消息 {lobby.Id} {friend} {message}");
            Lobby = lobby;
        };
        SteamMatchmaking.OnLobbyMemberJoined += (lobby, friend) =>
        {
            Log.Info($"房间成员加入 {lobby.Id} {friend}");
            Lobby = lobby;
        };
        SteamMatchmaking.OnLobbyMemberLeave += (lobby, friend) =>
        {
            Log.Info($"房间成员离开 {lobby.Id} {friend}");
            Lobby = lobby;
        };
        SteamMatchmaking.OnLobbyMemberBanned += (lobby, friend, friend2) =>
        {
            Log.Info($"房间成员被禁言 {lobby.Id} {friend} {friend2}");
        };
        SteamMatchmaking.OnLobbyMemberDataChanged += (lobby, friend) =>
        {
            Log.Info($"房间成员数据改变 {lobby.Id} {friend}");
            Lobby = lobby;
        };
        SteamMatchmaking.OnLobbyMemberDisconnected += (lobby, friend) =>
        {
            Log.Info($"房间成员断开连接 {lobby.Id} {friend}");
            Lobby = lobby;
        };
        SteamMatchmaking.OnLobbyMemberKicked += (lobby, friend, friend2) =>
        {
            Log.Info($"房间成员被踢 {lobby.Id} {friend} {friend2}");
            Lobby = lobby;
        };
    }

    public void CreateLobby()
    {
        SteamMatchmaking.CreateLobbyAsync(MaxUser);
    }

    public void LeaveLobby()
    {
        Lobby?.Leave();
        Lobby = null;
    }

    public static async Task<List<Lobby>> Search(int minSlots = 1, int maxResult = 10)
    {
        var lobbyQuery = SteamMatchmaking.LobbyList.WithMaxResults(maxResult);
        lobbyQuery = lobbyQuery.WithSlotsAvailable(minSlots);
        lobbyQuery = lobbyQuery.WithKeyValue(nameof(Project.Version), Project.Version);
        var lobbies = await lobbyQuery.RequestAsync();
        return lobbies == null ? [] : lobbies.ToList();
    }

    public void Invite(SteamId steamId)
    {
        Lobby?.InviteFriend(steamId);
    }
}