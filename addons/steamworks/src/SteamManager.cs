using System;
using System.IO;
using System.Threading.Tasks;
using Steamworks;

namespace Godot;

/// <summary>
/// 封装steam api到godot
/// <seealso href="https://partner.steamgames.com/doc/features">steamworks文档</seealso>
/// </summary>
[SceneTree]
public partial class SteamManager : CanvasLayer
{
    public override void _Ready()
    {
        SteamUtil.InitEnvironment();
        SetVisible(SteamConfig.Debug);
        SteamInit();
        SClient.Instance.Connect();
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

            var lobby = SteamLobby.Instantiate((int)MaxLobbyUser.Value);
            LobbyInfo.AddChild(lobby);
            lobby.Create();
        };
        SearchLobby.Pressed += async () =>
        {
            var lobbies = await SteamLobby.Search();
            Log.Info(lobbies.Count);
        };
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

    private async Task OnSteamClientConnected()
    {
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
    }
    
}