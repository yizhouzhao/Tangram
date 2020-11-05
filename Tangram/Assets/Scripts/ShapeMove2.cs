using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShapeInfo2
{
    [Header("Identity")]
    public int shapeId;
    public EFurnitureType furnitureType;
    public ERotationType rotationType;

    [Header("Position and Rotation")]
    public Vector2 shapePosition;
    public Vector2 shapePositionIds;

    public Vector3 shapeRotation;
    public int shapeRotationId;

    public ShapeInfo2() { }

    public ShapeInfo2(ShapeInfo2 anotherInfo)
    {
        shapeId = anotherInfo.shapeId;
        furnitureType = anotherInfo.furnitureType;
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
        if (rotationType == ERotationType.Circle)
        {
            shapeRotationId = 0;
        }
        else if (rotationType == ERotationType.Square)
        {

            //shapeRotationId = (shapeRotationId + clockWise) % 6;
            shapeRotationId = ShapeInfo.mod(shapeRotationId + clockWise * times, 6);
        }
        else if (rotationType == ERotationType.Triangle)
        {
            //shapeRotationId = (shapeRotationId + clockWise) % 24;
            shapeRotationId = ShapeInfo.mod(shapeRotationId + clockWise * times, 24);
        }
        else if (rotationType == ERotationType.Rectangle)
        {
            shapeRotationId = ShapeInfo.mod(shapeRotationId + clockWise * times, 12);
        }
        else //rotationType == ERotationType.Parallelogram or Rectangle
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
}

public class ShapeMove2 : MonoBehaviour
{
    [Header("Shape info")]
    public ShapeInfo2 shapeInfo;
    public List<ShapeMove2> overLapList = new List<ShapeMove2>();

    //Move by left button
    private Vector3 mOffset;
    private float mZCoord;
    private Vector3 currentPosition;

    //Rotate by right button
    private float rotAngle = 15f;
    private bool canRotateR;
    private bool canRotateL;

    //Control Keycode
    private KeyCode rotationClockWiseCode = KeyCode.D;
    private KeyCode rotationCounterClockWiseCode = KeyCode.A;

    //selected
    public static ShapeMove2 selectedShapeMove;
    public GameObject highlightSphere;

    // Start is called before the first frame update
    void Start()
    {
        highlightSphere = this.transform.Find("HighlightSphere").gameObject;
        highlightSphere.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (ReferenceEquals(ShapeMove2.selectedShapeMove, this))
        {
            if (Input.GetKeyUp(rotationClockWiseCode) || Input.GetMouseButtonUp(1))
            {
                RotateShape(true);
            }

            if (Input.GetKeyUp(rotationCounterClockWiseCode))
            {
                RotateShape(false);
            }
        }
    }

    //Rotate shape clockwisely or counterclockwisely
    public void RotateShape(bool isClockWise, int times = 1)
    {
        float clockWise = isClockWise ? 1.0f : -1.0f;
        this.gameObject.transform.Rotate(Vector3.back, rotAngle * clockWise * times, Space.World);

        //Debug.Log("Shape Move rotateshape: " + "");

        //shape information
        for (int i = 0; i < times; ++i)
        {
            shapeInfo.SetRotationId(isClockWise);
        }
        shapeInfo.RecordRotation(this.transform.rotation.eulerAngles);
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        //if (EventSystem.current.IsPointerOverGameObject())
        //{
        //    return;
        //}
        transform.position = GetMouseWorldPos() + mOffset;

        //Debug.Log("Shape move: " + transform.position);
        //Debug.Log("Shape move after: " + GetGridPosition(transform.position));

        this.transform.position = GetGridPosition(transform.position);
    }

    public void ToggleHighLight()
    {
        highlightSphere.SetActive(highlightSphere.activeSelf ? false : true);
    }

    private void OnMouseOver()
    {
        ////Right mouse button
        //if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(rotationClockWiseCode))
        //{
        //    canRotateR = true;
        //}

        //Left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            //if (EventSystem.current.IsPointerOverGameObject())
            //{
            //    return;
            //}

            //Record movement sequence
            if (!ReferenceEquals(ShapeMove2.selectedShapeMove, this))
            {
                //GameBoard.curInfo.AddMoveShapeToSequence(this.shapeInfo.shapeId);
                if (ShapeMove2.selectedShapeMove)
                {
                    ShapeMove2.selectedShapeMove.ToggleHighLight();
                }
                this.ToggleHighLight();
            }

            ShapeMove2.selectedShapeMove = this;
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

        ////Middle button
        //if (Input.GetMouseButtonDown(2))
        //{
        //    canFlip = true;
        //}
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
}
