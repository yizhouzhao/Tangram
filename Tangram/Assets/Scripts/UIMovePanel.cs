using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMovePanel : MonoBehaviour
{
    public Canvas myCanvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 pos;
        //if (ShapeMove.selectedShapeMove)
        //{
        //    Vector3 shapeScreenPostion = Camera.main.WorldToScreenPoint(ShapeMove.selectedShapeMove.gameObject.transform.position);
        //    RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, shapeScreenPostion, myCanvas.worldCamera, out pos);
        //    transform.position = myCanvas.transform.TransformPoint(pos + new Vector2(30, -50));
        //}
    }

    public void RotateSelectedShape(bool isClockWise)
    {
        if (ShapeMove.selectedShapeMove)
        {
            ShapeMove.selectedShapeMove.RotateShape(isClockWise);
        }
    }

    public void FlipSelectedShape()
    {
        if (ShapeMove.selectedShapeMove)
        {
            ShapeMove.selectedShapeMove.FlipShape();
        }
    }
}
