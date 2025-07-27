using System;
using System.Collections.Generic;
using System.Text;
using Steamworks;

namespace Godot;

[Singleton]
public partial class SNetworking : SteamComponent
{
    [Signal]
    public delegate void ReceiveVoiceEventHandler(ulong steamId, byte[] data);

    [Signal]
    public delegate void ReceiveMessageEventHandler(ulong steamId, string data);


    [Signal]
    public delegate void UserConnectEventHandler(ulong steamId);

    [Signal]
    public delegate void UserDisconnectEventHandler(ulong steamId);

    /// <summary>
    /// 已连接的玩家id
    /// </summary>
    public readonly List<SteamId> ConnectedIds = new();

    public override void _Ready()
    {
        base._Ready();
        SteamNetworking.OnP2PSessionRequest += (steamId) =>
        {
            Log.Info($"p2p:收到 {steamId}连接请求");
            var success = SteamNetworking.AcceptP2PSessionWithUser(steamId);
            Log.Info($"p2p:接受 {steamId}连接请求: {success}");
            if (success && !ConnectedIds.Contains(steamId))
            {
                ConnectedIds.Add(steamId);
                EmitSignalUserConnect(steamId);
            }
        };
        SteamNetworking.OnP2PConnectionFailed += (steamId, error) =>
        {
            Log.Info($"p2p:与 {steamId} 连接失败,{error}");
            if (ConnectedIds.Contains(steamId))
            {
                ConnectedIds.Remove(steamId);
                EmitSignalUserDisconnect(steamId);
            }
        };
        SClient.Instance.SteamClientConnected += () =>
        {
            SetProcess(true);
            // 添加队伍语音
            AddChild(TeamVoice.Instance);
        };
        SClient.Instance.SteamClientDisconnected += () => { SetProcess(false); };
    }

    /// <summary>
    /// 与某人断开
    /// </summary>
    /// <param name="steamId"></param>
    /// <returns></returns>
    public bool Disconnect(SteamId steamId)
    {
        var result = SteamNetworking.CloseP2PSessionWithUser(steamId);
        if (result)
        {
            Log.Info($"p2p:与 {steamId} 断开连接");
            ConnectedIds.Remove(steamId);
            EmitSignalUserDisconnect(steamId);
        }

        return result;
    }

    public override void _Process(double delta)
    {
        if (SteamNetworking.IsP2PPacketAvailable((int)Channel.Voice))
        {
            var readP2PPacket = SteamNetworking.ReadP2PPacket((int)Channel.Voice);
            if (readP2PPacket.HasValue)
            {
                EmitSignalReceiveVoice(readP2PPacket.Value.SteamId, readP2PPacket.Value.Data);
            }
        }

        if (SteamNetworking.IsP2PPacketAvailable((int)Channel.Msg))
        {
            var readP2PPacket = SteamNetworking.ReadP2PPacket((int)Channel.Msg);
            if (readP2PPacket.HasValue)
            {
                EmitSignalReceiveMessage(readP2PPacket.Value.SteamId,
                    Encoding.UTF8.GetString(readP2PPacket.Value.Data));
            }
        }
    }

    public void SendP2P(SteamId steamId, string content, Channel channel, P2PSend sendType = P2PSend.Reliable)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        SendP2P(steamId, bytes, channel, sendType);
    }

    public void SendP2P(SteamId steamId, byte[] data, Channel channel, P2PSend sendType = P2PSend.Reliable)
    {
        SteamNetworking.SendP2PPacket(steamId, data, data.Length, (int)channel, sendType);
    }
}