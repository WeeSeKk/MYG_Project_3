using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class GridManager : MonoBehaviour
{
    [SerializeField] public Grid grid;
    [SerializeField] GameObject boxPrefab;
    [SerializeField] Transform boxParent;
    [SerializeField] UIManager uIManager;

    public GameObject[,] gridArray;
    public List<GameObject> selectedBoxs = new List<GameObject>();

    public int gridWidth = 10;
    public int gridHeight = 7;
    public int cellMaxHeight = 6;
    bool foundEmptyCell = true;
    Coroutine spawnBoxCoroutine;

    void Start()
    {
        EventManager.gameOverEvent += GameOver;
        gridArray = new GameObject[gridWidth, gridHeight];
        StartSpawningBoxes();
    }

    void Update()
    {
        if (Input.GetKeyDown("w"))//test for debug
        {
            //Time.timeScale = 5;
            //FillGrid();

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (gridArray[x, y] != null)
                    {
                        Debug.Log("FULL " + x + " " + y);
                        break;
                    }
                }
            }
        }
    }

    public void StartSpawningBoxes()
    {
        if (spawnBoxCoroutine != null)
        {
            StopCoroutine(spawnBoxCoroutine);
        }
        spawnBoxCoroutine = StartCoroutine(SpawnBox());
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
                EventManager.GameOverEvent();
                break;
            } 

            while (attempts < maxAttempts)//attempt to find a place to spawn a new box
            {
                attempts++;

                System.Random rand = new System.Random();
                int x = rand.Next(0, gridWidth);
                int y = 6;

                if (gridArray[x, y] == null)//found a empty cell
                {
                    Vector3 worldPosition = grid.CellToWorld(new Vector3Int(x, y));//create it's position in the grid

                    GameObject newBox = ObjectPool.BoxSpawn(boxPrefab, worldPosition, Quaternion.identity);//add gameobject to the pool
                    newBox.transform.SetParent(boxParent);

                    gridArray[x, y] = newBox;//add it to the array
                    break;
                } 
            }
        }
    }

    public void UpdateArray(GameObject go, int x, int y)//update the array
    {
        for (int a = 0; a < gridWidth; a ++)
        {
            for (int b = 0; b < gridHeight; b ++)
            {
                if (gridArray[a, b] == go)
                {
                    gridArray[a, b] = null;
                    break;
                }
            }
        }
        gridArray[x, y] = go;
    }

    public IEnumerator RemoveBox()//destroy all the boxes used to create a word
    {
        for (int i = 0; i < selectedBoxs.Count; i++)//for every box in the list
        {
            GameObject box = selectedBoxs[i];

            if (box != null)
            {
                for (int x = 0; x < gridWidth; x++)//for every x position
                {
                    for (int y = 0; y < gridHeight; y++)//for every y position
                    {
                        if (gridArray[x, y] == box)//once we found the box in the array
                        {
                            gridArray[x, y] = null;
                            
                            
                        }
                    }
                }
            }

            foreach (GameObject go in selectedBoxs)
            {
                BoxsFly(go);
            }
            
            yield return new WaitForSeconds(0.5f);
            
            foreach (GameObject boxs in selectedBoxs)
            {
                if (boxs != null)
                {
                    ObjectPool.ReturnObjectToPool(boxs); //disable object and add it to the inactive object list
                }
            }
        }
        selectedBoxs.Clear();
    }

    public void ResetGridAndArray()
    {
        for (int x = 0; x < gridWidth; x++)//for every x position
        {
            for (int y = 0; y < gridHeight; y++)//for every y position
            {
                if (gridArray[x, y] != null)//once we found a != null location in the array
                {
                    gridArray[x, y] = null;
                    break;
                }
            }
        }

        foreach (Transform child in boxParent)
        {
            Destroy(child.gameObject);
        }

        selectedBoxs.Clear();
        foundEmptyCell = true;
    }

    void GameOver()
    {
        if (spawnBoxCoroutine != null)
        {
            StopCoroutine(spawnBoxCoroutine);
        }
    }

    void BoxsFly(GameObject gameObject)
    {
        System.Random random = new System.Random();
        int i = random.Next(0, 20);

        Vector2 jumpEnd = new Vector2(i, 10);
        gameObject.transform.DOJump(jumpEnd, 2f, 1, 1f, false);
    }

    void FillGrid()
    {
        for (int x = 0; x < gridWidth; x++)//for every x position
        {
            for (int y = 0; y < gridHeight; y++)//for every y position
            {
                Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(x, y));
                Instantiate(boxPrefab, worldPosition, Quaternion.identity);
            }
        }
    }
}