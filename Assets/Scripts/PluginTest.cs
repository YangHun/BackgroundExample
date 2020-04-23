using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class PluginTest : MonoBehaviour
{
    public Text text;
    bool serviceRunning = false;

    private void Start()
    {
        long t = 1000;
        DownloadManager.OnSuccess += UpdateDownloadedFilePath;
        LocalNotification.SendNotification(1, t, "Send Notification Test Call!", "Yee", Color.white, true, true, true, "app_icon", null, "default");
    }


    private void UpdateDownloadedFilePath(string path)
    {
        text.text = "last downloaded: " + path;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (!DownloadManager.IsDownloading) 
                DownloadManager.StartDownloadByFolder("Free Fruit Icons Set 1.png");
        }   
        else
        {

        }
    }

    private void OnDestroy()
    {
        DownloadManager.OnSuccess -= UpdateDownloadedFilePath;
    }
}
