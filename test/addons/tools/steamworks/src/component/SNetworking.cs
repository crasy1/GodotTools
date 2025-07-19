using System;
using System.Text;
using Steamworks;

namespace Godot;

[Singleton]
public partial class SNetworking : SteamComponent
{
    [Signal]
    public delegate void ReceiveMessageEventHandler(ulong steamId, string content);

    public override void _Ready()
    {
        base._Ready();
        SteamNetworking.OnP2PSessionRequest += (steamId) =>
        {
            var success = SteamNetworking.AcceptP2PSessionWithUser(steamId);
            Log.Info($"p2p连接请求 {steamId} : {success}");
        };
        SteamNetworking.OnP2PConnectionFailed += (steamId, error) => { Log.Info($"p2p连接失败 {steamId},{error}"); };
        SClient.Instance.SteamClientConnected += () => { SetProcess(true); };
        SClient.Instance.SteamClientDisconnected += () => { SetProcess(false); };
    }

    public override void _Process(double delta)
    {
        if (SteamNetworking.IsP2PPacketAvailable(0))
        {
            var readP2PPacket = SteamNetworking.ReadP2PPacket(0);
            if (readP2PPacket.HasValue)
            {
                var steamId = readP2PPacket.Value.SteamId;
                var data = Encoding.UTF8.GetString(readP2PPacket.Value.Data);
                Log.Info($"p2p从 {steamId} 收到数据 ,{data}");
                EmitSignalReceiveMessage(steamId, data);
            }
        }
    }

    public static void SendP2P(SteamId steamId, string content, P2PSend sendType = P2PSend.Reliable)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        SteamNetworking.SendP2PPacket(steamId, bytes, bytes.Length, 0, sendType);
    }
}