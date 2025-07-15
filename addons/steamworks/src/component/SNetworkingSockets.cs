using System;
using Steamworks;
using Steamworks.Data;

namespace Godot;

public partial class SNetworkingSockets : SteamComponent
{
    private static readonly Lazy<SNetworkingSockets> LazyInstance = new(() => new());
    public static SNetworkingSockets Instance => LazyInstance.Value;

    private NetAddress? FakeIp { set; get; }

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
        SteamNetworkingSockets.OnConnectionStatusChanged += (c, ci) => { Log.Info($"连接状态改变 {c},{ci}"); };
        SteamNetworkingSockets.OnFakeIPResult += (NetAddress na) =>
        {
            Log.Info($"请求steam fake ip 结果 {na}");
            FakeIp = na;
        };
        SClient.Instance.SteamClientConnected += () => { SteamNetworkingSockets.RequestFakeIP(); };
    }

    public void CreateNormal()
    {
        if (FakeIp.HasValue)
        {
            MySocketManager mySocketManager = new();
            NormalServer = SteamNetworkingSockets.CreateNormalSocket(FakeIp.Value, mySocketManager);
        }
    }

    public void CreateFake()
    {
        if (FakeIp.HasValue)
        {
            MySocketManager mySocketManager = new();
            RelayServer = SteamNetworkingSockets.CreateRelaySocket(FakeIp.Value.Port, mySocketManager);
        }
    }

    public void ConnectNormal(NetAddress address)
    {
        if (FakeIp.HasValue)
        {
            MyConnectionManager manager = new();
            NormalClient = SteamNetworkingSockets.ConnectNormal(address, manager);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serverId">可能是用户steamId</param>
    /// <param name="port"></param>
    public void ConnectRelay(SteamId serverId, int port)
    {
        if (FakeIp.HasValue)
        {
            MyConnectionManager manager = new();
            NormalClient = SteamNetworkingSockets.ConnectRelay(serverId, port, manager);
        }
    }
}