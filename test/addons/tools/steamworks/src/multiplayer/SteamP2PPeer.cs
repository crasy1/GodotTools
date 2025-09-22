using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 相当于 <see href="https://partner.steamgames.com/doc/api/ISteamNetworking">steamworks networking</see> p2p的包装
/// </summary>
public partial class SteamP2PPeer : SteamPeer
{
    public static async Task<SteamP2PPeer> CreateServer(int maxUser = 4)
    {
        var peer = new SteamP2PPeer();
        peer.PeerId = ServerPeerId;
        await peer.CreateLobby(maxUser);
        return peer;
    }

    public static async Task<SteamP2PPeer> CreateClient(Lobby? lobby)
    {
        var peer = new SteamP2PPeer();
        peer.PeerId = (int)SteamClient.SteamId.AccountId;
        await peer.JoinLobby(lobby);
        return peer;
    }
    private void OnHandShakeFailed(ulong steamId)
    {
        HandShake(steamId);
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
            if (ConnectedPeers.TryAdd(peerId, steamId))
            {
                EmitSignalPeerConnected(peerId);
            }

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