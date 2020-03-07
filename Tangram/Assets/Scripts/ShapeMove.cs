using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShapeInfo
{
    [Header("Identity")]
    public int shapeId;
    public EShapeType shapeType;
    public ERotationType rotationType;

    [Header("Position and Rotation")]
    public Vector2 shapePosition;
    public Vector2 shapePositionIds;

    public Vector3 shapeRotation;
    public int shapeRotationId;


    public void RecordPosition(float x, float y)
    {
        shapePosition[0] = x;
        shapePosition[1] = y;

        int idx = Mathf.FloorToInt(x / GTangram.gridMoveStep + 0.5f);
        int idy = Mathf.FloorToInt(y / GTangram.gridMoveStep + 0.5f);

        shapePositionIds[0] = idx;
        shapePositionIds[1] = idy;

    }

    public void RecordRotation(Vector3 eularAngle)
    {
        shapeRotation = eularAngle;
    }

    public void SetRotationId(bool isClockWise)
    {
        //Debug.Log("Shape Move ShapeInfo: " + "set rotation id");
        if (rotationType == ERotationType.Square)
        {
            shapeRotationId = (shapeRotationId + 1) % 2;
        }
        else if(rotationType == ERotationType.Triangle)
        {
            int clockWise = isClockWise ? 1 : -1;
            shapeRotationId = (shapeRotationId + clockWise) % 8;
        }
        else //rotationType == ERotationType.Parallelogram
        {
            int clockWise = isClockWise ? 1 : -1;
            if (shapeRotationId < 4)
            {
                shapeRotationId = (shapeRotationId + clockWise) % 4;
            }
            else
            {
                shapeRotationId = 4 + (shapeRotationId + clockWise) % 4;
            }
        }
    }

    public void SetRotationIdWhenFlip()
    {
        if (rotationType == ERotationType.Square) {}
        else if (rotationType == ERotationType.Triangle)
        {
            shapeRotationId = GTangram.triangleFlipDic[shapeRotationId];

        }
        else //rotationType == ERotationType.Parallelogram
        {
            shapeRotationId = GTangram.parallogramFlipDic[shapeRotationId];
        }
    }


}

public class ShapeMove : MonoBehaviour
{
    [Header("Shape info")]
    public ShapeInfo shapeInfo;


    //Move by left button
    private Vector3 mOffset;
    private float mZCoord;
    private Vector3 currentPosition;

    //Rotate by right button
    private float rotAngle = 45f;
    private bool canRotateR;
    private bool canRotateL;

    //Flip by middle click
    //private float lastClickTime = 0f;
    //private float catchTime = 0.2f;
    private bool canFlip;

    // Start is called before the first frame update
    void Start()
    {
        //shapeInfo = new ShapeInfo();

        SetRandomPositionRotation();
        this.transform.position = GetGridPosition(transform.position);

        string json = JsonUtility.ToJson(shapeInfo);
        Debug.Log("Shape move: " + json);
        
    }

    public void SetRandomPositionRotation()
    {
        //Set random position
        float boardLeft = GTangram.boardCenter.x - GTangram.boardWidth / 2;
        float boardRight = GTangram.boardCenter.x + GTangram.boardWidth / 2;

        float boardBottom = GTangram.boardCenter.y - GTangram.boardHeight / 2;
        float boardTop = GTangram.boardCenter.y + GTangram.boardHeight / 2;

        float x = UnityEngine.Random.Range(boardLeft + GTangram.gridGenerateStep, boardRight - GTangram.gridGenerateStep);
        float y = UnityEngine.Random.Range(boardBottom + GTangram.gridGenerateStep, boardTop - GTangram.gridGenerateStep);

        this.transform.position = new Vector3(x, y, 0f);

        //Set random rotation
        this.transform.rotation = Quaternion.Euler(0f, 90f, 90f);

        int rotationTimes = UnityEngine.Random.Range(0, 8);
        for(int i = 0; i < rotationTimes; i++)
        {

        }

    }

    //Rotate shape clockwisely or counterclockwisely
    void RotateShape(bool isClockWise)
    {
        float clockWise = isClockWise ? 1.0f : -1.0f;
        this.gameObject.transform.Rotate(Vector3.back, rotAngle * clockWise, Space.World);

        Debug.Log("Shape Move rotateshape: " + "");

        //shape information
        shapeInfo.SetRotationId(isClockWise);
        shapeInfo.RecordRotation(this.transform.rotation.eulerAngles);
    }

    //flip the shape up/down
    void FlipShape()
    {
        this.gameObject.transform.Rotate(Vector3.right, 180, Space.World);

        //shape information
        shapeInfo.SetRotationIdWhenFlip();
        shapeInfo.RecordRotation(this.transform.rotation.eulerAngles);

    }


    // Update is called once per frame
    void Update()
    {
        ////Right mouse button
        //if (Input.GetMouseButtonUp(1))
        //{
        //    canRotateR = false;
        //}
        if (canRotateR)
        {
            if (Input.GetMouseButtonUp(1))
            {
                //Rotate
                Debug.Log("Pressed right click. rotate" + gameObject.name);
                RotateShape(false);
                canRotateR = false;
            }
        }

        
        if (canRotateL)
        {
            //Rotate
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Pressed left click. rotate" + gameObject.name);
                if (Vector3.Distance(currentPosition, this.transform.position) < 1e-2f)
                {
                    RotateShape(true);
                }
                canRotateL = false;
            }
        }

        if (canFlip)
        {
            if (Input.GetMouseButtonUp(2))
            {
                //this.gameObject.transform.Rotate(Vector3.right, 180, Space.World);
                FlipShape();
                canFlip = false;
            }
        }
    }

    //private void OnMouseDown()
    //{

    //}

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + mOffset;

        //Debug.Log("Shape move: " + transform.position);
        //Debug.Log("Shape move after: " + GetGridPosition(transform.position));

        this.transform.position = GetGridPosition(transform.position);
    }

    private void OnMouseOver()
    {
        //Right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            canRotateR = true;
        }

        //Left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            canRotateL = true;
            //Debug.Log("ShapeMove");
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            mOffset = gameObject.transform.position - GetMouseWorldPos();
            currentPosition = this.transform.position;

            //if (Time.time - lastClickTime < catchTime)
            //{
            //    //double click
            //    print("double click done:" + (Time.time - lastClickTime).ToString());
            //    canFlip = true;
            //}
            //else
            //{
            //    //normal click
            //    print("miss:" + (Time.time - lastClickTime).ToString());
            //}
            //lastClickTime = Time.time;
        }

        //Middle button
        if (Input.GetMouseButtonDown(2))
        {
            canFlip = true;
        }
    }


    private Vector3 GetGridPosition(Vector3 newPosition)
    {
        float x = Mathf.Floor(newPosition.x / GTangram.gridMoveStep + 0.5f) * GTangram.gridMoveStep;
        float y = Mathf.Floor(newPosition.y / GTangram.gridMoveStep + 0.5f) * GTangram.gridMoveStep;

        Vector3 snapPosition = new Vector3(x, y, newPosition.z);

        //record info
        shapeInfo.RecordPosition(snapPosition.x, snapPosition.y);

        return snapPosition;
    }
}
