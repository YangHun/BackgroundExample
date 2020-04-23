using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Assertions;
using UnityEngine.Events;

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
        public Sprite image;
        public string text;
    
        public Content(Sprite image)
        {
            this.image = image;
            this.type = Type.Image;
            this.text = null;
        }

        public Content(string text)
        {
            this.image = null;
            this.type = Type.Text;
            this.text = text;
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

    List<string> filePath = new List<string>();
    Dictionary<int, Content> cache = new Dictionary<int, Content>();

    int cursor = -1;

    private void Start()
    {
        left.interactable = false;
        right.interactable = false;
        previewText.gameObject.SetActive(false);
        previewImage.gameObject.SetActive(false);

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

    private void CacheContent(int index)
    {
        string path = filePath[index];
        Assert.IsTrue(File.Exists(path));
        FileInfo info = new FileInfo(path);

        if (info.Extension.Equals(".png") || info.Extension.Equals(".jpg"))
        {
            StartCoroutine(LoadImage(index));            
        }
        else if (info.Extension.Equals(".txt"))
        {
            StartCoroutine(LoadText(index));
        }
    }

    private bool TryShowContent(int index)
    {
        Content content = cache.ContainsKey(index) ? cache[index] : null;
        if (content == null) return false;

        if (content.type == Content.Type.Image) previewImage.sprite = content.image;
        else if (content.type == Content.Type.Text) previewText.text = content.text;

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

    private IEnumerator LoadImage(int index)
    {
        byte[] data = File.ReadAllBytes(filePath[index]);
        yield return null;

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(data);
        yield return null;

        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

        cache.Add(index, new Content(sprite));

        // update current UI
        MoveTo(cursor);
        yield return null;
    }

    private IEnumerator LoadText(int index)
    {
        using (StreamReader reader = new StreamReader(filePath[index]))
        {
            char[] buffer = new char[101];
            reader.Read(buffer, 0, 100);
            yield return null;

            cache.Add(index, new Content(string.Concat(buffer)));
        }

        // update current UI
        MoveTo(cursor);
        yield return null;
    }
}
