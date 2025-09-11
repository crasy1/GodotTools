using System;
using System.Collections.Generic;
using System.Text;
using Steamworks;

namespace Godot;

[Singleton]
public partial class SNetworking : SteamComponent
{
    [Signal]
    public delegate void ReceiveDataEventHandler(ulong steamId, int channel, byte[] data);

    [Signal]
    public delegate void UserConnectedEventHandler(ulong steamId);

    [Signal]
    public delegate void UserConnectFailedEventHandler(ulong steamId);

    [Signal]
    public delegate void UserDisconnectedEventHandler(ulong steamId);

    /// <summary>
    /// 已连接的玩家id
    /// </summary>
    public readonly List<SteamId> ConnectedIds = new();

    public const int MaxPacketSize = 512 * 1024;

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
                EmitSignalUserConnected(steamId);
            }
        };
        SteamNetworking.OnP2PConnectionFailed += (steamId, error) =>
        {
            Log.Info($"p2p:与 {steamId} 连接失败,{error}");
            EmitSignalUserConnectFailed(steamId);
            if (ConnectedIds.Contains(steamId))
            {
                ConnectedIds.Remove(steamId);
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
            EmitSignalUserDisconnected(steamId);
        }

        return result;
    }

    public override void _Process(double delta)
    {
        if (SteamNetworking.IsP2PPacketAvailable((int)Channel.Msg))
        {
            var readP2PPacket = SteamNetworking.ReadP2PPacket((int)Channel.Msg);
            if (readP2PPacket.HasValue)
            {
                EmitSignalReceiveData(readP2PPacket.Value.SteamId, (int)Channel.Msg, readP2PPacket.Value.Data);
            }
        }

        if (SteamNetworking.IsP2PPacketAvailable((int)Channel.Voice))
        {
            var readP2PPacket = SteamNetworking.ReadP2PPacket((int)Channel.Voice);
            if (readP2PPacket.HasValue)
            {
                EmitSignalReceiveData(readP2PPacket.Value.SteamId, (int)Channel.Voice, readP2PPacket.Value.Data);
            }
        }

        if (SteamNetworking.IsP2PPacketAvailable((int)Channel.Handshake))
        {
            var readP2PPacket = SteamNetworking.ReadP2PPacket((int)Channel.Handshake);
            if (readP2PPacket.HasValue)
            {
                EmitSignalReceiveData(readP2PPacket.Value.SteamId, (int)Channel.Handshake, readP2PPacket.Value.Data);
            }
        }
    }

    public static bool SendP2P(SteamId steamId, string content, Channel channel, P2PSend sendType = P2PSend.Reliable)
    {
        return SendP2P(steamId, Encoding.UTF8.GetBytes(content), channel, sendType);
    }

    public static bool SendP2P(SteamId steamId, byte[] data, Channel channel, P2PSend sendType = P2PSend.Reliable)
    {
        return SteamNetworking.SendP2PPacket(steamId, data, data.Length, (int)channel, sendType);
    }
}