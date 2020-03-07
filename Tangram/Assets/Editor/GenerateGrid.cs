using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(GameBoard))]
public class GenerateGrid : Editor
{
    string _prefabAssetPath = "Assets/Prefabs/GridCylinder.prefab";
    GameObject _gridPrefab;

    private GameBoard gameBoard;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Grids"))
        {
            _generateGrid();
        }
    }

    private void _generateGrid()
    {
        gameBoard = target as GameBoard;
        _gridPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_prefabAssetPath);

        //Destroy oringal ones
        foreach (Transform childTransform in gameBoard.gameObject.transform)
        {
            UnityEditor.EditorApplication.delayCall += () => { GameObject.DestroyImmediate(childTransform.gameObject); };
        }

        float boardLeft = ETangram.boardCenter.x - ETangram.boardWidth / 2;
        float boardRight = ETangram.boardCenter.x + ETangram.boardWidth / 2;

        float boardTop = ETangram.boardCenter.y + ETangram.boardHeight / 2;
        float boardBottom = ETangram.boardCenter.y - ETangram.boardHeight / 2;

        float step = ETangram.gridGenerateStep;
        float z = gameBoard.transform.position.z;

        Debug.Log("Editor");
        for (float x = boardLeft + step; x <= boardRight - step; x += step)
        {
            for (float y = boardBottom + step; y <= boardTop - step; y += step)
            {

                GameObject gridObject = Instantiate(_gridPrefab, gameBoard.transform);

                Debug.Log("Editor" + gridObject.transform.position);
                gridObject.transform.position = new Vector3(x, y, 0.1f);
            }
        }

    }
}
