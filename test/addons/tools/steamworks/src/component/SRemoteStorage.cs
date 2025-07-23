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
Files:                      {string.Join(",", SteamRemoteStorage.Files)}
QuotaBytes:                 {SteamRemoteStorage.QuotaBytes}
QuotaUsedBytes:             {SteamRemoteStorage.QuotaUsedBytes}
QuotaRemainingBytes:        {SteamRemoteStorage.QuotaRemainingBytes}
IsCloudEnabled:             {SteamRemoteStorage.IsCloudEnabled}
IsCloudEnabledForAccount:   {SteamRemoteStorage.IsCloudEnabledForAccount}
IsCloudEnabledForApp:       {SteamRemoteStorage.IsCloudEnabledForApp}
----    {nameof(SteamRemoteStorage)}    ----
");
    }


    public bool WriteToCloud(string filename, string content)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        return SteamRemoteStorage.FileWrite(filename, bytes);
    }

    public string? ReadFromCloud(string filename)
    {
        if (SteamRemoteStorage.FileExists(filename))
        {
            var content = SteamRemoteStorage.FileRead(filename);
            return System.Text.Encoding.UTF8.GetString(content);
        }

        return null;
    }
}