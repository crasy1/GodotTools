using System;
using Steamworks;

namespace Godot;

[Singleton]
public partial class SRemoteStorage : SteamComponent
{

    public override void _Ready()
    {
        base._Ready();
    }

    public void GetInfo()
    {
        Log.Info($@"
----    {nameof(SteamRemoteStorage)}    ----
FileCount:                  {SteamRemoteStorage.FileCount}
Files:                      {SteamRemoteStorage.Files}
IsCloudEnabled:             {SteamRemoteStorage.IsCloudEnabled}
IsCloudEnabledForAccount:   {SteamRemoteStorage.IsCloudEnabledForAccount}
IsCloudEnabledForApp:       {SteamRemoteStorage.IsCloudEnabledForApp}
----    {nameof(SteamRemoteStorage)}    ----
");
    }


    public static void WriteToCloud(string filename, string content)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        SteamRemoteStorage.FileWrite(filename, bytes);
    }

    public static string ReadFromCloud(string filename)
    {
        var content = SteamRemoteStorage.FileRead(filename);
        return System.Text.Encoding.UTF8.GetString(content);
    }
}