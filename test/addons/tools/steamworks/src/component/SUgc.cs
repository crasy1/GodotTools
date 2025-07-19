using System;
using Steamworks;

namespace Godot;

[Singleton]
public partial class SUgc : SteamComponent
{

    public override void _Ready()
    {
        base._Ready();
        SteamUGC.OnItemSubscribed += (appId, publishFileId) =>
        {
            Log.Info($"订阅成功，appId: {appId}, publishFileId: {publishFileId}");
        };
        SteamUGC.OnItemUnsubscribed += (appId, publishFileId) =>
        {
            Log.Info($"取消订阅，appId: {appId}, publishFileId: {publishFileId}");
        };
        SteamUGC.OnItemInstalled += (appId, publishFileId) =>
        {
            Log.Info($"安装成功，appId: {appId}, publishFileId: {publishFileId}");
        };
        SteamUGC.OnDownloadItemResult += (result) =>
        {
            Log.Info($"下载结果 {result}");
        };
    }
    
}