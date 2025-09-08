using Steamworks;

namespace Godot;

public struct SteamMessage
{
    public int PeerId { init; get; }
    public int TransferChannel { init; get; }
    public byte[] Data { init; get; }
    public SteamId SteamId => (ulong)PeerId + 0x0110000100000000UL;
}