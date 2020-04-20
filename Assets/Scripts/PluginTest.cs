using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PluginTest : MonoBehaviour
{
    public Text text;


    private void Start()
    {
        var plugin = new AndroidJavaClass("com.eg.myplugin.PluginClass");
        text.text = plugin.CallStatic<string>("UnityCall", "testcall");

        long t = 1000;

        LocalNotification.SendNotification(1, t, "Send Notification Test Call!", "Yee", Color.white, true, true, true, "app_icon", null, "default");
    }


    public void CallFromNative(string str)
    {
        text.text = str;
    }
}
