using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 相当于 <see href="https://partner.steamgames.com/doc/api/ISteamNetworkingMessages">steamworks networking message</see> p2p的包装
/// </summary>
public partial class SteamMessageP2PPeer : SteamPeer
{
    private SteamMessageP2PPeer(int peerId, SteamSocketType socketType) : base(peerId, socketType)
    {
    }

    public static async Task<SteamMessageP2PPeer> CreateServer(int maxUser = 4)
    {
        var peer = new SteamMessageP2PPeer(ServerPeerId, SteamSocketType.P2PMessage);
        await CreateLobby(maxUser);
        return peer;
    }

    public static async Task<SteamMessageP2PPeer> CreateClient(Lobby lobby)
    {
        var peer = new SteamMessageP2PPeer((int)SteamClient.SteamId.AccountId, SteamSocketType.P2PMessage);
        await JoinLobby(lobby);
        return peer;
    }

    protected override void OnCreate()
    {
        SNetworkingSocketMessages.Instance.ReceiveData += ReceiveData;
        SNetworkingSocketMessages.Instance.UserConnectFailed += OnHandShakeFailed;
    }

    protected override void OnClose()
    {
        SNetworkingSocketMessages.Instance.ReceiveData -= ReceiveData;
        SNetworkingSocketMessages.Instance.UserConnectFailed -= OnHandShakeFailed;
    }

    protected override void OnLobbyEntered(Lobby lobby)
    {
        base.OnLobbyEntered(lobby);
        if (!_IsServer())
        {
            // 给服务器发送握手包
            HandShake(lobby.Owner.Id);
        }
    }

    protected override void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        base.OnLobbyEntered(lobby);
        // 服务器给新加入的peer发送握手包
        if (_IsServer())
        {
            HandShake(friend.Id);
        }
    }

    private void OnHandShakeFailed(ulong steamId)
    {
        if (!_IsServer())
        {
            ConnectionStatus = ConnectionStatus.Disconnected;
        }
    }

    public override void ReceiveData(ulong steamId, int channel, byte[] data)
    {
        var peerId = Lobby!.Value.IsOwnedBy(steamId) ? ServerPeerId : (int)((SteamId)steamId).AccountId;
        // 过滤掉握手包
        if (channel == (int)Channel.Handshake)
        {
            OnSocketConnected(steamId);
            return;
        }

        ProcessData(peerId, steamId, channel, data);
    }

    private void HandShake(SteamId steamId)
    {
        SendMsg(steamId, Consts.SocketHandShake, Channel.Handshake);
    }

    protected override bool SendMsg(SteamId steamId, byte[] data, Channel channel = Channel.Msg,
        SendType sendType = SendType.Reliable)
    {
        return SNetworkingSocketMessages.SendP2P(steamId, data, channel, sendType);
    }

    protected override void OnPeerDisconnect(SteamId steamId)
    {
        SNetworkingSocketMessages.Instance.Disconnect(steamId);
    }

    protected override bool ServerRelaySupported()
    {
        return true;
    }

    protected override void Receive()
    {
    }
}