using System;
using System.Linq;
using Steamworks;

namespace Godot.multiplayer;

/// <summary>
/// 相当于steamworks socket的包装
/// </summary>
public partial class SteamworksPeer : MultiplayerPeerExtension
{
    private SteamworksPeer(PeerConnectionManager peerConnectionManager, ConnectionManager connectionManager)
    {
        PeerConnectionManager = peerConnectionManager;
        ConnectionManager = connectionManager;
    }

    private SteamworksPeer(PeerSocketManager peerSocketManager, SocketManager socketManager)
    {
        PeerSocketManager = peerSocketManager;
        SocketManager = socketManager;
        IsServer = true;
    }

    private PeerConnectionManager PeerConnectionManager { set; get; }
    private ConnectionManager ConnectionManager { set; get; }
    private PeerSocketManager PeerSocketManager { set; get; }
    private SocketManager SocketManager { set; get; }

    private bool IsServer { set; get; }

    /// <summary>
    /// 创建客户端
    /// </summary>
    /// <param name="steamId"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public static SteamworksPeer CreateClient(SteamId steamId, int port)
    {
        try
        {
            var peerConnectionManager = new PeerConnectionManager();
            var connectionManager = SteamNetworkingSockets.ConnectRelay(steamId, port, peerConnectionManager);
            var steamworksPeer = new SteamworksPeer(peerConnectionManager, connectionManager);
            return steamworksPeer;
        }
        catch (Exception e)
        {
            Log.Error($"创建{nameof(SteamworksPeer)} client 异常, {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// 创建服务端
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public static SteamworksPeer CreateServer(int port)
    {
        try
        {
            var peerSocketManager = new PeerSocketManager();
            var socketManager = SteamNetworkingSockets.CreateRelaySocket(port, peerSocketManager);
            var steamworksPeer = new SteamworksPeer(peerSocketManager, socketManager);
            return steamworksPeer;
        }
        catch (Exception e)
        {
            Log.Error($"创建{nameof(SteamworksPeer)} server 异常, {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// <para>当多人游戏对等体应该立即关闭时调用（参见 <see cref="Godot.MultiplayerPeer.Close()"/>）。</para>
    /// </summary>
    public override void _Close()
    {
        if (IsServer)
        {
            SocketManager.Close();
        }
        else
        {
            ConnectionManager.Close();
        }
    }

    /// <summary>
    /// <para>当应该强制断开连接的 <paramref name="pPeer"/> 时调用（参见 <see cref="Godot.MultiplayerPeer.DisconnectPeer(int, bool)"/>）。</para>
    /// </summary>
    public override void _DisconnectPeer(int pPeer, bool pForce)
    {
        if (IsServer)
        {
            var connection = PeerSocketManager.Connections.FirstOrDefault(kv => (uint)pPeer == kv.Value.AccountId).Key;
            connection.Close();
        }
        else
        {
            ConnectionManager.Close();
        }
    }

    /// <summary>
    /// <para>当 <see cref="Godot.MultiplayerApi"/> 内部请求可用数据包计数时调用。</para>
    /// </summary>
    public override int _GetAvailablePacketCount()
    {
        if (IsServer)
        {
        }

        return default;
    }

    /// <summary>
    /// <para>当请求 <see cref="Godot.MultiplayerPeer"/> 上的连接状态时调用（参见 <see cref="Godot.MultiplayerPeer.GetConnectionStatus()"/>）。</para>
    /// </summary>
    public override ConnectionStatus _GetConnectionStatus()
    {
        if (IsServer)
        {
            return PeerSocketManager.ConnectionStatus;
        }
        else
        {
            return PeerConnectionManager.ConnectionStatus;
        }
    }

    /// <summary>
    /// <para>当 <see cref="Godot.MultiplayerApi"/> 请求最大允许的数据包大小（以字节为单位）时调用。</para>
    /// </summary>
    public override int _GetMaxPacketSize()
    {
        return default;
    }

    /// <summary>
    /// <para>调用以获取接收到下一个可用数据包的通道。参见 <see cref="Godot.MultiplayerPeer.GetPacketChannel()"/>。</para>
    /// </summary>
    public override int _GetPacketChannel()
    {
        return default;
    }

    /// <summary>
    /// <para>调用以获取远程对等体用于发送下一个可用数据包的传输模式。参见 <see cref="Godot.MultiplayerPeer.GetPacketMode()"/>。</para>
    /// </summary>
    public override MultiplayerPeer.TransferModeEnum _GetPacketMode()
    {
        return default;
    }

    /// <summary>
    /// <para>当请求发送最近数据包的 <see cref="Godot.MultiplayerPeer"/> 的 ID 时调用（参见 <see cref="Godot.MultiplayerPeer.GetPacketPeer()"/>）。</para>
    /// </summary>
    public override int _GetPacketPeer()
    {
        return default;
    }

    /// <summary>
    /// <para>当 <see cref="Godot.MultiplayerApi"/> 需要接收数据包时调用，如果未实现 <c>_get_packet</c>。通过 GDScript 扩展此类时使用。</para>
    /// </summary>
    public override byte[] _GetPacketScript()
    {
        return default;
    }

    /// <summary>
    /// <para>当在此 <see cref="Godot.MultiplayerPeer"/> 上读取传输通道时调用（参见 <see cref="Godot.MultiplayerPeer.TransferChannel"/>）。</para>
    /// </summary>
    public override int _GetTransferChannel()
    {
        return default;
    }

    /// <summary>
    /// <para>当在此 <see cref="Godot.MultiplayerPeer"/> 上读取传输模式时调用（参见 <see cref="Godot.MultiplayerPeer.TransferMode"/>）。</para>
    /// </summary>
    public override MultiplayerPeer.TransferModeEnum _GetTransferMode()
    {
        return default;
    }

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
    public override bool _IsRefusingNewConnections()
    {
        return default;
    }

    /// <summary>
    /// <para>当在 <see cref="Godot.MultiplayerApi"/> 上请求"是否为服务器"状态时调用。参见 <see cref="Godot.MultiplayerApi.IsServer()"/>。</para>
    /// </summary>
    public override bool _IsServer() => IsServer;

    /// <summary>
    /// <para>调用以检查服务器在当前配置中是否可以充当中继。参见 <see cref="Godot.MultiplayerPeer.IsServerRelaySupported()"/>。</para>
    /// </summary>
    public override bool _IsServerRelaySupported()
    {
        return IsServer;
    }

    /// <summary>
    /// <para>当 <see cref="Godot.MultiplayerApi"/> 被轮询时调用。参见 <see cref="Godot.MultiplayerApi.Poll()"/>。</para>
    /// </summary>
    public override void _Poll()
    {
        if (IsServer)
        {
            SocketManager.Receive();
        }
        else
        {
            ConnectionManager.Receive();
        }
    }

    /// <summary>
    /// <para>当 <see cref="Godot.MultiplayerApi"/> 需要发送数据包时调用，如果未实现 <c>_put_packet</c>。通过 GDScript 扩展此类时使用。</para>
    /// </summary>
    public override Error _PutPacketScript(byte[] pBuffer)
    {
        return default;
    }

    /// <summary>
    /// <para>当在此 <see cref="Godot.MultiplayerPeer"/> 上设置"拒绝新连接"状态时调用（参见 <see cref="Godot.MultiplayerPeer.RefuseNewConnections"/>）。</para>
    /// </summary>
    public override void _SetRefuseNewConnections(bool pEnable)
    {
    }

    /// <summary>
    /// <para>当为此 <see cref="Godot.MultiplayerPeer"/> 设置目标对等体时调用（参见 <see cref="Godot.MultiplayerPeer.SetTargetPeer(int)"/>）。</para>
    /// </summary>
    public override void _SetTargetPeer(int pPeer)
    {
    }

    /// <summary>
    /// <para>当为此 <see cref="Godot.MultiplayerPeer"/> 设置要使用的通道时调用（参见 <see cref="Godot.MultiplayerPeer.TransferChannel"/>）。</para>
    /// </summary>
    public override void _SetTransferChannel(int pChannel)
    {
    }

    /// <summary>
    /// <para>当在此 <see cref="Godot.MultiplayerPeer"/> 上设置传输模式时调用（参见 <see cref="Godot.MultiplayerPeer.TransferMode"/>）。</para>
    /// </summary>
    public override void _SetTransferMode(TransferModeEnum pMode)
    {
    }
}