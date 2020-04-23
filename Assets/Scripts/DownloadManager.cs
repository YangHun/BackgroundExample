using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class DownloadManager : MonoBehaviour
{ 
    static AndroidJavaClass activity = null;
    static bool isRunning = false;

    public static UnityAction<string> OnSuccess;

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

    public void OnSuccessDownloadFile(string localPath)
    {
        OnSuccess?.Invoke(localPath);
    }
}
