using System;
using System.Text;
using Godot;
using Steamworks;

[SceneTree]
public partial class Test2d : Node2D
{
    private bool IsServer { set; get; }

    private Friend? ChooseFriend { set; get; }
    private int Port { set; get; } = 60937;
    private int PeerType => SocketType.Selected;

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
        Create.Pressed += async () =>
        {
            if (multiplayerApi.MultiplayerPeer is not OfflineMultiplayerPeer)
            {
                return;
            }

            IsServer = true;
            switch (PeerType)
            {
                case 0:
                    try
                    {
                        multiplayerApi.MultiplayerPeer = await SteamP2PPeer.CreateServer();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }

                    break;
                case 1:
                    try
                    {
                        multiplayerApi.MultiplayerPeer = await SteamMessageP2PPeer.CreateServer();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }

                    break;
                case 2:
                    try
                    {
                        multiplayerApi.MultiplayerPeer = await SteamSocketPeer.CreateRelayServer(Port);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }

                    break;
                case 3:
                    try
                    {
                        multiplayerApi.MultiplayerPeer = await SteamSocketPeer.CreateNormalServer((ushort)Port);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }

                    break;
                default:
                    var peer = new ENetMultiplayerPeer();
                    peer.CreateServer(Port, 1);
                    multiplayerApi.MultiplayerPeer = peer;
                    break;
            }

            var test2dPlayer = AddPlayer(Multiplayer.GetUniqueId());
            test2dPlayer.Position = new Vector2(500, 500);
        };
        Search.Pressed += () =>
        {
            Friends.ClearAndFreeChildren();
            foreach (var (steamId, friend) in SFriends.Friends)
            {
                if (friend.IsOnline && friend.IsPlayingThisGame)
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
        };
        Exit.Pressed += () =>
        {
            if (multiplayerApi.MultiplayerPeer is OfflineMultiplayerPeer)
            {
                return;
            }

            if (Multiplayer.IsServer())
            {
                Players.ClearAndFreeChildren();
            }

            multiplayerApi.MultiplayerPeer.Close();
            multiplayerApi.MultiplayerPeer = new OfflineMultiplayerPeer();
        };
        Join.Pressed += async () =>
        {
            if (multiplayerApi.MultiplayerPeer is not OfflineMultiplayerPeer)
            {
                return;
            }

            var lobby = ChooseFriend?.GameInfo?.Lobby;
            if (lobby is null)
            {
                return;
            }

            switch (PeerType)
            {
                case 0:
                    try
                    {
                        multiplayerApi.MultiplayerPeer = await SteamP2PPeer.CreateClient(lobby.Value);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }

                    break;
                case 1:
                    try
                    {
                        multiplayerApi.MultiplayerPeer =
                            await SteamMessageP2PPeer.CreateClient(lobby.Value);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }

                    break;
                case 2:
                    try
                    {
                        multiplayerApi.MultiplayerPeer =
                            await SteamSocketPeer.CreateRelayClient(lobby.Value, Port);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }

                    break;
                case 3:
                    try
                    {
                        multiplayerApi.MultiplayerPeer =
                            await SteamSocketPeer.CreateNormalClient(Host.Text, (ushort)Port, lobby.Value);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }

                    break;
                default:
                    var peer = new ENetMultiplayerPeer();
                    peer.CreateClient(Host.Text, Port);
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
                    var p2PPeer = (SteamP2PPeer)multiplayerApi.MultiplayerPeer;
                    break;
                case 1:
                    var msgP2PPeer = (SteamMessageP2PPeer)multiplayerApi.MultiplayerPeer;
                    break;
                case 2:
                    var relayPeer = (SteamSocketPeer)multiplayerApi.MultiplayerPeer;
                    break;
                case 3:
                    var normalPeer = (SteamSocketPeer)multiplayerApi.MultiplayerPeer;
                    break;
                default:
                    var enetPeer = (ENetMultiplayerPeer)multiplayerApi.MultiplayerPeer;
                    // 貌似不行，只能用在场景同步
                    // enetPeer.Host.Broadcast(0, content, (int)MultiplayerPeer.TransferModeEnum.Reliable);
                    break;
            }
        };
        Spawner.Spawned += (node) => { Log.Info($"{node},{node.Name} Spawned on {Multiplayer.GetUniqueId()}"); };
        Spawner.Despawned += (node) => { Log.Info($"{node},{node.Name} Despawned on {Multiplayer.GetUniqueId()}"); };
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
                case Key.Key3: Create.EmitSignal(BaseButton.SignalName.Pressed); break;
            }
        }
    }

    private void OnPeerDisconnected(long id)
    {
        Log.Debug($"{Multiplayer.GetUniqueId()} peer {id} 断开,peers :{Multiplayer.GetPeers().Join()}");
        if (Multiplayer.IsServer())
        {
            foreach (var child in Players.GetChildren())
            {
                if (child.Name == id.ToString())
                {
                    Players.RemoveAndQueueFreeChild(child);
                }
            }
        }
    }

    private async void OnPeerConnected(long id)
    {
        Log.Debug($"{Multiplayer.GetUniqueId()} peer {id} 连接,peers :{Multiplayer.GetPeers().Join()}");
        if (Multiplayer.IsServer())
        {
            var test2dPlayer = AddPlayer((int)id);
            test2dPlayer.Rpc(Test2dPlayer.MethodName.AsyncPosition,
                new Vector2(500 + Multiplayer.GetPeers().Length * 200, 500));
        }
    }


    private void OnConnectedToServer()
    {
        Log.Debug($"{Multiplayer.GetUniqueId()} 连接到服务器");
    }

    private void OnConnectionFailed()
    {
        Log.Debug($"{Multiplayer.GetUniqueId()} 连接服务器失败");
    }

    private void OnServerDisconnected()
    {
        Log.Debug($"{Multiplayer.GetUniqueId()} 与服务器断开连接");
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer,
        CallLocal = true,
        TransferMode = MultiplayerPeer.TransferModeEnum.Reliable,
        TransferChannel = 0)]
    public void AnyPeerCallRemoteAndLocal(string message)
    {
        Log.Info($"peerId:{Multiplayer.GetUniqueId()} {nameof(AnyPeerCallRemoteAndLocal)} {message}");
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer,
        CallLocal = false,
        TransferMode = MultiplayerPeer.TransferModeEnum.Reliable,
        TransferChannel = 1)]
    public void AnyPeerCallRemote(string message)
    {
        Log.Info($"peerId:{Multiplayer.GetUniqueId()} {nameof(AnyPeerCallRemote)} {message}");
    }
}