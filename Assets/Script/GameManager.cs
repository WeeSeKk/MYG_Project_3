using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using JetBrains.Annotations;
using Unity.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    static readonly Dictionary<GameObject, int> boxFrequencies = new Dictionary<GameObject, int>();
    GridManager gridManager;
    WordsManager wordsManager;
    UIManager uIManager;
    TimerScript timerScript;
    GameObject boxParent;
    GameObject boxPosBeforeArray;
    Grid lineGrid;
    public List<GameObject> boxsPrefab;
    public GameObject[,] spawnPosition;
    public float spawnSpeed;
    int currentGamemode;
    int gridWidth = 1;
    int gridHeight = 10;
    bool gameOver;
    int score;
    int powerUpUseCrusher = 0;
    int powerUpUseFire = 0;
    int powerUpUseBomb = 0;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))//test for debug
        {
            //Time.timeScale = 5;
            StartCoroutine(LoadScene("Scene_Gamemode_01"));
        }
    }

    void InitializeBoxFrequencies()
    {
        boxFrequencies.Add(boxsPrefab[0], 100);//default box
        boxFrequencies.Add(boxsPrefab[1], 0);//crusher box
        boxFrequencies.Add(boxsPrefab[2], 0);//skull box
        boxFrequencies.Add(boxsPrefab[3], 0);//fire box
        boxFrequencies.Add(boxsPrefab[4], 0);//magnet box
        boxFrequencies.Add(boxsPrefab[5], 0);//bomb box 
    }

    public void LaunchGamemode_1()
    {
        StartCoroutine(LoadScene("Scene_Gamemode_01"));
    }

    public IEnumerator LoadScene(string scene)
    {
        
        if (scene == "Scene_Gamemode_01")
        {
            yield return SceneManager.LoadSceneAsync(scene);
            gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
            wordsManager = GameObject.Find("WordsManager").GetComponent<WordsManager>();
            uIManager = GameObject.Find("UIDocument").GetComponent<UIManager>();
            timerScript = GameObject.Find("UIDocument").GetComponent<TimerScript>();
            boxParent = GameObject.Find("BoxParent");
            boxPosBeforeArray = GameObject.Find("BoxPosBeforeArray");
            lineGrid = GameObject.Find("BoxLinePosition").GetComponent<Grid>();

            spawnPosition = new GameObject[gridWidth, gridHeight];

            EventManager.gameOverEvent += GameOver;
            EventManager.resetEvent += ResetCurrentGamemode;

            currentGamemode = 1;

            InitializeBoxFrequencies();

            StartCoroutine(SpawnNewBoxs());
        }
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

    public void ResetAll()
    {
        foreach (Transform child in boxParent.transform)
        {
            ObjectPool.ReturnObjectToPool(child.gameObject); 
        }

        for (int x = 0; x < gridManager.gridWidth; x++) // For every x position
        {
            for (int y = 0; y < gridManager.gridHeight; y++) // For every y position
            {
                if (gridManager.gridArray[x, y] != null) // Once we found the box in the array
                {
                    gridManager.gridArray[x, y] = null;
                }
            }
        }

        for (int x = 0; x < gridWidth; x++) // For every x position
        {
            for (int y = 0; y < gridHeight; y++) // For every y position
            {
                if (spawnPosition[x, y] != null) // Once we found the box in the array
                {
                    spawnPosition[x, y] = null;
                }
            }
        }

        gridManager.selectedBoxs.Clear();
        timerScript.timeLeft = 300;
        powerUpUseCrusher = 0;
        powerUpUseFire = 0;
        powerUpUseBomb = 0;
        wordsManager.correctWordsFound.Clear();
        uIManager.CleanLabel();
        uIManager.Reset();
        Time.timeScale = 1;
        timerScript.Reset();
        gameOver = false;
        gridManager.gameOver = false;

        StartCoroutine(SpawnNewBoxs());
    }

    public void CountScore(int value)
    {
        int baseLetterPoint = 15;

        baseLetterPoint -= value;

        score += baseLetterPoint;

        if (value < 10)
        {
            StartCoroutine(timerScript.AddTimerTime(1));
        }
        else 
        {
            StartCoroutine(timerScript.AddTimerTime(5));
        }

        Debug.Log(score);
        
    }

    public void CountPowerupUse(int type)
    {
        switch (type)
        {
            case 0:
                powerUpUseCrusher ++;

            break;

            case 1:
                powerUpUseFire ++;

            break;

            case 2:
                powerUpUseBomb ++;

            break;
        }
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
        float timeChange = 0;
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

            if (timeChange < Time.time && spawnSpeed > 0.4f)//speed up spawn speed with game time
            {
                timeChange = Time.time +1;
                spawnSpeed -=0.002f;
            }

            yield return new WaitForSeconds(spawnSpeed);
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