using System;
using Steamworks;

namespace Godot;

public partial class SMatchmaking : SteamComponent
{
    private static readonly Lazy<SMatchmaking> LazyInstance = new(() => new());
    public static SMatchmaking Instance => LazyInstance.Value;

    private SMatchmaking()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        SteamMatchmaking.OnLobbyCreated += (result, lobby) => { Log.Info($"创建房间结果 {result}"); };
        SteamMatchmaking.OnLobbyInvite += (friend, lobby) => { Log.Info($"收到房间邀请 {friend} {lobby}"); };
        SteamMatchmaking.OnLobbyEntered += (lobby) => { Log.Info($"进入房间 {lobby}"); };
        SteamMatchmaking.OnLobbyGameCreated += (lobby, i, s, steamId) =>
        {
            Log.Info($"房间游戏已创建 {lobby} {i} {s} {steamId}");
        };
        SteamMatchmaking.OnLobbyDataChanged += (lobby) => { Log.Info($"房间数据已改变 {lobby}"); };
        SteamMatchmaking.OnChatMessage += (lobby, friend, message) =>
        {
            Log.Info($"房间聊天消息 {lobby} {friend} {message}");
        };
        SteamMatchmaking.OnLobbyMemberJoined += (lobby, friend) => { Log.Info($"房间成员加入 {lobby} {friend}"); };
        SteamMatchmaking.OnLobbyMemberLeave += (lobby, friend) => { Log.Info($"房间成员离开 {lobby} {friend}"); };
        SteamMatchmaking.OnLobbyMemberBanned += (lobby, friend, friend2) =>
        {
            Log.Info($"房间成员被禁言 {lobby} {friend} {friend2}");
        };
        SteamMatchmaking.OnLobbyMemberDataChanged += (lobby, friend) => { Log.Info($"房间成员数据改变 {lobby} {friend}"); };
        SteamMatchmaking.OnLobbyMemberDisconnected += (lobby, friend) => { Log.Info($"房间成员断开连接 {lobby} {friend}"); };
        SteamMatchmaking.OnLobbyMemberKicked += (lobby, friend, friend2) =>
        {
            Log.Info($"房间成员被踢 {lobby} {friend} {friend2}");
        };
    }
}