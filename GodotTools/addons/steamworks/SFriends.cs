using System;
using System.Threading.Tasks;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks;

public partial class SFriends : SteamComponent
{
    private static readonly Lazy<SFriends> LazyInstance = new(() => new());
    public static SFriends Instance => LazyInstance.Value;

    private SFriends()
    {
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