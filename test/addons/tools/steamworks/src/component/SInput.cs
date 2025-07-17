using System;
using Steamworks;

namespace Godot;

public partial class SInput:SteamComponent
{
    private static readonly Lazy<SInput> LazyInstance = new(() => new());
    public static SInput Instance => LazyInstance.Value;

    private SInput()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        SClient.Instance.SteamClientConnected += () =>
        {
            var controllers = SteamInput.Controllers;
            foreach (var controller in controllers)
            {
                var inputType = controller.InputType;
                Log.Info($"已连接设备: {inputType}");
            }
        };
    }
}