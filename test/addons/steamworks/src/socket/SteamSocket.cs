using Steamworks;

namespace Godot;

public abstract partial class SteamSocket : SteamComponent
{
    /// <summary>
    /// 收到来自 steamId的消息
    /// </summary>
    [Signal]
    public delegate void ReceiveMessageEventHandler(ulong steamId, string message);

    public abstract void Close();
}