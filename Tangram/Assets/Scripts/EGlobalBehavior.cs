using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTangram
{
    public static float boardHeight = 10f;
    public static float boardWidth = 10f;

    public static Vector2 boardCenter = new Vector2(-0.5f, 0.5f);
    public static float gridGenerateStep = 1f;
    public static float gridMoveStep = 0.1f;

    public static Dictionary<int, int> triangleFlipDic = new Dictionary<int, int>{
        {0, 6}, {6, 0}, {2 , 4}, { 4, 2}, { 7, 7}, {3, 3}, {1, 5}, {5, 1}
    };

    public static Dictionary<int, int> parallogramFlipDic = new Dictionary<int, int>{
        {0, 4}, {4, 0}, {1 , 7}, { 7, 1}, { 2, 6}, {6, 2}, {3, 5}, {5, 3}
    };

}

public enum EShapeType
{
    Triangle_Small, Triangle_Middle, Triangle_Large, Square, Parallelogram
}

public enum ERotationType
{
    Triangle, Square, Parallelogram
}

public class EGlobalBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
