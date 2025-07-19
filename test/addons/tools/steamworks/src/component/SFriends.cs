using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Steamworks;

namespace Godot;

[Singleton]
public partial class SFriends : SteamComponent
{
    
    public static readonly Dictionary<SteamId, Friend> Friends = new();
    public static Friend Me { private set; get; }

    public override void _Ready()
    {
        base._Ready();
        SteamFriends.OnChatMessage += (friend, s1, s2) => { Log.Info($"steam 收到聊天消息 {friend} {s1} {s2}"); };
        SteamFriends.OnClanChatMessage += (friend, s1, s2) => { Log.Info($"steam 群组收到聊天消息 {friend} {s1} {s2}"); };
        SteamFriends.OnFriendRichPresenceUpdate += (friend) => { Log.Info($"steam 好友状态更新 {friend}"); };
        SteamFriends.OnGameLobbyJoinRequested += (lobby, steamId) => { Log.Info($"steam 群组加入请求 {lobby} {steamId}"); };
        SteamFriends.OnGameOverlayActivated += (result) => { Log.Info($"steam 覆盖界面界面 {(result ? "激活" : "关闭")}"); };
        SteamFriends.OnGameRichPresenceJoinRequested += (friend, s) => { Log.Info($"steam 游戏状态加入请求 {friend} {s}"); };
        SteamFriends.OnGameServerChangeRequested += (s1, s2) => { Log.Info($"steam 游戏服务器改变请求 {s1} {s2}"); };
        SteamFriends.OnOverlayBrowserProtocol += (s) => { Log.Info($"steam 覆盖界面浏览器协议 {s}"); };
        SteamFriends.OnPersonaStateChange += (friend) =>
        {
            if (friend.IsMe)
            {
                Me = friend;
                Friends.TryAdd(friend.Id, Me);
            }
        };
        SClient.Instance.SteamClientConnected += () =>
        {
            foreach (var friend in SteamFriends.GetFriends())
            {
                Friends.Add(friend.Id, friend);
            }
        };
    }


    public static async Task<Image> Avatar(SteamId steamId, int size = 0)
    {
        var avatar = size switch
        {
            < 0 => await SteamFriends.GetSmallAvatarAsync(steamId),
            > 0 => await SteamFriends.GetLargeAvatarAsync(steamId),
            _ => await SteamFriends.GetMediumAvatarAsync(steamId)
        };

        if (!avatar.HasValue)
        {
            return null;
        }

        var image = avatar.Value;
        return SteamUtil.CreateImage(image);
    }
}