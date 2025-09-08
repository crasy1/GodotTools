using System;
using System.Linq;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 相当于steamworks socket server的包装
/// </summary>
public partial class SteamworksServerPeer : MultiplayerPeerExtension
{
    private SteamworksServerPeer(PeerSocketManager peerSocketManager, SocketManager socketManager)
    {
        PeerSocketManager = peerSocketManager;
        SocketManager = socketManager;
    }

    private PeerSocketManager PeerSocketManager { get; }
    private SocketManager SocketManager { get; }

    private int TargetPeer { set; get; }
    private SteamMessage? LastPacket { set; get; }

    /// <summary>
    /// 创建服务端
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public static SteamworksServerPeer CreateServer(int port)
    {
        try
        {
            var peerSocketManager = new PeerSocketManager();
            var socketManager = SteamNetworkingSockets.CreateRelaySocket(port, peerSocketManager);
            var steamworksPeer = new SteamworksServerPeer(peerSocketManager, socketManager);
            return steamworksPeer;
        }
        catch (Exception e)
        {
            Log.Error($"创建{nameof(SteamworksServerPeer)} 异常, {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// <para>当多人游戏对等体应该立即关闭时调用（参见 <see cref="Godot.MultiplayerPeer.Close()"/>）。</para>
    /// </summary>
    public override void _Close()
    {
        SocketManager.Close();
    }

    /// <summary>
    /// <para>当应该强制断开连接的 <paramref name="pPeer"/> 时调用（参见 <see cref="Godot.MultiplayerPeer.DisconnectPeer(int, bool)"/>）。</para>
    /// </summary>
    public override void _DisconnectPeer(int pPeer, bool pForce)
    {
        var connection = PeerSocketManager.Connections.FirstOrDefault(kv => (uint)pPeer == kv.Value.AccountId).Key;
        connection.Close();
    }

    /// <summary>
    /// <para>当 <see cref="Godot.MultiplayerApi"/> 内部请求可用数据包计数时调用。</para>
    /// </summary>
    public override int _GetAvailablePacketCount() => PeerSocketManager.PacketQueue.Count;

    /// <summary>
    /// <para>当请求 <see cref="Godot.MultiplayerPeer"/> 上的连接状态时调用（参见 <see cref="Godot.MultiplayerPeer.GetConnectionStatus()"/>）。</para>
    /// </summary>
    public override ConnectionStatus _GetConnectionStatus() => PeerSocketManager.ConnectionStatus;

    /// <summary>
    /// <para>当 <see cref="Godot.MultiplayerApi"/> 请求最大允许的数据包大小（以字节为单位）时调用。</para>
    /// </summary>
    public override int _GetMaxPacketSize() => 52 * 1024;

    /// <summary>
    /// <para>调用以获取接收到下一个可用数据包的通道。参见 <see cref="Godot.MultiplayerPeer.GetPacketChannel()"/>。</para>
    /// </summary>
    public override int _GetPacketChannel()
    {
        if (PeerSocketManager.PacketQueue.TryPeek(out var packet))
        {
            return packet.TransferChannel;
        }

        return default;
    }

    /// <summary>
    /// <para>调用以获取远程对等体用于发送下一个可用数据包的传输模式。参见 <see cref="Godot.MultiplayerPeer.GetPacketMode()"/>。</para>
    /// </summary>
    public override TransferModeEnum _GetPacketMode()
    {
        return default;
    }

    /// <summary>
    /// <para>当请求发送最近数据包的 <see cref="Godot.MultiplayerPeer"/> 的 ID 时调用（参见 <see cref="Godot.MultiplayerPeer.GetPacketPeer()"/>）。</para>
    /// </summary>
    public override int _GetPacketPeer()
    {
        if (PeerSocketManager.PacketQueue.TryPeek(out var packet))
        {
            return packet.PeerId;
        }

        return default;
    }

    /// <summary>
    /// <para>当 <see cref="Godot.MultiplayerApi"/> 需要接收数据包时调用，如果未实现 <c>_get_packet</c>。通过 GDScript 扩展此类时使用。</para>
    /// </summary>
    public override byte[] _GetPacketScript()
    {
        if (PeerSocketManager.PacketQueue.Count <= 0)
        {
            return null;
        }

        LastPacket = PeerSocketManager.PacketQueue.Dequeue();
        return LastPacket.Value.Data;
    }

    /// <summary>
    /// <para>当在此 <see cref="Godot.MultiplayerPeer"/> 上读取传输通道时调用（参见 <see cref="Godot.MultiplayerPeer.TransferChannel"/>）。</para>
    /// </summary>
    public override int _GetTransferChannel() => TransferChannel;

    /// <summary>
    /// <para>当在此 <see cref="Godot.MultiplayerPeer"/> 上读取传输模式时调用（参见 <see cref="Godot.MultiplayerPeer.TransferMode"/>）。</para>
    /// </summary>
    public override TransferModeEnum _GetTransferMode() => TransferMode;

    /// <summary>
    /// <para>当请求此 <see cref="Godot.MultiplayerPeer"/> 的唯一 ID 时调用（参见 <see cref="Godot.MultiplayerPeer.GetUniqueId()"/>）。该值必须在 <c>1</c> 和 <c>2147483647</c> 之间。</para>
    /// </summary>
    public override int _GetUniqueId()
    {
        return (int)SteamClient.SteamId.AccountId;
    }

    /// <summary>
    /// <para>当在此 <see cref="Godot.MultiplayerPeer"/> 上请求"拒绝新连接"状态时调用（参见 <see cref="Godot.MultiplayerPeer.RefuseNewConnections"/>）。</para>
    /// </summary>
    public override bool _IsRefusingNewConnections() => RefuseNewConnections;

    /// <summary>
    /// <para>当在 <see cref="Godot.MultiplayerApi"/> 上请求"是否为服务器"状态时调用。参见 <see cref="Godot.MultiplayerApi.IsServer()"/>。</para>
    /// </summary>
    public override bool _IsServer() => true;

    /// <summary>
    /// <para>调用以检查服务器在当前配置中是否可以充当中继。参见 <see cref="Godot.MultiplayerPeer.IsServerRelaySupported()"/>。</para>
    /// </summary>
    public override bool _IsServerRelaySupported()
    {
        return true;
    }

    /// <summary>
    /// <para>当 <see cref="Godot.MultiplayerApi"/> 被轮询时调用。参见 <see cref="Godot.MultiplayerApi.Poll()"/>。</para>
    /// </summary>
    public override void _Poll()
    {
        SocketManager.Receive();
    }

    /// <summary>
    /// <para>当 <see cref="Godot.MultiplayerApi"/> 需要发送数据包时调用，如果未实现 <c>_put_packet</c>。通过 GDScript 扩展此类时使用。</para>
    /// </summary>
    public override Error _PutPacketScript(byte[] pBuffer)
    {
        try
        {
            var sendType = TransferMode switch
            {
                TransferModeEnum.Reliable => SendType.Reliable,
                TransferModeEnum.Unreliable => SendType.Unreliable,
                TransferModeEnum.UnreliableOrdered => SendType.NoNagle,
                _ => SendType.Reliable
            };
            var connections = PeerSocketManager.Connections;
            if (TargetPeer == 0)
            {
                foreach (var connection in connections.Keys)
                {
                    connection.SendMessage(pBuffer, sendType, (ushort)TransferChannel);
                }
            }
            else if (TargetPeer > 1)
            {
                foreach (var (connection, steamId) in connections)
                {
                    if (steamId.AccountId == TargetPeer)
                    {
                        connection.SendMessage(pBuffer, sendType, (ushort)TransferChannel);
                    }
                }
            }
            else if (TargetPeer < 0)
            {
                foreach (var (connection, steamId) in connections)
                {
                    if (steamId.AccountId != -TargetPeer)
                    {
                        connection.SendMessage(pBuffer, sendType, (ushort)TransferChannel);
                    }
                }
            }

            return Error.Ok;
        }
        catch (Exception e)
        {
            Log.Error($"{nameof(SteamworksServerPeer)} 发送数据包异常, {e.Message}");
            return Error.Failed;
        }
    }

    /// <summary>
    /// <para>当在此 <see cref="Godot.MultiplayerPeer"/> 上设置"拒绝新连接"状态时调用（参见 <see cref="Godot.MultiplayerPeer.RefuseNewConnections"/>）。</para>
    /// </summary>
    public override void _SetRefuseNewConnections(bool pEnable) => RefuseNewConnections = pEnable;

    /// <summary>
    /// <para>当为此 <see cref="Godot.MultiplayerPeer"/> 设置目标对等体时调用（参见 <see cref="Godot.MultiplayerPeer.SetTargetPeer(int)"/>）。</para>
    /// </summary>
    public override void _SetTargetPeer(int pPeer)
    {
        TargetPeer = pPeer;
        SetTargetPeer(pPeer);
    }

    /// <summary>
    /// <para>当为此 <see cref="Godot.MultiplayerPeer"/> 设置要使用的通道时调用（参见 <see cref="Godot.MultiplayerPeer.TransferChannel"/>）。</para>
    /// </summary>
    public override void _SetTransferChannel(int pChannel) => TransferChannel = pChannel;

    /// <summary>
    /// <para>当在此 <see cref="Godot.MultiplayerPeer"/> 上设置传输模式时调用（参见 <see cref="Godot.MultiplayerPeer.TransferMode"/>）。</para>
    /// </summary>
    public override void _SetTransferMode(TransferModeEnum pMode) => TransferMode = pMode;
}