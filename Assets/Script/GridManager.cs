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
    [SerializeField] GameObject bigBoxPrefab;
    [SerializeField] GameObject deathBoxPrefab;
    [SerializeField] GameObject fireBoxPrefab;
    [SerializeField] Transform boxParent;
    [SerializeField] AnimationManager animationManager;

    public GameObject[,] gridArray;
    public List<GameObject> selectedBoxs = new List<GameObject>();

    public int gridWidth = 10;
    public int gridHeight = 10;
    public int cellMaxHeight = 6;
    bool foundEmptyCell = true;
    Coroutine spawnBoxCoroutine;

    void Start()
    {
        EventManager.gameOverEvent += StopSpawningBoxes;
        gridArray = new GameObject[gridWidth, gridHeight];
        StartSpawningBoxes();
    }

    void Update()
    {
        if (Input.GetKeyDown("w"))//test for debug
        {
            FillGrid();
        }
        if (Input.GetKeyDown("z"))//test for debug
        {
            SpawnSpecialBoxs("crusher");
        }
        if (Input.GetKeyDown("q"))//test for debug
        {
            SpawnSpecialBoxs("death");
        }
        if (Input.GetKeyDown("e"))//test for debug
        {
            SpawnSpecialBoxs("fire");
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

    public void StopSpawningBoxes()
    {
        if (spawnBoxCoroutine != null)
        {
            StopCoroutine(spawnBoxCoroutine);
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
                for (int y = 0; y < 7; y++)
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

    void SpawnSpecialBoxs(string type)
    {
        int maxAttempts = 100; 
        int attempts = 0;
        
        while (attempts < maxAttempts)//attempt to find a place to spawn a new box
        {
            GameObject specialBox = new GameObject();

            attempts++;

            int x = 0;
            int y = 0;

            if (type == "crusher")
            {
                specialBox = bigBoxPrefab;

                x = 5;
                y = 9;
                
            }
            if (type == "death")
            {
                specialBox = deathBoxPrefab;

                System.Random rand = new System.Random();
                x = rand.Next(0, gridWidth);
                y = 6;
            }
            if (type == "fire")
            {
                specialBox = fireBoxPrefab;

                System.Random rand = new System.Random();
                x = rand.Next(0, gridWidth);
                y = 6;
            }
            

            if (gridArray[x, y] == null)//found a empty cell
            {
                Vector3 worldPosition = grid.CellToWorld(new Vector3Int(x, y));//create it's position in the grid

                GameObject newBox = ObjectPool.BoxSpawn(specialBox, worldPosition, Quaternion.identity);
                newBox.transform.SetParent(boxParent);

                gridArray[x, y] = newBox;//add it to the array
                break;
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

    public IEnumerator RemoveSelectedBox() //Destroy all the boxes used to create a word
{
    List<GameObject> boxesToRemove = new List<GameObject>();

    for (int i = 0; i < selectedBoxs.Count; i++) // For every box in the list
    {
        foreach (GameObject box in selectedBoxs)
        {
            for (int x = 0; x < gridWidth; x++) // For every x position
            {
                for (int y = 0; y < gridHeight; y++) // For every y position
                {
                    if (gridArray[x, y] == box) // Once we found the box in the array
                    {
                        gridArray[x, y] = null;
                        boxesToRemove.Add(box);
                    }
                }
            }
            
            if (box != null)
            {
                animationManager.BoxsFly(box);
            }
        }
    }

    yield return new WaitForSeconds(1f);

    foreach (GameObject box in boxesToRemove)
    {
        ObjectPool.ReturnObjectToPool(box); 
    }

    selectedBoxs.Clear();
}

    public void RemoveBoxs(GameObject gameObject)
    {
        for (int x = 0; x < gridWidth; x++)//for every x position
        {
            for (int y = 0; y < gridHeight; y++)//for every y position
            {
                if (gridArray[x, y] == gameObject)//once we found the box in the array
                {
                    gridArray[x, y] = null;
                    ObjectPool.ReturnObjectToPool(gameObject);
                }
            }
        }
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

    void FillGrid()//for debug and test
    {
        for (int x = 0; x < gridWidth; x++)//for every x position
        {
            for (int y = 0; y < 6; y++)//for every y position
            {
                Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(x, y));
                GameObject newBox = ObjectPool.BoxSpawn(boxPrefab, worldPosition, Quaternion.identity);//add gameobject to the pool
                gridArray[x, y] = newBox;
            }
        }
    }
}