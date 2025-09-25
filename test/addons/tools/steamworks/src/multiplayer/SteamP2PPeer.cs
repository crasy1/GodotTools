using System;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace Godot;

/// <summary>
/// 相当于 <see href="https://partner.steamgames.com/doc/api/ISteamNetworking">steamworks networking</see> p2p的包装
/// 相当于 <see href="https://partner.steamgames.com/doc/api/ISteamNetworkingMessages">steamworks networking message</see> p2p的包装
/// </summary>
public partial class SteamP2PPeer : SteamPeer
{
    private SteamP2PPeer(int peerId, SteamSocketType socketType) : base(peerId, socketType)
    {
    }

    public static SteamP2PPeer CreateServer(SteamSocketType socketType = SteamSocketType.P2PMessage)
    {
        return new SteamP2PPeer(ServerPeerId, socketType);
    }

    public static SteamP2PPeer CreateClient(SteamId steamId, SteamSocketType socketType = SteamSocketType.P2PMessage)
    {
        var peer = new SteamP2PPeer((int)SteamClient.SteamId.AccountId, socketType);
        peer.SendMsg(steamId, Consts.SocketHandShake, Channel.Handshake);
        return peer;
    }


    protected override void OnCreate()
    {
        if (SteamSocketType == SteamSocketType.P2PMessage)
        {
            SNetworkingSocketMessages.Instance.ReceiveData += ReceiveData;
            // SNetworkingSocketMessages.Instance.UserConnected += OnSocketConnected;
            SNetworkingSocketMessages.Instance.UserConnectFailed += OnSocketDisconnected;
        }
        else if (SteamSocketType == SteamSocketType.P2P)
        {
            SNetworking.Instance.ReceiveData += ReceiveData;
            // SNetworking.Instance.UserConnected += OnSocketConnected;
            SNetworking.Instance.UserConnectFailed += OnSocketDisconnected;
        }
    }

    protected override void OnClose()
    {
        if (SteamSocketType == SteamSocketType.P2PMessage)
        {
            SNetworkingSocketMessages.Instance.ReceiveData -= ReceiveData;
            // SNetworkingSocketMessages.Instance.UserConnected -= OnSocketConnected;
            SNetworkingSocketMessages.Instance.UserConnectFailed -= OnSocketDisconnected;
        }
        else if (SteamSocketType == SteamSocketType.P2P)
        {
            SNetworking.Instance.ReceiveData -= ReceiveData;
            // SNetworking.Instance.UserConnected -= OnSocketConnected;
            SNetworking.Instance.UserConnectFailed -= OnSocketDisconnected;
        }
    }

    public override void ReceiveData(ulong steamId, int channel, byte[] data)
    {
        var peerId = SteamIdToPeerId(steamId);
        // 过滤掉握手包
        if (channel == (int)Channel.Handshake)
        {
            var msg = Encoding.UTF8.GetString(data);
            switch (msg)
            {
                case Consts.SocketHandShake:
                    if (_IsServer())
                    {
                        SendMsg(steamId, Consts.SocketHandShakeReply, Channel.Handshake);
                    }

                    else
                    {
                        SendMsg(steamId, Consts.SocketHandShake, Channel.Handshake);
                    }

                    break;
                case Consts.SocketHandShakeReply:
                    if (_IsServer())
                    {
                        OnSocketConnected(steamId);
                    }
                    else
                    {
                        SendMsg(steamId, Consts.SocketHandShakeReply, Channel.Handshake);
                        OnSocketConnected(steamId);
                    }

                    break;
                case Consts.SocketDisconnect:
                    if (ConnectedPeers.ContainsValue(steamId))
                    {
                        OnSocketDisconnected(steamId);
                    }
                    break;
            }

            return;
        }

        ProcessData(peerId, steamId, channel, data);
    }

    protected override bool SendMsg(SteamId steamId, byte[] data, Channel channel = Channel.Msg,
        SendType sendType = SendType.Reliable)
    {
        try
        {
            if (SteamSocketType == SteamSocketType.P2PMessage)
            {
                return SNetworkingSocketMessages.SendP2P(steamId, data, channel, sendType);
            }
            else if (SteamSocketType == SteamSocketType.P2P)
            {
                return SNetworking.SendP2P(steamId, data, channel, sendType);
            }

            return false;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    protected override async Task OnPeerDisconnect(SteamId steamId)
    {
        SendMsg(steamId, Consts.SocketDisconnect, Channel.Handshake);
        await Task.Delay(1000);
        if (SteamSocketType == SteamSocketType.P2PMessage)
        {
            SNetworkingSocketMessages.Instance.Disconnect(steamId);
        }
        else if (SteamSocketType == SteamSocketType.P2P)
        {
            SNetworking.Instance.Disconnect(steamId);
        }
    }

    protected override bool ServerRelaySupported()
    {
        return true;
    }

    protected override void Receive()
    {
    }
}