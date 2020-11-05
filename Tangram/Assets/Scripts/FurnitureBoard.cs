using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CurrentShapeInformation2
{
    public List<ShapeInfo2> allShapeInfo;

    public CurrentShapeInformation2()
    {
        allShapeInfo = new List<ShapeInfo2>();
    }
}

public class FurnitureBoard : MonoBehaviour
{
    [Header("Shapes")]
    private List<ShapeMove2> registeredShapes;
    public List<ShapeMove2> activeShapes = new List<ShapeMove2>();
    public CurrentShapeInformation2 curInfo;

    [Header("Google form sender")]
    public GoogleFormSender formSender;
    // Start is called before the first frame update
    void Start()
    {
        // collect all tans(furniture)
        registeredShapes = new List<ShapeMove2>();
        GameObject[] tans = GameObject.FindGameObjectsWithTag("Tangram");
        foreach(GameObject tan in tans)
        {
            registeredShapes.Add(tan.GetComponent<ShapeMove2>());
        }
        // collect tans in scene
        curInfo = new CurrentShapeInformation2();
        int id_index = 0;
        foreach(ShapeMove2 shape in registeredShapes)
        {
            bool inBoard = true;
            if (shape.gameObject.activeSelf)
            {
                Vector3 pos = shape.gameObject.transform.position;
                if (pos.x < - GTangram.boardWidth / 2 + GTangram.boardCenter.x || pos.y < -GTangram.boardHeight / 2 + GTangram.boardCenter.y)
                {
                    inBoard = false;
                }
                if (pos.x > GTangram.boardWidth / 2 + GTangram.boardCenter.x || pos.y > GTangram.boardHeight / 2 + GTangram.boardCenter.y)
                {
                    inBoard = false;
                }
                if (inBoard)
                {
                    shape.shapeInfo.shapeId = id_index;
                    id_index++;
                    activeShapes.Add(shape);
                }
            }
        }
        Debug.Log("Furniture Board start: " + activeShapes.Count.ToString());
    }

    public void CollectCurrentInfo()
    {
        curInfo.allShapeInfo.Clear();
        foreach (ShapeMove2 shape in activeShapes)
        {
            curInfo.allShapeInfo.Add(shape.shapeInfo);
        }
    }

    public void SendInfo()
    {
        CollectCurrentInfo();
        string jsonInfo = JsonUtility.ToJson(curInfo);
        Debug.Log("Furniture Board: " + jsonInfo);
        formSender.SendInfoToGoogleForm("furniture", "", jsonInfo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
