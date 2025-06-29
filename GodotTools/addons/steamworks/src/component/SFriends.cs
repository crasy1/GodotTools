using System;
using System.Threading.Tasks;
using Steamworks;

namespace Godot;

public partial class SFriends : SteamComponent
{
    private static readonly Lazy<SFriends> LazyInstance = new(() => new());
    public static SFriends Instance => LazyInstance.Value;

    private SFriends()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        SteamFriends.OnChatMessage += (friend, s1, s2) => { };
        SteamFriends.OnClanChatMessage += (friend, s1, s2) => { };
        SteamFriends.OnFriendRichPresenceUpdate += (friend) => { };
        SteamFriends.OnGameLobbyJoinRequested += (lobby, steamId) => { };
        SteamFriends.OnGameOverlayActivated += (result) => { };
        // TODO
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
        return Image.CreateFromData((int)image.Width, (int)image.Height, false, Image.Format.Rgba8, image.Data);
    }
}