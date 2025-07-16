using System;
using System.Threading;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public partial class SNetworkingSockets : SteamComponent
{
    private static readonly Lazy<SNetworkingSockets> LazyInstance = new(() => new());
    public static SNetworkingSockets Instance => LazyInstance.Value;

    public NetAddress? FakeIp { set; get; }

    private SNetworkingSockets()
    {
    }


    public override void _Ready()
    {
        base._Ready();
        SteamNetworkingSockets.OnConnectionStatusChanged += (c, ci) =>
        {
            Log.Info($"连接状态改变 {ci.Identity},{ci.Address},{ci.State},{ci.EndReason}");
        };
        SteamNetworkingSockets.OnFakeIPResult += (NetAddress na) =>
        {
            Log.Info($"steam fake ip 地址 {na}");
            FakeIp = na;
        };
        SClient.Instance.SteamClientConnected += () => { SteamNetworkingUtils.InitRelayNetworkAccess(); };
        SteamManager.AddBeforeGameQuitAction(CloseAllSocket);
    }

    private void CloseAllSocket()
    {
        Log.Info("---------------- 关闭[steam socket]开始 ----------------");
        foreach (var child in GetChildren())
        {
            if (child is SteamSocket socket)
            {
                if (IsInstanceValid(child))
                {
                    socket.Close();
                }
            }
        }

        Log.Info("---------------- 关闭[steam socket]结束 ----------------");
    }

    public static NormalServer CreateNormal(ushort port)
    {
        var normalServer = new NormalServer(10000);
        Instance.AddChild(normalServer);
        return normalServer;
    }

    public static NormalClient ConnectNormal(ushort port)
    {
        var normalClient = new NormalClient(port);
        Instance.AddChild(normalClient);
        return normalClient;
    }

    public static RelayServer CreateRelay(int port)
    {
        var relayServer = new RelayServer(port);
        Instance.AddChild(relayServer);
        return relayServer;
    }

    public static RelayClient ConnectRelay(SteamId serverId, int port)
    {
        var relayClient = new RelayClient(serverId, port);
        Instance.AddChild(relayClient);
        return relayClient;
    }
}