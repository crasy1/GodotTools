
namespace Godot;

public static class SteamConfig
{
    private static readonly Resource Instance = ResourceLoader.Load(Paths.SteamworksConfig);

    public static uint AppId
    {
        get => Instance.GetMeta("appId", 480).AsUInt32();
        set => Save("appId", value);
    }

    public static bool Debug
    {
        get => Instance.GetMeta("debug", false).AsBool();
        set => Save("debug", value);
    }

    public static bool CallbackDebug
    {
        get => Instance.GetMeta("callbackDebug", false).AsBool();
        set => Save("callbackDebug", value);
    }

    private static void Save(string key, Variant value)
    {
        Instance.SetMeta(key, value);
        ResourceSaver.Save(Instance, Paths.SteamworksConfig);
    }
}