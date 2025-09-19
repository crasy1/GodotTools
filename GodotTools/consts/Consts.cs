namespace Godot;

public static class Consts
{
    public const float MinDb = -60f;
    public const float MaxDb = 0f;
    public const string ResDirPrefix = "res://";
    public const string UserDirPrefix = "user://";
    public const string AddonsDir = "res://addons";

    public const string DefaultBusLayoutPath = "res://default_bus_layout.tres";

    // 音频总线名字
    public const string BusVoice = "Voice";
    public const string BusMusic = "Music";
    public const string BusSfx = "SFX";
    public const string BusTeamVoice = "TeamVoice";

    // socket报文
    public const string SocketHandShake = "[SOCKET_HANDSHAKE]";
    public const string SocketHandShakeReply = "[SOCKET_HANDSHAKE_REPLY]";
    public const string SocketDisconnect = "[SOCKET_DISCONNECT]";
}