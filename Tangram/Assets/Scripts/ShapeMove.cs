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

    public ShapeInfo() {}

    public ShapeInfo(ShapeInfo anotherInfo)
    {
        shapeId = anotherInfo.shapeId;
        shapeType = anotherInfo.shapeType;
        shapePosition = anotherInfo.shapePosition;
        shapePositionIds = anotherInfo.shapePositionIds;
        shapeRotation = anotherInfo.shapeRotation;
        shapeRotationId = anotherInfo.shapeRotationId;
    }

    //Mod function
    public static int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

    public void RecordPosition(float x, float y)
    {
        shapePosition[0] = x - GTangram.boardCenter.x;
        shapePosition[1] = y - GTangram.boardCenter.y;

        int idx = Mathf.FloorToInt(shapePosition[0] / GTangram.gridMoveStep + 0.5f);
        int idy = Mathf.FloorToInt(shapePosition[1] / GTangram.gridMoveStep + 0.5f);

        shapePositionIds[0] = idx;
        shapePositionIds[1] = idy;

    }

    public void RecordRotation(Vector3 eularAngle)
    {
        shapeRotation = eularAngle;
    }

    public void SetRotationId(bool isClockWise, int times = 1)
    {
        int clockWise = isClockWise ? 1 : -1;
        //Debug.Log("Shape Move ShapeInfo: " + "set rotation id");
        if (rotationType == ERotationType.Square)
        {

            //shapeRotationId = (shapeRotationId + clockWise) % 6;
            shapeRotationId = ShapeInfo.mod(shapeRotationId + clockWise * times, 6);
        }
        else if(rotationType == ERotationType.Triangle)
        {
            //shapeRotationId = (shapeRotationId + clockWise) % 24;
            shapeRotationId = ShapeInfo.mod(shapeRotationId + clockWise * times, 24);
        }
        else //rotationType == ERotationType.Parallelogram
        {
            if (shapeRotationId >= 0)
            {
                //shapeRotationId = (shapeRotationId + clockWise) % 12;
                shapeRotationId = ShapeInfo.mod(shapeRotationId + clockWise * times, 12);
            }
            else //shapeRotationId < 0
            {
                //shapeRotationId = (shapeRotationId + clockWise) % 12 - 12;
                shapeRotationId = ShapeInfo.mod(shapeRotationId + clockWise * times, 12) - 12;
            }
        }
    }

    public void SetRotationIdWhenFlip()
    {
        if (rotationType == ERotationType.Square) 
        {
            //shapeRotationId = (-shapeRotationId) % 6;
            shapeRotationId = ShapeInfo.mod(-shapeRotationId, 6);
        }
        else if (rotationType == ERotationType.Triangle)
        {
            //Debug.Log("Shape Move SetRotationIdWhenFlip" + " " + shapeRotationId);
            //shapeRotationId = GTangram.triangleFlipDic[shapeRotationId];
            //shapeRotationId = (-shapeRotationId) % 24;
            shapeRotationId = ShapeInfo.mod(-shapeRotationId, 24);

        }
        else //rotationType == ERotationType.Parallelogram
        {
            //shapeRotationId = GTangram.parallogramFlipDic[shapeRotationId];
            if (shapeRotationId >= 0)
            {
                //shapeRotationId = (-shapeRotationId) % 12 - 12;
                shapeRotationId = ShapeInfo.mod(-shapeRotationId, 12) - 12;
            }
            else //shapeRotationId < 0
            {
                //shapeRotationId = (-shapeRotationId) % 12;
                shapeRotationId = ShapeInfo.mod(-shapeRotationId, 12);
            }
        }
    }


}

public class ShapeMove : MonoBehaviour
{
    [Header("Shape info")]
    public ShapeInfo shapeInfo;
    public List<ShapeMove> overLapList = new List<ShapeMove>();

    //Move by left button
    private Vector3 mOffset;
    private float mZCoord;
    private Vector3 currentPosition;

    //Rotate by right button
    private float rotAngle = 15f;
    private bool canRotateR;
    private bool canRotateL;

    //Flip by middle click
    //private float lastClickTime = 0f;
    //private float catchTime = 0.2f;
    private bool canFlip;

    //Control Keycode
    public KeyCode rotationClockWiseCode = KeyCode.LeftArrow;
    public KeyCode rotationCounterClockWiseCode = KeyCode.RightArrow;
    public KeyCode flipCode = KeyCode.UpArrow;
    public KeyCode flipCode2 = KeyCode.DownArrow;

    //selected
    public static ShapeMove selectedShapeMove;



    // Start is called before the first frame update
    void Start()
    {
        //Set init shapeinfo
       // shapeInfo = new ShapeInfo();

        //shapeInfo.shapeRotationId = 3; // For rotation group analysis see https://docs.google.com/presentation/d/109r5e57-rYyy24ohulOeN1EyXAWMG9CRjzNiGJXONoc/edit#slide=id.g813abeac4c_0_28

        //SetRandomPositionRotation();
        //this.transform.position = GetGridPosition(transform.position);

        //string json = JsonUtility.ToJson(shapeInfo);
        //Debug.Log("Shape move: " + json);

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

        this.transform.position = GetGridPosition(new Vector3(x, y, 0f));

        //Set random rotation
        this.transform.rotation = Quaternion.Euler(-45f, 90f, 90f); //init rotation to be on board
        this.shapeInfo.shapeRotationId = 0;

        int rotationTimes = UnityEngine.Random.Range(0, 24);
        //for (int i = 0; i < rotationTimes; i++)
        //{
        //    RotateShape(true);
        //}

        RotateShape(true, rotationTimes);

        //Random Flip
        if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
        {
            FlipShape();
        }

        //Debug.Log("ShapeMove SetRandomPositionRotation " + this.gameObject.name + " " + isOverlap.ToString());
        ////avoid overlapping
        //if (this.isOverlap)
        //{
        //    SetRandomPositionRotation();
        //}
    }

    //Rotate shape clockwisely or counterclockwisely
    void RotateShape(bool isClockWise, int times = 1)
    {
        float clockWise = isClockWise ? 1.0f : -1.0f;
        this.gameObject.transform.Rotate(Vector3.back, rotAngle * clockWise * times, Space.World);

        //Debug.Log("Shape Move rotateshape: " + "");

        //shape information
        for(int i = 0; i < times; ++i)
        {
            shapeInfo.SetRotationId(isClockWise);
        }
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
        
        if(ReferenceEquals(ShapeMove.selectedShapeMove, this))
        {
            if (Input.GetKeyUp(rotationClockWiseCode))
            {
                RotateShape(false);
            }

            if (Input.GetKeyUp(rotationCounterClockWiseCode))
            {
                RotateShape(true);
            }


            if (Input.GetKeyUp(flipCode) || Input.GetKeyUp(flipCode2))
            {
                FlipShape();
            }

        }

        //if (canRotateR)
        //{
        //    if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(rotationClockWiseCode))
        //    {
        //        //Rotate
        //        //Debug.Log("Pressed right click. rotate" + gameObject.name);
        //        RotateShape(false);
        //        canRotateR = false;
        //    }
        //}

        
        //if (canRotateL)
        //{
        //    //Rotate
        //    if (Input.GetMouseButtonUp(0))
        //    {
        //        //Debug.Log("Pressed left click. rotate" + gameObject.name);
        //        if (Vector3.Distance(currentPosition, this.transform.position) < 1e-2f)
        //        {
        //            RotateShape(true);
        //        }
        //        canRotateL = false;
        //    }
        //}

        //if (canFlip)
        //{
        //    if (Input.GetMouseButtonUp(2))
        //    {
        //        //this.gameObject.transform.Rotate(Vector3.right, 180, Space.World);
        //        FlipShape();
        //        canFlip = false;
        //    }
        //}
    }

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
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(rotationClockWiseCode))
        {
            canRotateR = true;
        }

        //Left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            ShapeMove.selectedShapeMove = this;
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

    //snap to grid
    private Vector3 GetGridPosition(Vector3 newPosition)
    {
        float x = Mathf.Floor(newPosition.x / GTangram.gridMoveStep + 0.5f) * GTangram.gridMoveStep;
        float y = Mathf.Floor(newPosition.y / GTangram.gridMoveStep + 0.5f) * GTangram.gridMoveStep;

        Vector3 snapPosition = new Vector3(x, y, newPosition.z);

        //record info
        shapeInfo.RecordPosition(snapPosition.x, snapPosition.y);

        return snapPosition;
    }

    //
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Shape move over lap " + this.gameObject.name + " " + collision.gameObject.name);
    //}

    private void OnTriggerEnter(Collider other)
    {
        
        //Debug.Log("Shape move over lap" + this.gameObject.name + " " + other.gameObject.name);
        if (other.gameObject.tag == "Tangram")
        {
            overLapList.Add(other.gameObject.GetComponent<ShapeMove>());
            //other.gameObject.GetComponent<ShapeMove>().isOverlap = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Tangram")
        {
            overLapList.Remove(other.gameObject.GetComponent<ShapeMove>());
            //other.gameObject.GetComponent<ShapeMove>().isOverlap = false;
        }
    }
}
