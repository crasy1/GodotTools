using Steamworks;

namespace Godot;

public abstract partial class SteamSocket : SteamComponent
{
    /// <summary>
    /// steamId已连接
    /// </summary>
    [Signal]
    public delegate void ConnectedEventHandler(ulong steamId);

    /// <summary>
    /// steamId已断开
    /// </summary>
    [Signal]
    public delegate void DisconnectedEventHandler(ulong steamId);

    /// <summary>
    /// 收到来自 steamId的消息
    /// </summary>
    [Signal]
    public delegate void ReceiveMessageEventHandler(ulong steamId, string message);

    public abstract void Close();
}