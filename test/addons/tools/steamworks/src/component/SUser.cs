using System;
using Steamworks;

namespace Godot;

[Singleton]
public partial class SUser : SteamComponent
{

    public override void _Ready()
    {
        base._Ready();
        SteamUser.OnClientGameServerDeny += () => { Log.Info("游戏服务器已拒绝客户端连接"); };
        SteamUser.OnDurationControl += (durationControl) => { Log.Info($"游戏时长控制"); };
        SteamUser.OnGameWebCallback += (url) => { Log.Info($"游戏网页回调 {url}"); };
        SteamUser.OnLicensesUpdated += () => { Log.Info($"已更新授权"); };
        SteamUser.OnMicroTxnAuthorizationResponse += (appId, orderId, userAuthorized) =>
        {
            Log.Info($"用户响应微事务授权请求 {appId} {orderId} {userAuthorized}");
        };
        SteamUser.OnSteamServersConnected += () => { Log.Info($"已连接到Steam服务器"); };
        SteamUser.OnSteamServersDisconnected += () => { Log.Info($"已断开Steam服务器"); };
        SteamUser.OnSteamServerConnectFailure += () => { Log.Info($"Steam服务器连接失败"); };
        SteamUser.OnValidateAuthTicketResponse += (steamId, steamId2, authResponse) =>
        {
            Log.Info($"用户验证授权 {steamId} {steamId2} {authResponse}");
        };
    }

    public void GetInfo()
    {
        Log.Info($@"
----    {nameof(SteamUser)}    ----
SteamLevel:                         {SteamUser.SteamLevel}
SampleRate:                         {SteamUser.SampleRate}
IsBehindNAT:                        {SteamUser.IsBehindNAT}
IsPhoneIdentifying:                 {SteamUser.IsPhoneIdentifying}
IsPhoneVerified:                    {SteamUser.IsPhoneVerified}
IsPhoneRequiringVerification:       {SteamUser.IsPhoneRequiringVerification}
IsTwoFactorEnabled:                 {SteamUser.IsTwoFactorEnabled}
----    {nameof(SteamUser)}    ----
");
    }
}