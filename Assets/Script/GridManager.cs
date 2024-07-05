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
    public List<GameObject> selectedBoxs = new List<GameObject>();

    public int gridWidth = 10;
    public int gridHeight = 7;
    public int cellMaxHeight = 6;
    bool foundEmptyCell = true;

    void Start()
    {
        gridArray = new GameObject[gridWidth, gridHeight];
        StartCoroutine(SpawnBox());
        
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))//test for debug
        {
            Time.timeScale = 5;
        }
        if (Input.GetKeyDown("w"))//test for debug
        {
            StartCoroutine(SpawnBox());
        }
    }

    IEnumerator SpawnBox()
    {
        while (foundEmptyCell == true)
        {
            yield return new WaitForSeconds(1f);
            int maxAttempts = 100; 
            int attempts = 0;
            foundEmptyCell = false;

            for (int x = 0; x < gridWidth; x++)//look for an empty cell
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (gridArray[x, y] == null)
                    {
                        foundEmptyCell = true;
                        break;
                    } 
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
                    Vector3 worldPosition = grid.CellToWorld(new Vector3Int(x, y));//create it's position in the grid
                    GameObject newBox = Instantiate(boxPrefab, worldPosition, Quaternion.identity, boxParent);//instantiate the object 

                    gridArray[x, y] = newBox;//add it to the array
                    break;
                } 
            }
        }
    }

    public void UpdateArray(GameObject go, int x, int y)//update the array
    {
        gridArray[x, y - 1] = go;
        gridArray[x, y] = null;
    }

    public void RemoveBox()//destroy all the boxes used to create a word
    {
        for (int i = 0; i < selectedBoxs.Count; i++)//for every boxs in the list
        {
            for (int x = 0; x < gridWidth; x++)//for every x position
            {
                for (int y = 0; y < gridHeight; y++)//for every y position
                {
                    if (gridArray[x, y] == selectedBoxs[i])//once we found the box in the array
                    {
                        gridArray[x, y] = null;
                        Destroy(selectedBoxs[i]);
                        break;
                    }
                }
            }
        }
        selectedBoxs.Clear();
    }
}