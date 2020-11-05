using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(FurnitureBoard))]
public class GenerateGrid2 : Editor
{
    string _prefabAssetPath = "Assets/Prefabs/GridCylinder.prefab";
    GameObject _gridPrefab;

    private FurnitureBoard furnitureBoard;

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
        furnitureBoard = target as FurnitureBoard;
        _gridPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_prefabAssetPath);

        //Destroy oringal ones
        foreach (Transform childTransform in furnitureBoard.gameObject.transform)
        {
            UnityEditor.EditorApplication.delayCall += () => { GameObject.DestroyImmediate(childTransform.gameObject); };
        }

        float boardLeft = GTangram.boardCenter.x - GTangram.boardWidth / 2;
        float boardRight = GTangram.boardCenter.x + GTangram.boardWidth / 2;

        float boardTop = GTangram.boardCenter.y + GTangram.boardHeight / 2;
        float boardBottom = GTangram.boardCenter.y - GTangram.boardHeight / 2;

        float step = GTangram.gridGenerateStep;
        float z = furnitureBoard.transform.position.z;

        Debug.Log("Editor");
        for (float x = boardLeft + step; x <= boardRight - step; x += step)
        {
            for (float y = boardBottom + step; y <= boardTop - step; y += step)
            {

                GameObject gridObject = Instantiate(_gridPrefab, furnitureBoard.transform);

                Debug.Log("Editor" + gridObject.transform.position);
                gridObject.transform.position = new Vector3(x, y, 0.1f);
            }
        }

    }
}
