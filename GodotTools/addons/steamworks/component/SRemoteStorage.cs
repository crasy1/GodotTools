using System;
using Godot;
using Steamworks;

namespace GodotTools.addons.steamworks.component;

public partial class SRemoteStorage : SteamComponent
{
    private static readonly Lazy<SRemoteStorage> LazyInstance = new(() => new());
    public static SRemoteStorage Instance => LazyInstance.Value;

    private SRemoteStorage()
    {
    }

    

    public void WriteToCloud(string filename, string content)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        SteamRemoteStorage.FileWrite(filename, bytes);
    }

    public string ReadFromCloud(string filename)
    {
        var content = SteamRemoteStorage.FileRead(filename);
        return System.Text.Encoding.UTF8.GetString(content);
    }
}