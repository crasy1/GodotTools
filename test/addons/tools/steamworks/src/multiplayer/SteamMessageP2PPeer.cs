using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 相当于 <see href="https://partner.steamgames.com/doc/api/ISteamNetworkingMessages">steamworks networking message</see> p2p的包装
/// </summary>
public partial class SteamMessageP2PPeer : SteamPeer
{
    public static async Task<SteamMessageP2PPeer> CreateServer(int maxUser = 4)
    {
        var peer = new SteamMessageP2PPeer();
        peer.PeerId = ServerPeerId;
        await peer.CreateLobby(maxUser);
        return peer;
    }

    public static async Task<SteamMessageP2PPeer> CreateClient(Lobby? lobby)
    {
        var peer = new SteamMessageP2PPeer();
        peer.PeerId = (int)SteamClient.SteamId.AccountId;
        await peer.JoinLobby(lobby);
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

    private void OnHandShakeFailed(ulong steamId)
    {
        HandShake(steamId);
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