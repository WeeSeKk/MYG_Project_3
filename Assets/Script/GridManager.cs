using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    public GameObject[,] gridArray;
    [SerializeField] public Grid grid;
    [SerializeField] Transform boxParent;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] AnimationManager animationManager;
    [SerializeField] ObjectPool objectPool;
    [SerializeField] GameObject defaultBox;
    Scene currentScene;
    public List<GameObject> selectedBoxs = new List<GameObject>();
    public List<GameObject> powerUp = new List<GameObject>();
    public int gridWidth = 12;
    public int maxgridWidth = 11;
    public int gridHeight = 10;
    public int cellMaxHeight = 7;
    bool foundEmptyCell = true;
    public bool gameOver = false;

    void Awake()
    {
        EventManager.gameOverEvent += GameOver;
        gridArray = new GameObject[gridWidth, gridHeight];
        currentScene = SceneManager.GetActiveScene();
    }
    /**
        <summary>
        Add gameobject to the grid.
        </summary>
        <param name="gameObject">Gameobject to moove to the array</param>
        <returns></returns>
    **/
    public void SpawnBox(GameObject gameObject)
    {
        foundEmptyCell = false;
        int maxAttempt = 100;
        int attempt = 0;
        bool safeguard = false;

        for (int a = 0; a < maxgridWidth; a++)//look for an empty cell
        {
            for (int b = 0; b < 7; b++)
            {
                if (gridArray[a, b] == null)
                {
                    foundEmptyCell = true;
                    break;
                } 
            }
        }

        if (!foundEmptyCell)//all the cells are full so game over
        {
            if (currentScene.name == "Scene_Gamemode_01")
            {
                EventManager.GameOverEvent();
            }
        } 

        safeguard = true;

        for (int a = 0; a < maxgridWidth; a++)
        {
            if (gridArray[a, cellMaxHeight] == null)
            {
                safeguard = false;
                break;
            }
        }

        while (attempt < maxAttempt && !gameOver && !safeguard)
        {
            System.Random rand = new System.Random();
            int x = rand.Next(0, maxgridWidth);
            int y = 9;

            if (gridArray[x, y] == null && gridArray[x, cellMaxHeight] == null)//found a empty cell
            {
                Vector3 worldPosition = grid.CellToWorld(new Vector3Int(x, y));//create it's position in the grid

                gameObject.transform.DOMove(worldPosition, 0.1f, false).SetEase(Ease.OutCirc).OnComplete(() => {

                    gridArray[x, y] = gameObject;//add it to the array  
                });
                break;    
            } 
        }
    }
    /**
        <summary>
        Add powerUp to the grid.
        </summary>
        <param name="type">PowerUP from the list.</param>
        <returns></returns>
    **/
    public void SpawnPowerUp(int type)
    {
        GameObject newBox = ObjectPool.BoxSpawn(powerUp[type], spawnPoint.transform.position, Quaternion.identity);
        newBox.transform.SetParent(boxParent.transform);

        SpawnBox(newBox);
    }
    /**
        <summary>
        Update the position of a gameobject on the grid.
        </summary>
        <param name="go">Gameobject to update.</param>
        <param name="x">New x position of the gameobject.</param>
        <param name="y">New y position of the gameobject.</param>
        <returns></returns>
    **/
    public void UpdateArray(GameObject go, int x, int y)
    {
        for (int a = 0; a < maxgridWidth; a ++)
        {
            for (int b = 0; b < gridHeight; b ++)
            {
                if (gridArray[a, b] == go)
                {
                    gridArray[a, b] = null;
                }
            }
        }
        gridArray[x, y] = go;
    }
    /**
        <summary>
        Return to the pool all gameobject selected by the player.
        </summary>
        <param name=""></param>
        <returns></returns>
    **/
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
    /**
        <summary>
        Remove gameobject who are not selected by the player expl : powerUP.
        </summary>
        <param name="gameObject">Gameobject to return to the pool.</param>
        <returns></returns>
    **/
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
                    selectedBoxs.Remove(gameObject);
                }
            }
        }
    }
    /**
        <summary>
        Empty the grid (only for debug).
        </summary>
        <param name=""></param>
        <returns></returns>
    **/
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
        gameOver = true;
    }
}