using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 相当于 <see href="https://partner.steamgames.com/doc/api/ISteamNetworking">steamworks networking</see> p2p的包装
/// </summary>
public partial class SteamP2PPeer : SteamPeer
{
    private SteamP2PPeer(int peerId, SteamSocketType socketType) : base(peerId, socketType)
    {
    }

    public static async Task<SteamP2PPeer> CreateServer(int maxUser = 4)
    {
        var peer = new SteamP2PPeer(ServerPeerId, SteamSocketType.P2P);
        await CreateLobby(maxUser);
        return peer;
    }

    public static async Task<SteamP2PPeer> CreateClient(Lobby lobby)
    {
        var peer = new SteamP2PPeer((int)SteamClient.SteamId.AccountId, SteamSocketType.P2P);
        await JoinLobby(lobby);
        return peer;
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

    private void HandShake(SteamId steamId)
    {
        SendMsg(steamId, Consts.SocketHandShake, Channel.Handshake);
    }

    protected override void OnCreate()
    {
        SNetworking.Instance.ReceiveData += ReceiveData;
        SNetworking.Instance.UserConnectFailed += OnHandShakeFailed;
    }

    protected override void OnClose()
    {
        SNetworking.Instance.ReceiveData -= ReceiveData;
        SNetworking.Instance.UserConnectFailed += OnHandShakeFailed;
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

    protected override bool SendMsg(SteamId steamId, byte[] data, Channel channel = Channel.Msg,
        SendType sendType = SendType.Reliable)
    {
        return SNetworking.SendP2P(steamId, data, channel, sendType);
    }

    protected override void OnPeerDisconnect(SteamId steamId)
    {
        SNetworking.Instance.Disconnect(steamId);
    }

    protected override bool ServerRelaySupported()
    {
        return true;
    }

    protected override void Receive()
    {
    }
}