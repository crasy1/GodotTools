using Steamworks;

namespace Godot;

public struct SteamworksMessagePacket
{
    public int TransferChannel { init; get; }
    public byte[] Data { init; get; }
    public SteamId SteamId { init; get; }
    public int PeerId => (int)SteamId.AccountId;
}