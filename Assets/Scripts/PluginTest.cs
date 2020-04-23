using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class PluginTest : MonoBehaviour
{
    public Text progressText;
    bool serviceRunning = false;

    int totalFiles = 0;
    int downloadedFiles = 0;

    [SerializeField]
    FileViewer viewer;

    private void Start()
    {
        long t = 1000;

        progressText.text = "대기 중 - Background 전환 시 다운로드를 시작합니다";

        DownloadManager.OnSuccessDownload += UpdateDownloadedCount;
        DownloadManager.OnGetFileCount += InitProgressText;

        LocalNotification.SendNotification(1, t, "Send Notification Test Call!", "Yee", Color.white, true, true, true, "app_icon", null, "default");
    }

    private void InitProgressText(int value)
    {
        this.totalFiles = value;
        this.downloadedFiles = 0;
        UpdateProgressText();
    }

    private void UpdateDownloadedFilePath(string path)
    {
        progressText.text = "last downloaded: " + path;
    }

    private void UpdateDownloadedCount(string msg)
    {
        if (this.downloadedFiles >= this.totalFiles) return;
        ++this.downloadedFiles;
        UpdateProgressText();
    }

    private void UpdateProgressText()
    {
        progressText.text = string.Format("리소스 다운로드 중 ( {0} / {1} )", downloadedFiles, totalFiles);
    }

    

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (!DownloadManager.IsDownloading)
            {
                DownloadManager.StartDownloadByFolder("line");
                progressText.text = "리소스 다운로드를 시작합니다";
            }
        }   
        else
        {

        }
    }

    private void OnDestroy()
    {
        DownloadManager.OnSuccessDownload -= UpdateDownloadedCount;
        DownloadManager.OnGetFileCount -= InitProgressText;
    }
}
