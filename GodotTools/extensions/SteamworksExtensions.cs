using Steamworks;

namespace Godot;

public static class SteamworksExtensions
{
    private static readonly Dictionary<FriendState, string> FriendStates = new()
    {
        [FriendState.Offline] = "离线",
        [FriendState.Online] = "在线",
        [FriendState.Busy] = "忙碌",
        [FriendState.Away] = "离开",
        [FriendState.Snooze] = "勿扰",
        [FriendState.LookingToTrade] = "交易",
        [FriendState.LookingToPlay] = "游玩",
        [FriendState.Invisible] = "隐身",
    };

    /// <summary>
    /// steamworks image to godot image
    /// </summary>
    /// <param name="steamworksImage"></param>
    /// <param name="useMipmaps"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static Image GodotImage(this Steamworks.Data.Image steamworksImage, bool useMipmaps = false,
        Image.Format format = Image.Format.Rgba8)
    {
        return Image.CreateFromData((int)steamworksImage.Width, (int)steamworksImage.Height, useMipmaps,
            format,
            steamworksImage.Data);
    }

    public static string State(this FriendState friendState) => FriendStates[friendState];
}