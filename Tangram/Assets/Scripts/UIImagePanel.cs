using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UIImagePanel : MonoBehaviour
{
    public DirectoryInfo problemsDirectoryPath;
    public Image problemImage;
    private string problemImageURL = "https://github.com/yizhouzhao/Tangram/blob/master/Tangram/Assets/Resources/Problems/6.png?raw=true";

    //https://image.jimcdn.com/app/cms/image/transf/none/path/sb0abad0b84d20c80/image/iae9db43733254ec8/version/1407242965/tangram-letter-i.png

    //Resources/Problems/9.png;

    // Start is called before the first frame update
    void Start()
    {
        //LoadImageFromURL(problemImageURL);
    }

    public void LoadImageFromURL(string imageURL)
    {
        StartCoroutine(setImage(imageURL));
    }


    public void LoadRandomImage()
    {
        //FileInfo[] fileInfo = problemsDirectoryPath.GetFiles("*.*", SearchOption.AllDirectories);
    }


    IEnumerator setImage(string url)
    {
        //Texture2D texture = problemImage.mainTexture as Texture2D;

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                var texture = DownloadHandlerTexture.GetContent(uwr);

                problemImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
        }
    }
}
