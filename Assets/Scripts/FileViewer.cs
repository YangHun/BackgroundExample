using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Assertions;
using System.Threading.Tasks;
using System.Text;

public class FileViewer : MonoBehaviour
{
    private class Content
    {
        public enum Type
        {
            Image,
            Text,
            Count
        };

        public Type type;
        public Sprite sprite;
        public string text;
        public string path;
    
        public Content(string path, Sprite sprite)
        {
            this.sprite = sprite;
            this.type = Type.Image;
            this.text = null;
            this.path = path;
        }

        public Content(string path, string text)
        {
            this.sprite = null;
            this.type = Type.Text;
            this.text = text;
            this.path = path;
        }
    }

    [SerializeField]
    Button left;
    [SerializeField]
    Button right;

    [SerializeField]
    Image previewImage;
    [SerializeField]
    Text previewText;
    [SerializeField]
    Text infoText;

    List<string> filePath = new List<string>();

    Dictionary<int, Content> cache = new Dictionary<int, Content>();

    Material imgMaterial;

    int cursor = -1;

    private void Start()
    {
        left.interactable = false;
        right.interactable = false;
        previewText.gameObject.SetActive(false);
        previewImage.gameObject.SetActive(false);
        infoText.text = "";

        imgMaterial = previewImage.material;

        left.onClick.AddListener(OnClickLeft);
        right.onClick.AddListener(OnClickRight);
        DownloadManager.OnSuccessDownload += UpdatePreviewList;
    }

    private void UpdatePreviewList(string msg)
    {
        if (!filePath.Contains(msg))
        {
            filePath.Add(msg);
            CacheContent(filePath.Count - 1);
        }
    }

    private async void CacheContent(int index)
    {
        string path = filePath[index];
        Assert.IsTrue(File.Exists(path));
        FileInfo info = new FileInfo(path);

        if (info.Extension.Equals(".png") || info.Extension.Equals(".jpg"))
        {
            await Task.Yield();
            byte[] data = await Task.Run(() => ReadFileAsync(index));
            int size = (int)(Mathf.Sqrt(data.Length / 4.0f));

            Texture2D tex = new Texture2D(size, size);
            await Task.Yield();
            tex.LoadImage(data);

            await Task.Yield();
            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            cache.Add(index, new Content(filePath[index], sprite));
            MoveTo(cursor);
        }
        else if (info.Extension.Equals(".txt"))
        {
            await Task.Yield();
            byte[] data = await ReadFileAsync(index);
            cache.Add(index, new Content(filePath[index], Encoding.Default.GetString(data)));
            MoveTo(cursor);
        }
    }

    private bool TryShowContent(int index)
    {
        if (cache.Count < filePath.Count) return false;

        Content content = cache.ContainsKey(index) ? cache[index] : null;
        if (content == null) return false;

        if (content.type == Content.Type.Image)
        {
            previewImage.sprite = content.sprite;
        }
        else if (content.type == Content.Type.Text)
        {
            previewText.text = content.text;
        }

        infoText.text = string.Format("{0}\n cursor = {1}",content.path, cursor);

        previewImage.gameObject.SetActive(content.type == Content.Type.Image);
        previewText.gameObject.SetActive(content.type == Content.Type.Text);

        
        return true;
    }

    private void MoveTo(int index)
    {
        index = Mathf.Clamp(index, 0, filePath.Count - 1);
        cursor = index;

        if (TryShowContent(index))
        {
            right.interactable = !(cursor >= filePath.Count - 1);
            left.interactable = !(cursor <= 0);
        }
        else
        {
            previewText.text = "( Loading ... )";
            previewImage.gameObject.SetActive(false);
            previewText.gameObject.SetActive(true);
        }
    }

    public void OnClickLeft()
    {
        MoveTo(cursor - 1);
    }

    public void OnClickRight()
    {
        MoveTo(cursor + 1);
    }

    private void OnDestroy()
    {
        left.onClick.RemoveAllListeners();
        right.onClick.RemoveAllListeners();
        DownloadManager.OnSuccessDownload -= UpdatePreviewList;
    }

    private async Task<byte[]> ReadFileAsync(int index)
    {
        using (MemoryStream memStream = new MemoryStream(1000))
        {
            using (FileStream fileStream = new FileStream(filePath[index], FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
            {
                byte[] buffer = new byte[1024];
                int numRead = 0;
                while ((numRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    await memStream.WriteAsync(buffer, 0, numRead);
                }
            }
                       
            return memStream.ToArray();
        }
    }

    
}
