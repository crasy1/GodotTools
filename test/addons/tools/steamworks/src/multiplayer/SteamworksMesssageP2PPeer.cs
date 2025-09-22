using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 相当于steamworks networking message p2p的包装
/// https://partner.steamgames.com/doc/api/ISteamNetworkingMessages
/// </summary>
public partial class SteamworksMessageP2PPeer : SteamPeer
{
    private SteamworksMessageP2PPeer() : base()
    {
        SNetworkingSocketMessages.Instance.ReceiveData += ReceiveData;
        Log.Info($"创建 {nameof(SteamworksMessageP2PPeer)}");
    }

    public static async Task<SteamworksMessageP2PPeer?> CreateServer(int maxUser = 4)
    {
        var peer = new SteamworksMessageP2PPeer();
        peer.PeerId = 1;
        var lobby = await SMatchmaking.CreateLobbyAsync(maxUser);
        if (!lobby.HasValue)
        {
            Log.Error("创建大厅失败");
            return null;
        }

        return peer;
    }

    public static SteamworksMessageP2PPeer CreateClient(Lobby? lobby)
    {
        var peer = new SteamworksMessageP2PPeer();
        peer.PeerId = (int)SteamClient.SteamId.AccountId;
        peer.Connect(lobby);
        return peer;
    }

    public override bool SendMsg(SteamId steamId, byte[] data, Channel channel = Channel.Msg,
        SendType sendType = SendType.Reliable)
    {
        return SNetworkingSocketMessages.SendP2P(steamId, data, channel, sendType);
    }


    public override void _Close()
    {
        base._Close();
        SNetworkingSocketMessages.Instance.ReceiveData -= ReceiveData;
    }

    public override void DisconnectPeer(Friend friend)
    {
        SNetworkingSocketMessages.Instance.Disconnect(friend.Id);
    }


    public override bool ServerRelaySupported()
    {
        return true;
    }


    public override void _SetTransferChannel(int pChannel)
    {
    }

    public override void Receive()
    {
    }
}