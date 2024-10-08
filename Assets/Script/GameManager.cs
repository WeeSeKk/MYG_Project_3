namespace GameManagerNamespace
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using DG.Tweening;
    using UnityEngine.SceneManagement;
    using Unity.VisualScripting;
    using WordsManagerNamespace;
    using PlayfabManagerNamespace;

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        static readonly Dictionary<GameObject, int> boxFrequencies = new Dictionary<GameObject, int>();
        GridManager gridManager;
        WordsManager wordsManager;
        ObjectPool objectPool;
        UIManager uIManager;
        TimerScript timerScript;
        GameObject boxParent;
        Scene currentScene;
        GameObject boxPosBeforeArray;
        Grid lineGrid;
        public List<GameObject> boxsPrefab;
        public GameObject[,] spawnPosition;
        public float spawnSpeed;
        int gridWidth = 1;
        int gridHeight = 10;
        bool gameOver;
        public int score;
        public int powerUpUseCrusher = 0;
        public int powerUpUseFire = 0;
        public int powerUpUseBomb = 0;

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
            InitializeBoxFrequencies();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown("space"))//test for debug
            {
                Time.timeScale = 5;
            }
        }
        /**
        <summary>
        Initialize the frequencies at which the gameobjects will spawn.
        </summary>
        <param name=""></param>
        <returns></returns>
        **/
        void InitializeBoxFrequencies()
        {
            boxFrequencies.Clear();

            boxFrequencies.Add(boxsPrefab[0], 100);//default box  100
            boxFrequencies.Add(boxsPrefab[1], 2);//crusher box  2
            boxFrequencies.Add(boxsPrefab[2], 6);//skull box  6
            boxFrequencies.Add(boxsPrefab[3], 3);//fire box  3
            boxFrequencies.Add(boxsPrefab[4], 2);//magnet box  2
            boxFrequencies.Add(boxsPrefab[5], 3);//bomb box   3
        }

        public void LaunchGamemode_1()
        {
            StartCoroutine(LoadScene("Scene_Gamemode_01", "none"));
        }

        public void LaunchGamemode_2(string category)
        {
            StartCoroutine(LoadScene("Scene_Gamemode_02", category));
        }

        public void LaunchLobby()
        {
            StartCoroutine(LoadScene("Lobby", "none"));
        }
        /**
        <summary>
        Load necessary scene.
        </summary>
        <param name="scene">Name of the scene to load.</param>
        <param name="category">Choosen category if scene to load is Gamemode 2.</param>
        <returns></returns>
        **/
        public IEnumerator LoadScene(string scene, string category)
        {
            if (scene == "Scene_Gamemode_01")
            {
                StopCoroutine(SpawnNewBoxs());
                StopCoroutine(SpawnNewBoxsGamemode2());
                yield return SceneManager.LoadSceneAsync(scene);
                currentScene = SceneManager.GetActiveScene();
                Time.timeScale = 1;
                gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
                objectPool = GameObject.Find("GridManager").GetComponent<ObjectPool>();
                wordsManager = GameObject.Find("WordsManager").GetComponent<WordsManager>();
                uIManager = GameObject.Find("UIDocument").GetComponent<UIManager>();
                timerScript = GameObject.Find("UIDocument").GetComponent<TimerScript>();
                lineGrid = GameObject.Find("BoxLinePosition").GetComponent<Grid>();
                boxParent = GameObject.Find("BoxParent");
                boxPosBeforeArray = GameObject.Find("BoxPosBeforeArray");

                spawnPosition = new GameObject[gridWidth, gridHeight];

                EventManager.gameOverEvent += GameOver;

                timerScript.SetupTimer(300f);

                ResetGamemode(1);
            }
            else if (scene == "Scene_Gamemode_02")
            {
                StopCoroutine(SpawnNewBoxs());
                StopCoroutine(SpawnNewBoxsGamemode2());
                yield return SceneManager.LoadSceneAsync(scene);
                currentScene = SceneManager.GetActiveScene();
                Time.timeScale = 1;
                gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
                objectPool = GameObject.Find("GridManager").GetComponent<ObjectPool>();
                wordsManager = GameObject.Find("WordsManager").GetComponent<WordsManager>();
                uIManager = GameObject.Find("UIDocument").GetComponent<UIManager>();
                timerScript = GameObject.Find("UIDocument").GetComponent<TimerScript>();
                lineGrid = GameObject.Find("BoxLinePosition").GetComponent<Grid>();
                boxParent = GameObject.Find("BoxParent");
                boxPosBeforeArray = GameObject.Find("BoxPosBeforeArray");

                var categoryTask = PlayfabManager.instance.GetCategoryAsync(category);
                yield return new WaitUntil(() => categoryTask.IsCompleted);

                wordsManager.ChooseWords();

                spawnPosition = new GameObject[gridWidth, gridHeight];

                EventManager.gameOverEvent += GameOver;

                timerScript.SetupTimer(300f);

                ResetGamemode(2);
            }
            else if (scene == "Lobby")
            {
                StopCoroutine(SpawnNewBoxs());
                StopCoroutine(SpawnNewBoxsGamemode2());
                yield return SceneManager.LoadSceneAsync(scene);
                currentScene = SceneManager.GetActiveScene();
                Time.timeScale = 1;
                PlayfabManager.instance.GetLeaderboard();
            }
        }
        /**
        <summary>
        Return gameobjects based on frequencies.
        </summary>
        <param name=""></param>
        <returns>Return a gameobject from the list.</returns>
        **/
        public GameObject GenerateBox()
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
        /**
        <summary>
        Reset Gamemode at launch.
        </summary>
        <param name="scene">Name of the scene to reset.</param>
        <returns></returns>
        **/
        public void ResetGamemode(int scene)
        {
            if (scene == 1)
            {
                StopAllCoroutines();

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

                foreach (Transform child in boxParent.transform)
                {
                    ObjectPool.ReturnObjectToPool(child.gameObject);
                }

                gridManager.selectedBoxs.Clear();
                timerScript.timeLeft = 300;
                powerUpUseCrusher = 0;
                powerUpUseFire = 0;
                powerUpUseBomb = 0;
                wordsManager.correctWordsFound.Clear();
                uIManager.CleanLabel();
                uIManager.ResetUI(1);
                Time.timeScale = 1;
                timerScript.ResetTimers(1);
                gameOver = false;
                gridManager.gameOver = false;

                StartCoroutine(SpawnNewBoxs());
            }
            else if (scene == 2)
            {
                StopAllCoroutines();

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

                foreach (Transform child in boxParent.transform)
                {
                    ObjectPool.ReturnObjectToPool(child.gameObject);
                }

                gridManager.selectedBoxs.Clear();
                timerScript.timeLeft = 300;
                wordsManager.correctWordsFound.Clear();
                uIManager.CleanLabel();
                uIManager.ResetUI(2);
                Time.timeScale = 1;
                timerScript.ResetTimers(2);
                gameOver = false;
                gridManager.gameOver = false;

                StartCoroutine(SpawnNewBoxsGamemode2());
            }
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

            uIManager.UpdateScoreLabel(score);

        }

        public void CountPowerupUse(int type)
        {
            switch (type)
            {
                case 0:
                    powerUpUseCrusher++;

                    break;

                case 1:
                    powerUpUseFire++;

                    break;

                case 2:
                    powerUpUseBomb++;

                    break;
            }
        }
        /**
        <summary>
        Spawn gameobject for gamemode 1.
        </summary>
        <param name=""></param>
        <returns></returns>
        **/
        public IEnumerator SpawnNewBoxs()
        {
            float timeChange = 0;
            int x = 0;
            int y = 0;

            while (!gameOver && currentScene.name == "Scene_Gamemode_01")
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
                    timeChange = Time.time + 1;
                    spawnSpeed -= 0.002f;
                }

                yield return new WaitForSeconds(spawnSpeed);
            }
        }
        /**
        <summary>
        Spawn gameobject for gamemode 2.
        </summary>
        <param name=""></param>
        <returns></returns>
        **/
        public IEnumerator SpawnNewBoxsGamemode2()
        {
            int x = 0;
            int y = 0;
            int count = 77;

            while (wordsManager.lettersForChosenWords.Count > 0 && currentScene.name == "Scene_Gamemode_02" || count > 0 && currentScene.name == "Scene_Gamemode_02")
            {
                GameObject box = boxsPrefab[0];

                GameObject newBox = ObjectPool.BoxSpawn(box, boxPosBeforeArray.transform.position, Quaternion.identity);

                newBox.transform.SetParent(boxParent.transform);
                spawnPosition[x, y] = newBox;

                gridManager.SpawnBox(newBox);

                yield return new WaitForSeconds(0.1f);

                count--;
            }
        }
        /**
        <summary>
        Move the gameobject on the grid then give then to the GridManager.
        </summary>
        <param name="go">Gameobject to moove</param>
        <returns></returns>
        **/
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

        void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }

        void GameOver()
        {
            PlayfabManager.instance.SendLeaderboard(score);
            gameOver = true;
        }
    }
}