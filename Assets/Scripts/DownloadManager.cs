﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class DownloadManager : MonoBehaviour
{ 
    static AndroidJavaClass activity = null;
    static bool isRunning = false;

    public static UnityAction<int> OnGetFileCount;
    public static UnityAction<string> OnSuccessDownload;
    public static UnityAction<string> OnDisconnectNetwork;


    public static bool IsDownloading
    {
        get { return isRunning; }
    }

    private void Start()
    {
        this.gameObject.name = "DownloadManager";
        DontDestroyOnLoad(this.gameObject);
    }
    
    public static void StartDownloadByFolder(string storagePath)
    {
        if (isRunning) return;
        if (activity == null) activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var obj = activity.GetStatic<AndroidJavaObject>("currentActivity");
        Assert.IsNotNull(obj, "UnityPlayerActivity is null");

        obj.Call("startDownloadFromUnity", FirebaseConfig.STORAGE_URL, storagePath);
        isRunning = true;
    }
    
    public static void Shutdown()
    {
        if (!isRunning || activity == null) return;
        var obj = activity.GetStatic<AndroidJavaObject>("currentActivity");
        Assert.IsNotNull(obj, "UnityPlayerActivity is null");

        obj.Call("shutdownFromUnity");
        isRunning = false;
    }

    public void OnSuccessRetrievePath(string msg)
    {
        int.TryParse(msg, out int value);
        OnGetFileCount?.Invoke(value);
    }

    public void OnDisconnectToServer(string msg)
    {
        OnDisconnectNetwork?.Invoke(msg);
    }

    public void OnSuccessDownloadFile(string msg)
    {
        OnSuccessDownload?.Invoke(msg);
    }

    public void OnDestroyService(string msg)
    {
        // Download event only occurs once
        // isRunning = false;
    }
}
