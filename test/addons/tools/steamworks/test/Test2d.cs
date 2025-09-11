using Godot;
using Steamworks;

[SceneTree]
public partial class Test2d : Node2D
{
    private bool IsServer { set; get; }

    private Friend? ChooseFriend { set; get; }
    private int Port { set; get; } = 5000;

    public override void _Ready()
    {
        var multiplayerApi = GetTree().GetMultiplayer();
        multiplayerApi.PeerConnected += OnPeerConnected;
        multiplayerApi.PeerDisconnected += OnPeerDisconnected;
        multiplayerApi.ConnectedToServer += OnConnectedToServer;
        multiplayerApi.ConnectionFailed += OnConnectionFailed;
        multiplayerApi.ServerDisconnected += OnServerDisconnected;
        Create.Pressed += () =>
        {
            IsServer = true;
            // multiplayerApi.MultiplayerPeer = SteamworksServerPeer.CreateServer(Port);
            multiplayerApi.MultiplayerPeer = new SteamworksP2PPeer();
            // Menu.Hide();
        };
        Search.Pressed += () => { };
        Exit.Pressed += () =>
        {
            multiplayerApi.MultiplayerPeer.Close();
            multiplayerApi.MultiplayerPeer = null;
        };
        Join.Pressed += () =>
        {
            if (ChooseFriend != null)
            {
                // multiplayerApi.MultiplayerPeer = SteamworksClientPeer.CreateClient(ChooseFriend.Value.Id, Port);
                var p2PPeer = new SteamworksP2PPeer();
                multiplayerApi.MultiplayerPeer = p2PPeer;
                p2PPeer.Connect(ChooseFriend.Value.Id);
            }
        };
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
                        Log.Info(friend.Id);
                    }
                };
            }
        }
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
        Log.Debug($"peer {id} 断开连接");
    }

    private void OnPeerConnected(long id)
    {
        Log.Debug($"peer {id} 连接");
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