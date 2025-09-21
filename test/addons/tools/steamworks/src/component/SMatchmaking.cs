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
    public static Lobby? Lobby { get; private set; }

    public const string KickMsg = "KICK:";

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
            // 添加steam状态
            SFriends.Instance.DisplayCustom("在大厅中");
            SFriends.Instance.ShowGroup(lobby.Id.ToString(), lobby.MemberCount);
            // 添加队伍语音
            foreach (var lobbyMember in lobby.Members)
            {
                if (!lobbyMember.IsMe)
                {
                    TeamVoice.Instance.AddTeamMember(lobbyMember.Id);
                }
            }
        };
        SteamMatchmaking.OnLobbyGameCreated += (lobby, i, s, steamId) =>
        {
            SFriends.Instance.DisplayCustom("在游戏中");
            SFriends.Instance.ShowGroup(lobby.Id.ToString(), lobby.MemberCount);
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
            if (message.StartsWith(KickMsg))
            {
                if (message.Replace(KickMsg, "") == SteamClient.SteamId.ToString())
                {
                    Log.Info("被踢出房间");
                    LeaveLobby();
                }
            }

            Lobby = lobby;
        };
        SteamMatchmaking.OnLobbyMemberJoined += (lobby, friend) =>
        {
            Log.Info($"房间成员加入 {lobby.Id} {friend}");
            Lobby = lobby;
            // 改变steam状态
            SFriends.Instance.ShowGroup(lobby.Id.ToString(), lobby.MemberCount);
            // 添加队伍语音
            TeamVoice.Instance.AddTeamMember(friend.Id);
        };
        SteamMatchmaking.OnLobbyMemberLeave += (lobby, friend) =>
        {
            Log.Info($"房间成员离开 {lobby.Id} {friend}");
            Lobby = lobby;
            // 改变steam状态
            SFriends.Instance.ShowGroup(lobby.Id.ToString(), lobby.MemberCount);
            // 移除队伍语音
            TeamVoice.Instance.RemoveTeamMember(friend.Id);
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
            SFriends.Instance.ShowGroup(lobby.Id.ToString(), lobby.MemberCount);
        };
        SteamMatchmaking.OnLobbyMemberKicked += (lobby, friend, friend2) =>
        {
            Log.Info($"房间成员被踢 {lobby.Id} {friend} {friend2}");
            Lobby = lobby;
            SFriends.Instance.ShowGroup(lobby.Id.ToString(), lobby.MemberCount);
        };
    }

    /// <summary>
    /// 创建大厅
    /// </summary>
    /// <param name="maxUser"></param>
    /// <returns></returns>
    public static async Task<Lobby?> CreateLobbyAsync(int maxUser = 4)
    {
        return await SteamMatchmaking.CreateLobbyAsync(maxUser);
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    /// <param name="lobby"></param>
    /// <returns></returns>
    public static async Task<bool> JoinLobbyAsync(Lobby lobby)
    {
        return await lobby.Join() == RoomEnter.Success;
    }

    /// <summary>
    /// 离开大厅
    /// </summary>
    public static void LeaveLobby()
    {
        Lobby?.Leave();
        Lobby = null;
        Log.Info("退出大厅");
        // 移除steam状态
        SFriends.Instance.CloseDisplay();
        SFriends.Instance.CloseGroup();
        // 退出队伍语音
        TeamVoice.Instance.RemoveAllTeamMember();
    }

    public static async Task<List<Lobby>> Search(int minSlots = 1, int maxResult = 10,
        Dictionary<string, string>? lobbyData = null)
    {
        var lobbyQuery = SteamMatchmaking.LobbyList.WithMaxResults(maxResult);
        lobbyQuery = lobbyQuery.WithSlotsAvailable(minSlots);
        lobbyQuery = lobbyQuery.WithKeyValue(nameof(Project.Version), Project.Version);
        lobbyData?.Keys.ToList().ForEach(key => lobbyQuery = lobbyQuery.WithKeyValue(key, lobbyData[key]));
        var lobbies = await lobbyQuery.RequestAsync();
        return lobbies == null ? [] : lobbies.ToList();
    }

    /// <summary>
    /// 邀请玩家
    /// </summary>
    /// <param name="steamId"></param>
    public static void Invite(SteamId steamId)
    {
        Lobby?.InviteFriend(steamId);
    }

    /// <summary>
    /// 踢掉玩家
    /// </summary>
    /// <param name="steamId"></param>
    public static void Kick(SteamId steamId)
    {
        if (!Lobby.HasValue)
        {
            return;
        }

        var lobby = Lobby.Value;
        if (lobby.IsOwnedBy(SteamClient.SteamId))
        {
            foreach (var member in lobby.Members)
            {
                if (member.Id == steamId)
                {
                    lobby.SendChatString($"{KickMsg}{steamId}");
                }
            }
        }
    }
}