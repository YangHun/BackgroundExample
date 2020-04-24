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
        DownloadManager.OnDisconnectNetwork += OnDisconnected;
        DownloadManager.OnGetFileCount += InitProgressText;

    }

    private void InitProgressText(int value)
    {
        this.totalFiles = value;
        this.downloadedFiles = 0;
        progressText.text = string.Format("리소스 다운로드 중 ( {0} / {1} )", downloadedFiles, totalFiles);
    }

    private void UpdateDownloadedCount(string msg)
    {
        if (this.downloadedFiles >= this.totalFiles) return;
        ++this.downloadedFiles;

        progressText.text =
            (this.downloadedFiles < this.totalFiles) 
            ? string.Format("리소스 다운로드 중 ( {0} / {1} )", downloadedFiles, totalFiles)
            : string.Format("다운로드 완료! ( {0} / {1} )", downloadedFiles, totalFiles);
    }

    private void OnDisconnected(string msg)
    {
        progressText.text = string.Format("다운로드가 중단되었습니다.\n인터넷 연결 확인 후, 다시 시도해주세요.\n\n( Background 전환 시 다운로드를 시작합니다 )");
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

    private void OnApplicationQuit()
    {
        DownloadManager.Shutdown();
    }

    private void OnDestroy()
    {
        DownloadManager.OnSuccessDownload -= UpdateDownloadedCount;
        DownloadManager.OnGetFileCount -= InitProgressText;
        DownloadManager.OnDisconnectNetwork -= OnDisconnected;
    }
}
