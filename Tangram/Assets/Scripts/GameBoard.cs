using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CurrentShapeInformation
{
    public List<int> MoveShapeSequence;
    public List<ShapeInfo> SevenShapeInfo;

    public CurrentShapeInformation()
    {
        SevenShapeInfo = new List<ShapeInfo>();
        MoveShapeSequence = new List<int>();
    }

    //Add the move sequence and labeling
    public void AddMoveShapeToSequence(int idx)
    {
        MoveShapeSequence.Add(idx);
    }
}


//[System.Serializable]
//public class AllInformation
//{
//    public List<CurrentShapeInformation> AllTimeShapeInfoList;

//    public AllInformation() { AllTimeShapeInfoList = new List<CurrentShapeInformation>(); }

//    public void AddCurrrentShapeInfo(CurrentShapeInformation currShapeInfo)
//    {
//        AllTimeShapeInfoList.Add(currShapeInfo);
//    }
//}


public class GameBoard : MonoBehaviour
{
    [Header("Images and URLs")]
    public string imageURLTextURL = "https://raw.githubusercontent.com/yizhouzhao/Tangram/master/Tangram/Assets/Resources/ImageURL.txt";
    public string alreadyLabeledURL = "https://raw.githubusercontent.com/yizhouzhao/Tangram/master/Tangram/Assets/Resources/AlreadyLabeledImageIndex.txt";
    public int imageIndex;
    public List<string> imageURLList;
    public string imageURL;
    public UIImagePanel imagePanelUI;

    [Header("Google form sender")]
    public GoogleFormSender formSender;

    //Information
    public List<ShapeMove> SevenShapeMoveList;

    //public AllInformation allInformation;

    public static CurrentShapeInformation curInfo;

    //Get images url from text url
    public void GetImageURLs()
    {
        StartCoroutine(GetTextFromURL(imageURLTextURL));
    }

    IEnumerator GetTextFromURL(string textURL)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(imageURLTextURL))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.Log( "Error: " + webRequest.error);

                yield return null;
            }
            else
            {
                string[] urlLinks = webRequest.downloadHandler.text.Split('\n');
                foreach(string urlLink in urlLinks)
                {
                    imageURLList.Add(urlLink);
                }
                //Debug.Log("Gameboard :\nReceived: " + webRequest.downloadHandler.text);

                //Debug.Log("Gameboard ruls length:" + urlLinks.Length.ToString());

                yield return null;



                if (urlLinks.Length > 0)
                {
                    while (imagePanelUI.loadImageSuccessful == false)
                    {
                        int randomImgIndex = UnityEngine.Random.Range(0, urlLinks.Length);
                        imageIndex = randomImgIndex;
                        Debug.Log("Gameboard image index: " + imageIndex);

                        imageURL = urlLinks[imageIndex];
                        Debug.Log("Gameboard image link: " + imageURL);

                        imagePanelUI.LoadImageFromURL(imageURL);

                        yield return new WaitForSeconds(1f);
                    }


                    //for (int j = 0; j < urlLinks.Length; ++j)
                    //{

                    //    imageURL = urlLinks[j];
                    //    Debug.Log("Gameboard image link: " + j + " " + imageURL);

                    //    imagePanelUI.LoadImageFromURL(imageURL);

                    //    yield return null;
                    //}
                }
            }
        }

        yield return null;
    }

    IEnumerator LoadRandomImage()
    {
        yield return null;

    }


    public void SaveCurrentShapeInfo()
    {
        foreach (ShapeMove shapeMove in SevenShapeMoveList)
        {
            GameBoard.curInfo.SevenShapeInfo.Add(new ShapeInfo(shapeMove.shapeInfo));
        }

        string jsonInfo = JsonUtility.ToJson(GameBoard.curInfo);
        //Debug.Log("game board exit json: " + jsonInfo);
        if (GameBoard.curInfo.MoveShapeSequence.Count > 0)
        {
            formSender.SendInfoToGoogleForm(imageURL, imageIndex.ToString(), jsonInfo);
        }
    //allInformation.AddCurrrentShapeInfo(curInfo);
}

    public void ReportWrongImage()
    {
        string jsonInfo = "wrong image";
        //Debug.Log("game board exit json: " + jsonInfo);

        formSender.SendInfoToGoogleForm(imageURL, imageIndex.ToString(), jsonInfo);

        //allInformation.AddCurrrentShapeInfo(curInfo);
    }

    public void SaveAllShapeInfo()
    {
        //string path = Application.persistentDataPath;
        //Debug.Log("GameBoard " + path);
        //debugText.text = path;

        //string jsonInfo = JsonUtility.ToJson(allInformation);
        //Debug.Log("game board exit json: " + json);

        //formSender.SendInfoToGoogleForm(imageURL, imageIndex.ToString(), jsonInfo);
    }

    // Start is called before the first frame update
    void Start()
    {
        curInfo = new CurrentShapeInformation();

        //allInformation = new AllInformation();
        imageURLList = new List<string>();

        //Get Image
        GetImageURLs();
        //imagePanelUI.SetImage("https://raw.githubusercontent.com/yizhouzhao/Tangram/master/Tangram/Assets/Resources/Problems/5.png");

        //Place tangrams
        StartCoroutine(PlaceShapes());

    }

    IEnumerator PlaceShapes()
    {
        foreach(ShapeMove shape in SevenShapeMoveList)
        {         
            do
            {
                shape.SetRandomPositionRotation();
                yield return new WaitForFixedUpdate();
            } while (shape.overLapList.Count > 0);

            
        }
        yield return null;
    }

    public void LoadCurrentLevel()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitLevel()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
