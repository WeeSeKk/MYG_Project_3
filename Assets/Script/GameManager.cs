using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using JetBrains.Annotations;

public class GameManager : MonoBehaviour
{
    static readonly Dictionary<GameObject, int> boxFrequencies = new Dictionary<GameObject, int>();
    [SerializeField] GridManager gridManager;
    [SerializeField] WordsManager wordsManager;
    [SerializeField] UIManager uIManager;
    [SerializeField] TimerScript timerScript;
    [SerializeField] GameObject boxParent;
    [SerializeField] GameObject boxPosBeforeArray;
    [SerializeField] Grid lineGrid;
    public List<GameObject> boxsPrefab;
    public GameObject[,] spawnPosition;
    int currentGamemode;
    int gridWidth = 1;
    int gridHeight = 10;
    bool gameOver;
    int score;


    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = new GameObject[gridWidth, gridHeight];

        EventManager.gameOverEvent += GameOver;
        EventManager.resetEvent += ResetCurrentGamemode;

        currentGamemode = 1;

        InitializeBoxFrequencies();

        StartCoroutine(SpawnNewBoxs());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))//test for debug
        {
            Time.timeScale = 5;
        }
    }

    void InitializeBoxFrequencies()
    {
        boxFrequencies.Add(boxsPrefab[0], 100);//default box
        boxFrequencies.Add(boxsPrefab[1], 0);//crusher box
        boxFrequencies.Add(boxsPrefab[2], 0);//skull box
        boxFrequencies.Add(boxsPrefab[3], 0);//fire box
        boxFrequencies.Add(boxsPrefab[4], 0);//magnet box
        boxFrequencies.Add(boxsPrefab[5], 50);//bomb box 
    }

    GameObject GenerateBox()
    {
        int totalWeight = 0;
        foreach (var weight in boxFrequencies.Values)
        {
            totalWeight += weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        foreach (var box in boxFrequencies.Keys)
        {
            if (randomValue < boxFrequencies[box])
            {
                return box;
            }
            randomValue -= boxFrequencies[box];
        }

        return boxsPrefab[0];
    }

    void SetupGameMode(int gamemode)
    {
        //do different parameters for different gamemodes 
        //gamemode 1
        //gamemode 2
        //etc ..

        if(gamemode == 1)
        {
            //gamemode parameters

            //gridManager.StartSpawningBoxes();
            timerScript.SetupTimer(60f);

        }
    }

    public void CountScore(int value)
    {
        int baseLetterPoint = 15;

        baseLetterPoint -= value;

        score += baseLetterPoint;

        if (value < 10)
        {
            timerScript.AddTimerTime(1);
        }
        else 
        {
            timerScript.AddTimerTime(5);
        }

        Debug.Log(score);
    }

    void ResetCurrentGamemode(bool retry)
    {
        //reset all parameters for all gamemodes

        gridManager.ResetGridAndArray();
        timerScript.ResetTimer();

        if(retry == true)
        {
           SetupGameMode(currentGamemode); 
        }
    }

    public IEnumerator SpawnNewBoxs()
    {
        int x = 0;
        int y = 0;

        while (!gameOver)
        {
            GameObject box = GenerateBox();

            for (int a = 0; a < gridManager.gridWidth; a++)//prevent a second magnet from spawning if there is already one in one of the array
            {
                for (int b = 0; b < gridManager.gridHeight; b++)
                {
                    if (gridManager.gridArray[a, b] != null && gridManager.gridArray[a, b].name == "magnetBoxSquare(Clone)" || spawnPosition[0, b] != null && spawnPosition[0, b].name == "magnetBoxSquare(Clone)")
                    {
                        if (box.name == "magnetBoxSquare")
                        {
                            box = boxsPrefab[0];
                        }
                    }   
                }
            }
            
            Vector3 worldPosition = lineGrid.CellToWorld(new Vector3Int(x, y));
            GameObject newBox = ObjectPool.BoxSpawn(box, worldPosition, Quaternion.identity);

            newBox.transform.SetParent(boxParent.transform);
            spawnPosition[x, y] = newBox;

            MoveBoxs(newBox);

            yield return new WaitForSeconds(1f);
        }
    }

    void MoveBoxs(GameObject go)
    {
        for (int i = gridHeight - 1; i >= 0; i--)
        {
            if (spawnPosition[0, i] != null && !gameOver)
            {
                Vector3 newWorldPosition = lineGrid.CellToWorld(new Vector3Int(0, i + 1));
                spawnPosition[0, i].transform.DOMove(newWorldPosition, 3f, false).SetEase(Ease.OutCirc);

          
                spawnPosition[0, i + 1] = spawnPosition[0, i];
                spawnPosition[0, i] = null;
            }
        }

        if (!gameOver)
        {
            Vector3 initialPosition = lineGrid.CellToWorld(new Vector3Int(0, 0));
            go.transform.position = initialPosition;
            spawnPosition[0, 0] = go;
        }
        
        if (spawnPosition[0, gridHeight - 2] != null && !gameOver)
        {
            GameObject lastBox = spawnPosition[0, gridHeight - 2]; 
            lastBox.transform.DOKill();
            lastBox.transform.position = boxPosBeforeArray.transform.position;
            spawnPosition[0, gridHeight - 2] = null;
            gridManager.SpawnBox(lastBox);
        }
    }   

    void GameOver()
    {
        Debug.Log("THE GAME IS OVER");
        gameOver = true;
    }
}