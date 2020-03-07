using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeMove : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        
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
                //Debug.Log("Pressed right click. rotate" + gameObject.name);
                this.gameObject.transform.Rotate(Vector3.forward, rotAngle, Space.World);
                canRotateR = false;
            }
        }

        
        if (canRotateL)
        {
            //Rotate
            if (Input.GetMouseButtonUp(0))
            {
                //Debug.Log("Pressed right click. rotate" + gameObject.name);
                if (Vector3.Distance(currentPosition, this.transform.position) < 1e-2f)
                {
                    float rotX = rotAngle * Mathf.Deg2Rad;
                    this.gameObject.transform.Rotate(Vector3.forward, -rotAngle, Space.World);
                }
                canRotateL = false;
            }
        }

        if (canFlip)
        {
            if (Input.GetMouseButtonUp(2))
            {
                this.gameObject.transform.Rotate(Vector3.right, 180, Space.World);
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

}
