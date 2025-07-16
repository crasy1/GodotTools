using System;

namespace Godot;

public static class SteamConfig
{
    private static Lazy<Resource> _instance = new(() => ResourceLoader.Load(SteamworksConfigPath));
    public static Resource Instance => _instance.Value;

    private const string SteamworksConfigPath = "res://addons/steamworks/src/SteamConfig.tres";

    public static uint AppId
    {
        get => Instance.GetMeta("appId").AsUInt32();
        set => Save("appId", value);
    }

    public static bool Debug
    {
        get => Instance.GetMeta("debug").AsBool();
        set => Save("debug", value);
    }public static bool CallbackDebug
    {
        get => Instance.GetMeta("callbackDebug").AsBool();
        set => Save("callbackDebug", value);
    }

    private static void Save(string key, Variant value)
    {
        Instance.SetMeta(key, value);
        ResourceSaver.Save(Instance, SteamworksConfigPath);
    }
}