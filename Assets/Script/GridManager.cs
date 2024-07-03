using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]  public Grid grid;
    [SerializeField] GameObject boxPrefab;
    [SerializeField] Transform boxParent;

    public GameObject[,] gridArray;

    public int gridWidth = 10;
    public int gridHeight = 10;

    void Start()
    {
        gridArray = new GameObject[gridWidth, gridHeight];
        //test();
    }

    // Update is called once per frame
    void Update()
    {
       if (Input.GetKeyDown("space"))
        {
            RemoveBox(2, 3);
        }
        if (Input.GetKeyDown("z"))
        {
            SpawnBox();
        }
        if (Input.GetKeyDown("w"))
        {
            test();
        }
    }

    void SpawnBox()
    {
        int maxAttempts = 100; 
        int attempts = 0;
        bool foundEmptyCell = false;

        for (int i = 0; i < gridWidth; i++)
        {
            if(gridArray[i, 2] == null)
            {
                foundEmptyCell = true;
                break;
            }
        }

        if (!foundEmptyCell)
        {
            //game over
            Debug.Log("GAME OVER ARRAY FULL");
            return;
        } 

        while (attempts < maxAttempts)
        {
            attempts++;

            System.Random rand = new System.Random();
            int x = rand.Next(0, gridWidth);
            int y = 2;

            if (gridArray[x, y] == null)
            {

                Vector3 worldPosition = grid.CellToWorld(new Vector3Int(x, y, 0));
                GameObject newBox = Instantiate(boxPrefab, worldPosition, Quaternion.identity, boxParent);
                

                gridArray[x, y] = newBox;
                return;
            } 
        }
        Debug.LogError("BROKEN AF");
    }

    public void UpdateArray(GameObject go, int x, int y)
    {
        gridArray[x, y] = null;
        gridArray[x, y - 1] = go;
    }

    void test()
    {
        for (int y = 0; y < 7; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                Vector3 worldPosition = grid.CellToWorld(new Vector3Int(x, y, 0));
                GameObject newBox = Instantiate(boxPrefab, worldPosition, Quaternion.identity, boxParent);

                gridArray[x, y] = newBox;
            }
        }
    }

    void RemoveBox(int x, int y)
    {
        Destroy(gridArray[x, y]);
        gridArray[x, y] = null;
        Debug.Log("DESTROYED");
    }
}