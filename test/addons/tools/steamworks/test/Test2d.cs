using System.Text;
using Godot;
using Steamworks;

[SceneTree]
public partial class Test2d : Node2D
{
    private bool IsServer { set; get; }

    private Friend? ChooseFriend { set; get; }
    private int Port { set; get; } = 5000;
    private int PeerType { set; get; } = 3;

    public override void _Ready()
    {
        var multiplayerApi = GetTree().GetMultiplayer();
        multiplayerApi.PeerConnected += OnPeerConnected;
        multiplayerApi.PeerDisconnected += OnPeerDisconnected;
        multiplayerApi.ConnectedToServer += OnConnectedToServer;
        multiplayerApi.ConnectionFailed += OnConnectionFailed;
        multiplayerApi.ServerDisconnected += OnServerDisconnected;
        // 只有把场景加到列表中才会被同步，不在列表中的场景不会被同步
        Spawner.AddSpawnableScene(Test2dPlayer.TscnFilePath);
        Create.Pressed += () =>
        {
            if (multiplayerApi.MultiplayerPeer is not OfflineMultiplayerPeer)
            {
                return;
            }

            IsServer = true;
            switch (PeerType)
            {
                case 0:
                    multiplayerApi.MultiplayerPeer = new SteamworksP2PPeer();
                    break;
                case 1:
                    multiplayerApi.MultiplayerPeer = new SteamworksMessageP2PPeer();
                    break;
                case 2:
                    multiplayerApi.MultiplayerPeer = SteamworksServerPeer.CreateServer(Port);
                    break;
                case 3:
                    multiplayerApi.MultiplayerPeer = NormalServerPeer.CreateServer(Port);
                    break;
                default:
                    var peer = new ENetMultiplayerPeer();
                    peer.CreateServer(Port, 4);
                    multiplayerApi.MultiplayerPeer = peer;
                    break;
            }

            var test2dPlayer = AddPlayer(Multiplayer.GetUniqueId());
            test2dPlayer.Position = new Vector2(500, 500);
        };
        Search.Pressed += () => { };
        Exit.Pressed += () =>
        {
            if (multiplayerApi.MultiplayerPeer is OfflineMultiplayerPeer)
            {
                return;
            }

            multiplayerApi.MultiplayerPeer.Close();
            multiplayerApi.MultiplayerPeer = new OfflineMultiplayerPeer();
        };
        Join.Pressed += () =>
        {
            if (multiplayerApi.MultiplayerPeer is not OfflineMultiplayerPeer)
            {
                return;
            }

            switch (PeerType)
            {
                case 0:
                    if (ChooseFriend == null)
                    {
                        break;
                    }

                    var p2PPeer = new SteamworksP2PPeer();
                    multiplayerApi.MultiplayerPeer = p2PPeer;
                    p2PPeer.Connect(ChooseFriend.Value.Id);
                    break;
                case 1:
                    if (ChooseFriend == null)
                    {
                        break;
                    }

                    var msgP2PPeer = new SteamworksMessageP2PPeer();
                    multiplayerApi.MultiplayerPeer = msgP2PPeer;
                    msgP2PPeer.Connect(ChooseFriend.Value.Id);
                    break;
                case 2:
                    if (ChooseFriend == null)
                    {
                        break;
                    }

                    multiplayerApi.MultiplayerPeer = SteamworksClientPeer.CreateClient(ChooseFriend.Value.Id, Port);
                    break;
                case 3:
                    if (ChooseFriend == null)
                    {
                        break;
                    }

                    multiplayerApi.MultiplayerPeer = NormalClientPeer.CreateClient("127.0.0.1", Port);
                    break;
                default:
                    var peer = new ENetMultiplayerPeer();
                    peer.CreateClient("localhost", Port);
                    multiplayerApi.MultiplayerPeer = peer;
                    break;
            }
        };
        Send.Pressed += () =>
        {
            if (multiplayerApi.MultiplayerPeer is OfflineMultiplayerPeer)
            {
                return;
            }

            var content = Encoding.Unicode.GetBytes($"hello {Time.GetDateStringFromSystem()}");
            switch (PeerType)
            {
                case 0:
                    var p2PPeer = (SteamworksP2PPeer)multiplayerApi.MultiplayerPeer;
                    break;
                case 1:
                    var msgP2PPeer = (SteamworksMessageP2PPeer)multiplayerApi.MultiplayerPeer;
                    break;
                case 2:
                    if (IsServer)
                    {
                        var serverPeer = (SteamworksServerPeer)multiplayerApi.MultiplayerPeer;
                    }
                    else
                    {
                        var clientPeer = (SteamworksClientPeer)multiplayerApi.MultiplayerPeer;
                    }

                    break;
                case 3:
                    if (IsServer)
                    {
                        var serverPeer = (NormalServerPeer)multiplayerApi.MultiplayerPeer;
                    }
                    else
                    {
                        var clientPeer = (NormalClientPeer)multiplayerApi.MultiplayerPeer;
                    }

                    break;
                default:
                    var enetPeer = (ENetMultiplayerPeer)multiplayerApi.MultiplayerPeer;
                    // 貌似不行，只能用在场景同步
                    // enetPeer.Host.Broadcast(0, content, (int)MultiplayerPeer.TransferModeEnum.Reliable);
                    break;
            }
        };
        Spawner.Spawned += (node) => { Log.Info($"{node} Spawned on {Multiplayer.GetUniqueId()}"); };
        Spawner.Despawned += (node) => { Log.Info($"{node} Despawned on {Multiplayer.GetUniqueId()}"); };
        foreach (var (steamId, friend) in SFriends.Friends)
        {
            if (friend.IsOnline)
            {
                var steamUserInfo = SteamUserInfo.Instantiate(friend);
                Friends.AddChild(steamUserInfo);
                steamUserInfo.GuiInput += (@event) =>
                {
                    if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
                    {
                        ChooseFriend = friend;
                        Log.Info($"选择 {friend.Id}");
                    }
                };
            }
        }
    }

    public Test2dPlayer AddPlayer(int peerId)
    {
        var test2dPlayer = Test2dPlayer.Instantiate();
        test2dPlayer.Name = $"{peerId}";
        Players.AddChild(test2dPlayer);
        return test2dPlayer;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey { Pressed: true } eventKey)
        {
            switch (eventKey.Keycode)
            {
                case Key.Key1: Rpc(MethodName.AnyPeerCallRemoteAndLocal, "1"); break;
                case Key.Key2: Rpc(MethodName.AnyPeerCallRemote, "2"); break;
            }
        }
    }

    private void OnPeerDisconnected(long id)
    {
        Log.Debug($"{Multiplayer.GetUniqueId()} peer {id} 断开连接");
    }

    private void OnPeerConnected(long id)
    {
        Log.Debug($"{Multiplayer.GetUniqueId()} peer {id} 连接");
        if (Multiplayer.IsServer())
        {
            var test2dPlayer = AddPlayer((int)id);
            test2dPlayer.Rpc(Test2dPlayer.MethodName.AsyncPosition, new Vector2(1000, 500));
        }
    }


    private void OnConnectedToServer()
    {
        Log.Debug($"连接到服务器");
    }

    private void OnConnectionFailed()
    {
        Log.Debug($"连接服务器失败");
    }

    private void OnServerDisconnected()
    {
        Log.Debug($"与服务器断开连接");
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer,
        CallLocal = true,
        TransferMode = MultiplayerPeer.TransferModeEnum.Reliable,
        TransferChannel = 0)]
    public void AnyPeerCallRemoteAndLocal(string message)
    {
        Log.Info(nameof(AnyPeerCallRemoteAndLocal), message);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer,
        CallLocal = false,
        TransferMode = MultiplayerPeer.TransferModeEnum.Reliable,
        TransferChannel = 1)]
    public void AnyPeerCallRemote(string message)
    {
        Log.Info(nameof(AnyPeerCallRemote), message);
    }
}