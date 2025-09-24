using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using Steamworks.Data;

namespace Godot;

[Singleton]
public partial class SNetworkingSocketMessages : SteamComponent
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

    public static readonly List<int> Channels = Enum.GetValues(typeof(Channel))
        .Cast<Channel>()
        .Select(c => (int)c)
        .ToList();

    public override void _Ready()
    {
        base._Ready();
        SteamNetworkingMessages.OnSessionRequest += (identity) =>
        {
            var steamId = identity.SteamId;
            var success = SteamNetworkingMessages.AcceptSessionWithUser(ref identity);
            Log.Info($"p2p msg:接受 {steamId}连接请求: {success}");
            if (success && !ConnectedIds.Contains(steamId))
            {
                ConnectedIds.Add(steamId);
                EmitSignalUserConnected(steamId);
            }
        };
        SteamNetworkingMessages.OnSessionFailed += (connectionInfo) =>
        {
            var steamId = connectionInfo.Identity.SteamId;
            Log.Info($"p2p msg:与 {steamId} 连接失败");
            EmitSignalUserConnectFailed(steamId);
            ConnectedIds.Remove(steamId);
        };
        SteamNetworkingMessages.OnMessage += (identity, data, size, channel) =>
        {
            unsafe
            {
                var span = new Span<byte>((byte*)data.ToPointer(), size);
                EmitSignalReceiveData(identity.SteamId, channel, span.ToArray());
            }
        };
        SClient.Instance.SteamClientConnected += () => { SetProcess(true); };
        SClient.Instance.SteamClientDisconnected += () => { SetProcess(false); };
    }

    public override void _Process(double delta)
    {
        try
        {
            foreach (var channel in Channels)
            {
                SteamNetworkingMessages.Receive(channel);
            }
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    /// <summary>
    /// 与某人断开
    /// </summary>
    /// <param name="steamId"></param>
    /// <returns></returns>
    public bool Disconnect(SteamId steamId)
    {
        var netIdentity = (NetIdentity)steamId;
        var result = SteamNetworkingMessages.CloseSessionWithUser(ref netIdentity);
        if (result)
        {
            Log.Info($"p2p msg:与 {steamId} 断开连接");
            if (ConnectedIds.Remove(steamId))
            {
                EmitSignalUserDisconnected(steamId);
            }
        }

        return result;
    }

    public static bool SendP2P(SteamId steamId, string content, Channel channel, SendType sendType = SendType.Reliable)
    {
        return SendP2P(steamId, Encoding.UTF8.GetBytes(content), channel, sendType);
    }

    public static bool SendP2P(NetIdentity steamId, byte[] data, Channel channel, SendType sendType = SendType.Reliable)
    {
        return SteamNetworkingMessages.SendMessageToUser(ref steamId, data, data.Length, (int)channel, sendType) ==
               Result.OK;
    }
}