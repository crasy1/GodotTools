using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Steamworks;

namespace Godot;

/// <summary>
/// 封装steam api到godot
/// <seealso href="https://partner.steamgames.com/doc/features">steamworks文档</seealso>
/// </summary>
[SceneTree]
[Singleton]
public partial class SteamManager : CanvasLayer
{
    /**
         * 退出前的操作
         */
    private static readonly Stack<Action> QuitActions = new();

    public override void _Ready()
    {
        GetTree().AutoAcceptQuit = false;
        SteamUtil.InitEnvironment();
        SetVisible(SteamConfig.Debug);
        SteamInit();
        if (SteamConfig.CallbackDebug)
        {
            Dispatch.OnDebugCallback += (type, message, server) =>
            {
                Log.Debug($"steamworks 调试回调信息 [Callback {type} {(server ? "server" : "client")}]: {message}");
            };
        }

        Dispatch.OnException += (e) => { Log.Error($"steamworks 异常", e); };
        SClient.Instance.Connect();
        SServer.Instance.StartServer("mod","des");
        Ping.Hide();
        ScreenShot.Pressed += () =>
        {
            SteamScreenshots.TriggerScreenshot();
            Log.Info("截屏");
        };
        WriteCloud.Pressed += () =>
        {
            var writeToCloud = SRemoteStorage.Instance.WriteToCloud("test.txt", "test123你好");
            Log.Info($"上传到云端 {writeToCloud}");
        };
        ReadCloud.Pressed += () =>
        {
            SRemoteStorage.Instance.GetInfo();
            var readFromCloud = SRemoteStorage.Instance.ReadFromCloud("test.txt");
            Log.Info($"从云端读取到 {readFromCloud}");
        };
        ShowFriends.Pressed += () =>
        {
            FriendsScrollContainer.Visible = true;
            AchievementsScrollContainer.Visible = false;
        };
        ShowAchievements.Pressed += () =>
        {
            FriendsScrollContainer.Visible = false;
            AchievementsScrollContainer.Visible = true;
        };
        AchievementsScrollContainer.Visible = false;
        OpenStore.Pressed += () => { SteamFriends.OpenStoreOverlay(SteamConfig.AppId); };
        OpenUrl.Pressed += () => { SteamFriends.OpenWebOverlay("https://www.baidu.com"); };
        OpenSettings.Pressed += () => { SteamFriends.OpenOverlay("settings"); };
        OpenFriends.Pressed += () => { SteamFriends.OpenOverlay("friends"); };
        OpenPlayers.Pressed += () => { SteamFriends.OpenOverlay("players"); };
        OpenCommunity.Pressed += () => { SteamFriends.OpenOverlay("community"); };
        OpenStats.Pressed += () => { SteamFriends.OpenOverlay("stats"); };
        OpenOfficalGameGroup.Pressed += () => { SteamFriends.OpenOverlay("officalgamegroup"); };
        OpenAchievements.Pressed += () => { SteamFriends.OpenOverlay("achievements"); };
        CreateLobby.Pressed += () =>
        {
            if (LobbyInfo.GetChildCount() > 0)
            {
                Log.Info("大厅已存在");
                return;
            }

            var lobby = SteamLobby.Instantiate();
            LobbyInfo.AddChild(lobby);
        };
        SearchLobby.Pressed += async () =>
        {
            var lobbies = await SteamLobby.Search();
            Log.Info(lobbies.Count);
        };
        SUserStats.AchievementUnlocked += (v) =>
        {
            Achievements.ClearAndFreeChildren();
            foreach (var achievement in SteamUserStats.Achievements)
            {
                var achievementUi = AchievementUi.Instantiate(achievement);
                Achievements.AddChild(achievementUi);
            }
        };
        Test.Pressed += () =>
        {
            GetTree().ChangeSceneToFile(SteamTest.TscnFilePath);
            Hide();
        };
    }

    public override void _Notification(int what)
    {
        if (NotificationWMCloseRequest == what)
        {
            Log.Info("用户请求退出游戏");
            BeforeGameQuit();
            GetTree().Quit();
        }

        if (NotificationWMGoBackRequest == what)
        {
            Log.Info("android用户请求退出游戏");
            BeforeGameQuit();
            GetTree().Quit();
        }
    }

    private void SteamInit()
    {
        var components = new Node();
        components.Name = $"{nameof(SteamComponent)}s";
        AddChild(components);
        SClient.Instance.SteamClientConnected += async () => await OnSteamClientConnected();
        SClient.Instance.SteamClientDisconnected += OnSteamClientDisconnected;
        components.AddChild(SClient.Instance);
        components.AddChild(SServer.Instance);
        components.AddChild(SApp.Instance);
        components.AddChild(SFriends.Instance);
        components.AddChild(SInput.Instance);
        components.AddChild(SInventory.Instance);
        components.AddChild(SMatchmaking.Instance);
        components.AddChild(SMatchmakingServers.Instance);
        components.AddChild(SMusic.Instance);
        components.AddChild(SNetworking.Instance);
        components.AddChild(SNetworkingSockets.Instance);
        components.AddChild(SNetworkingUtils.Instance);
        components.AddChild(SParental.Instance);
        components.AddChild(SParties.Instance);
        components.AddChild(SRemotePlay.Instance);
        components.AddChild(SRemoteStorage.Instance);
        components.AddChild(SScreenshots.Instance);
        components.AddChild(SServerStats.Instance);
        components.AddChild(STimeline.Instance);
        components.AddChild(SUgc.Instance);
        components.AddChild(SUser.Instance);
        components.AddChild(SUserStats.Instance);
        components.AddChild(SUtil.Instance);
    }

    private void OnSteamClientDisconnected()
    {
        Connected.Text = "未连接";
    }

    public static void AddBeforeGameQuitAction(Action action)
    {
        QuitActions.Push(action);
    }

    private void BeforeGameQuit()
    {
        Log.Info($"退出游戏前逻辑共{QuitActions.Count}个");
        var count = 0;
        for (int i = 0; i < QuitActions.Count; i++)
        {
            if (QuitActions.TryPop(out var action))
            {
                count++;
                Log.Info($"开始执行第{count}个 => {action.Target?.GetType().Name}");
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    Log.Error($"执行第{count}个异常 => {e.Message}");
                }

                Log.Info($"<= 结束执行第{count}个");
            }
        }

        Log.Info("退出游戏前逻辑执行完毕");
    }

    private async Task OnSteamClientConnected()
    {
        var ticket = SteamUser.GetAuthSessionTicket(SteamClient.SteamId);
        var ticketData = ticket.Data;
        var steamId = SteamClient.SteamId;
        var beginAuthSession = SteamServer.BeginAuthSession(ticketData, steamId);
        SteamFriends.ListenForFriendsMessages = true;
        Connected.Text = "已连接";
        UserInfo.Text = $@"
AppId:                              {SteamClient.AppId}
SteamId:                            {SteamClient.SteamId}
Name:                               {SteamClient.Name}
State:                              {SteamClient.State}
SteamLevel:                         {SteamUser.SteamLevel}
SampleRate:                         {SteamUser.SampleRate}
IsBehindNAT:                        {SteamUser.IsBehindNAT}
IsPhoneIdentifying:                 {SteamUser.IsPhoneIdentifying}
IsPhoneVerified:                    {SteamUser.IsPhoneVerified}
IsPhoneRequiringVerification:       {SteamUser.IsPhoneRequiringVerification}
IsTwoFactorEnabled:                 {SteamUser.IsTwoFactorEnabled}
";
        var image = await SFriends.Avatar(SteamClient.SteamId);
        Avatar.Texture = image.Texture();
        foreach (var friend in SteamFriends.GetFriends())
        {
            if (friend.IsOnline)
            {
                var lobby = friend.GameInfo?.Lobby;
                if (lobby.HasValue)
                {
                    Log.Info($"好友在大厅中,{lobby.Value.Id}");
                }

                var steamUserInfo = SteamUserInfo.Instantiate(friend);
                Friends.AddChild(steamUserInfo);
            }
        }

        foreach (var achievement in SteamUserStats.Achievements)
        {
            var achievementUi = AchievementUi.Instantiate(achievement);
            Achievements.AddChild(achievementUi);
        }
    }
}