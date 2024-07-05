using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]  public Grid grid;
    [SerializeField] GameObject boxPrefab;
    [SerializeField] Transform boxParent;

    public GameObject[,] gridArray;
    public List<GameObject> boxs = new List<GameObject>();

    public int gridWidth = 10;
    public int gridHeight = 10;
    public int cellMaxHeight = 6;
    bool foundEmptyCell = true;

    void Start()
    {
        gridArray = new GameObject[gridWidth, gridHeight];
        StartCoroutine(SpawnBox());
    }

    void Update()
    {

    }

    IEnumerator SpawnBox()
    {
        while (foundEmptyCell == true)
        {
            yield return new WaitForSeconds(1.5f);
            foundEmptyCell = false;
            int maxAttempts = 100; 
            int attempts = 0;

            for (int i = 0; i < gridWidth; i++)//look for an empty cell
            {
                if(gridArray[i, cellMaxHeight] == null)
                {
                    foundEmptyCell = true;
                    break;
                }
            }

            if (!foundEmptyCell)//all the cells are full so game over
            {
                //game over
                Debug.Log("GAME OVER ARRAY FULL");
                break;
            } 

            while (attempts < maxAttempts)//attempt to find a place to spawn a new box
            {
                attempts++;

                System.Random rand = new System.Random();
                int x = rand.Next(0, gridWidth);
                int y = cellMaxHeight;

                if (gridArray[x, y] == null)//found a empty cell
                {
                    Vector3 worldPosition = grid.CellToWorld(new Vector3Int(x, y, 0));
                    GameObject newBox = Instantiate(boxPrefab, worldPosition, Quaternion.identity, boxParent);

                    gridArray[x, y] = newBox;
                    break;
                } 
            }
        }
    }

    public void UpdateArray(GameObject go, int x, int y)//update the array
    {
        gridArray[x, y] = null;
        gridArray[x, y - 1] = go;
    }

    public void RemoveBox()//destroy all the boxes used to create a word
    {
        for (int i = 0; i < boxs.Count; i++)//for every boxs in the list
        {
            for (int x = 0; x < gridWidth; x++)//for every x position
            {
                for (int y = 0; y < gridHeight; y++)//for every y position
                {
                    if (gridArray[x, y] == boxs[i])//once we found the box in the array
                    {
                        gridArray[x, y] = null;
                        Destroy(boxs[i]);
                        Debug.Log(gridArray[x, y]);
                        break;
                    }
                }
            }
        }
        boxs.Clear();
    }
}