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

    public SocketManager? NormalServer { set; get; }
    public SocketManager? RelayServer { set; get; }
    public ConnectionManager? NormalClient { set; get; }
    public ConnectionManager? RelayClient { set; get; }

    private SNetworkingSockets()
    {
    }


    public override void _Ready()
    {
        base._Ready();
        SteamNetworkingSockets.OnConnectionStatusChanged += (c, ci) =>
        {
            Log.Info($"连接状态改变 {c},{ci}");
        };
        SteamNetworkingSockets.OnFakeIPResult += (NetAddress na) =>
        {
            Log.Info($"steam fake ip 地址 {na}");
            FakeIp = na;
        };
        SClient.Instance.SteamClientConnected += () => { SteamNetworkingSockets.RequestFakeIP(); };
    }

    public void CreateNormal()
    {
        MySocketManager mySocketManager = new();
        var netAddress = NetAddress.AnyIp(22222);
        NormalServer = SteamNetworkingSockets.CreateNormalSocket(netAddress, mySocketManager);
        Log.Info($"创建 normal server {netAddress}");
        var result = SteamNetworkingSockets.GetFakeIP(1,out var ip);
        Log.Info($"创建 normal server {result} ip {ip}");
    }

    public void CloseNormal()
    {
        if (NormalServer != null)
        {
            NormalServer.Close();
            NormalServer = null;
            Log.Info("关闭 normal server");
        }
    }

    public void CreateFake()
    {
        MySocketManager mySocketManager = new();
        RelayServer = SteamNetworkingSockets.CreateRelaySocket(33333, mySocketManager);
        Log.Info($"创建 relay socket ");
    }

    public void ConnectNormal(NetAddress address)
    {
        MyConnectionManager manager = new();
        NormalClient = SteamNetworkingSockets.ConnectNormal(address, manager);
        Log.Info($"连接 normal {address}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serverId">可能是用户steamId</param>
    /// <param name="port"></param>
    public void ConnectRelay(SteamId serverId, int port)
    {
        MyConnectionManager manager = new();
        NormalClient = SteamNetworkingSockets.ConnectRelay(serverId, port, manager);
        Log.Info($"连接 relay {serverId} {port}");
    }
}