using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CurrentShapeInformation
{
    public float time;
    public List<ShapeInfo> SevenShapeInfo;

    public CurrentShapeInformation(float ctime)
    {
        time = ctime;
        SevenShapeInfo = new List<ShapeInfo>();
    }
}

public class GameBoard : MonoBehaviour
{
    public Text debugText;

    public List<ShapeMove> SevenShapeMoveList;

    public List<CurrentShapeInformation> AllTimeShapeInfoList;

    public CurrentShapeInformation curInfo;

    public void SaveCurrentShapeInfo()
    {
        curInfo = new CurrentShapeInformation(Time.time);

        foreach (ShapeMove shapeMove in SevenShapeMoveList)
        {
            curInfo.SevenShapeInfo.Add(new ShapeInfo(shapeMove.shapeInfo));
        }

        AllTimeShapeInfoList.Add(curInfo);
    }

    public void SaveAllShapeInfo()
    {
        string path = Application.persistentDataPath;
        Debug.Log("GameBoard " + path);
        debugText.text = path;
    }

    // Start is called before the first frame update
    void Start()
    {
        AllTimeShapeInfoList = new List<CurrentShapeInformation>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
