using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PluginTest : MonoBehaviour
{
    public Text text;


    private void Start()
    {

        var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var obj = activity.GetStatic<AndroidJavaObject>("currentActivity");
           
        if (obj == null)
        {
            text.text = "current Activity is null!!";
        }
        else
        {
            obj.Call("startUnityActivity");
        }

        //var plugin = new AndroidJavaClass("com.eg.myplugin.PluginClass");
        //var plugin = new AndroidJavaObject("com.eg.downloader.ContentsDownloader");

        //text.text = plugin.CallStatic<string>("UnityCall", "testcall");
        //text.text = plugin.Call<bool>("StartDownload").ToString();

        long t = 1000;

        LocalNotification.SendNotification(1, t, "Send Notification Test Call!", "Yee", Color.white, true, true, true, "app_icon", null, "default");
    }


    public void CallFromNative(string str)
    {
        text.text = str;
    }
}
