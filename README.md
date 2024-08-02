[myg-word-game.dg9wp5z6.sw.md](https://github.com/user-attachments/files/16468431/myg-word-game.dg9wp5z6.sw.md)# MYG_Project_3

TRELLO LINK :
https://trello.com/invite/b/cZZcHlFe/ATTI593b4da3ca8f7fc4926574da62fcc1cfAF657B5F/myg-project-3

FIGMA BLOCKOUT :
![GAME LOBBY](https://github.com/WeeSeKk/MYG_Project_3/assets/113300702/60a5c25e-55c6-49d6-a870-1add8ec80e42)  ![GAME LOBBY SELECTED PLAYER ICON AND USERNAME](https://github.com/WeeSeKk/MYG_Project_3/assets/113300702/76ed719b-fd39-43e6-8494-ca472fb1626b)  ![GAME LOBBY SETTINGS](https://github.com/WeeSeKk/MYG_Project_3/assets/113300702/4030e8f3-b644-453d-a0f7-7401dee9a70d)

![MOD SELECTION AFTER PRESSING SINPLEPLAYER OR MULTIPLAYER](https://github.com/WeeSeKk/MYG_Project_3/assets/113300702/bdc335a0-c39e-4c45-b757-ab1e5c75a80d)

![GAME VIEW](https://github.com/WeeSeKk/MYG_Project_3/assets/113300702/9cdd688f-1d30-441d-91e8-bcce7f3e6a6a)  ![GAME PAUSED](https://github.com/WeeSeKk/MYG_Project_3/assets/113300702/761ce28f-13bd-4cbd-a499-a3429a6ce0dc)  ![GAME SETTINGS](https://github.com/WeeSeKk/MYG_Project_3/assets/113300702/4a1989e3-e839-4825-8415-4cba7156983d)


![GAME OVER TIME'S UP](https://github.com/WeeSeKk/MYG_Project_3/assets/113300702/b934a6c4-1af3-4931-95cc-cfbf6c8b8570)  ![GAME OVER TO MANY BLOCKS](https://github.com/WeeSeKk/MYG_Project_3/assets/113300702/3e5b3c64-df90-4751-872f-63e488543140)


CLASS DIAGRAM :
![MYG_Project_3 drawio](https://github.com/WeeSeKk/MYG_Project_3/assets/113300702/6d6aafc4-432b-4cc7-a4cb-a5b9396400ba)


title: MYG Word Game

# Introduction

This document will walk you through the implementation of the MYG Word Game feature.

The feature allows players to create an account, log in, and compete on a leaderboard. The game has two modes: one where blocks fall and form words, and another where players find words in a pre-filled grid.

We will cover:

1. <SwmToken path="/Assets/PlayFabSDK/Client/PlayFabClientModels.cs" pos="3805:1:1" line-data="        PlayFab,">`PlayFab`</SwmToken> login, registration, and leaderboard.
2. Object pooling.
3. Dictionary of letters and box frequencies.
4. Game manager's spawn logic for gamemode 1.
5. Grid manager functionalities.
6. Default and special types of boxes.
7. Gamemode 2 and <SwmToken path="/Assets/PlayFabSDK/Client/PlayFabClientModels.cs" pos="3805:1:1" line-data="        PlayFab,">`PlayFab`</SwmToken> category retrieval.

# <SwmToken path="/Assets/PlayFabSDK/Client/PlayFabClientModels.cs" pos="3805:1:1" line-data="        PlayFab,">`PlayFab`</SwmToken> login, registration, and leaderboard

## Registration :

<SwmSnippet path="/Assets/Script/PlayfabManager.cs" line="72">

---

The registration function creates a new user account using <SwmToken path="/Assets/PlayFabSDK/Client/PlayFabClientModels.cs" pos="3805:1:1" line-data="        PlayFab,">`PlayFab`</SwmToken>. It sends a request with the username and password, and upon success, updates the UI and starts a coroutine to show a success message.

```c#
        public void OnRegister(string username, string password)
        {
            var request = new RegisterPlayFabUserRequest
            {
                Username = username,
                Password = password,
                RequireBothUsernameAndEmail = false
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSucces, OnError);
        }

        void OnRegisterSucces(RegisterPlayFabUserResult result)
        {
            Debug.Log("Register Success" + result);
            playerUsername = result.Username;
            UpdateUsername(playerUsername);
            introUIManager.HideWaitingScreen();
            StartCoroutine(introUIManager.ShowError("Register Completed"));
        }

```

---

</SwmSnippet>

## Login :

<SwmSnippet path="/Assets/Script/PlayfabManager.cs" line="93">

---

The login function authenticates a user with <SwmToken path="/Assets/PlayFabSDK/Client/PlayFabClientModels.cs" pos="3805:1:1" line-data="        PlayFab,">`PlayFab`</SwmToken>. It sends a request with the username and password, and upon success, launches the game lobby.

```c#
        public void OnLogin(string username, string password)
        {
            var request = new LoginWithPlayFabRequest
            {
                Username = username,
                Password = password,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true
                }
            };

            PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSucces, OnError);
        }

        void OnLoginSucces(LoginResult result)
        {
            Debug.Log("Login Success");
            if (playerUsername == null)
            {
                playerUsername = result.InfoResultPayload.PlayerProfile.DisplayName;
            }
            GameManager.instance.LaunchLobby();
        }
```

---

</SwmSnippet>

## Leaderboard :

<SwmSnippet path="/Assets/Script/PlayfabManager.cs" line="118">

---

The leaderboard functions handle updating and retrieving player scores. The <SwmToken path="/Assets/Script/PlayfabManager.cs" pos="118:5:5" line-data="        public void SendLeaderboard(int score)">`SendLeaderboard`</SwmToken> function updates the player's high score, while the <SwmToken path="/Assets/Script/PlayfabManager.cs" pos="137:5:5" line-data="        public void GetLeaderboard()">`GetLeaderboard`</SwmToken> function retrieves the top 10 scores and updates the UI.

```c#
        public void SendLeaderboard(int score)
        {
            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "High Score",
                    Value = score
                }
            }
            };
            PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderbordUpdate, OnError);
        }

        void OnLeaderbordUpdate(UpdatePlayerStatisticsResult result)
        {
            Debug.Log("leaderbord updated");
        }

        public void GetLeaderboard()
        {
            var request = new GetLeaderboardRequest
            {
                StatisticName = "High Score",
                StartPosition = 0,
                MaxResultsCount = 10
            };
            PlayFabClientAPI.GetLeaderboard(request, OnleaderbordGet, OnError);
        }

        void OnleaderbordGet(GetLeaderboardResult result)
        {
            lobbyUIManager = GameObject.Find("UIDocument").GetComponent<LobbyUIManager>();
            foreach (var item in result.Leaderboard)
            {
                lobbyUIManager.AddLeaderboardList(item.DisplayName + " " + item.StatValue);
            }
        }
```

---

</SwmSnippet>

# Object pooling

## Box spawning :

<SwmSnippet path="Assets/Script/ObjectPool.cs" line="14">

---

The <SwmToken path="/Assets/Script/ObjectPool.cs" pos="14:7:7" line-data="    public static GameObject BoxSpawn(GameObject gameObject, Vector3 spawnPos, Quaternion spawnRot)">`BoxSpawn`</SwmToken> function manages the creation and reuse of game objects. It checks for inactive objects in the pool and reactivates them if available; otherwise, it instantiates a new object.

```
    public static GameObject BoxSpawn(GameObject gameObject, Vector3 spawnPos, Quaternion spawnRot)
    {
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookUpString == gameObject.name);

        if(pool == null)
        {
            pool = new PooledObjectInfo() {LookUpString = gameObject.name};
            ObjectPools.Add(pool);
        }

        GameObject spawnAbleObject = null;
        
        foreach (GameObject obj in pool.InactiveObjects)//look for inactive object in the pool 
        {
            if (obj != null)
            {
                spawnAbleObject = obj;
                break;
            }
        }

        if (spawnAbleObject == null)//if there is no inactive object then create one
        {
            spawnAbleObject = Instantiate(gameObject, spawnPos, spawnRot);
        }
        else//if there is an inactive object then reactive it
        {
            spawnAbleObject.transform.position = spawnPos;
            spawnAbleObject.transform.rotation = spawnRot;
            pool.InactiveObjects.Remove(spawnAbleObject);
            spawnAbleObject.SetActive(true);
        }
        return spawnAbleObject;
    }
```

---

</SwmSnippet>

## Returning objects to pool :

<SwmSnippet path="/Assets/Script/ObjectPool.cs" line="54">

---

The <SwmToken path="/Assets/Script/ObjectPool.cs" pos="54:7:7" line-data="    public static void ReturnObjectToPool(GameObject gameObject)">`ReturnObjectToPool`</SwmToken> function deactivates game objects and adds them back to the pool for future reuse.

```c#
    public static void ReturnObjectToPool(GameObject gameObject)
    {
        string goName = gameObject.name.Substring(0, gameObject.name.Length - 7);//removing the "(Clone)" frome the new instantiate gameobject

        PooledObjectInfo pool = ObjectPools.Find(p => p.LookUpString == goName);

        if(pool == null)
        {
            Debug.LogWarning("ERROR " + gameObject);
        }
        else
        {
            gameObject.SetActive(false);
            pool.InactiveObjects.Add(gameObject);
        }
    }
```

---

</SwmSnippet>

# Dictionary of letters and box frequencies

## Letter frequencies :

<SwmSnippet path="/Assets/Script/WordsManager.cs" line="16">

---

The <SwmToken path="/Assets/Script/WordsManager.cs" pos="16:15:15" line-data="        private static readonly Dictionary&lt;char, int&gt; letterFrequencies = new Dictionary&lt;char, int&gt;//List of letters with the frequencies of which they can appear">`letterFrequencies`</SwmToken> dictionary defines the frequency of each letter appearing in the game.

```c#
        private static readonly Dictionary<char, int> letterFrequencies = new Dictionary<char, int>//List of letters with the frequencies of which they can appear
    {
        { 'a', 8 }, { 'b', 2 }, { 'c', 3 }, { 'd', 4 }, { 'e', 13 }, { 'f', 2 }, { 'g', 2 }, { 'h', 6 },
        { 'i', 7 }, { 'j', 1 }, { 'k', 1 }, { 'l', 4 }, { 'm', 2 }, { 'n', 7 }, { 'o', 8 }, { 'p', 2 },
        { 'q', 1 }, { 'r', 6 }, { 's', 6 }, { 't', 9 }, { 'u', 3 }, { 'v', 1 }, { 'w', 2 }, { 'x', 1 },
        { 'y', 2 }, { 'z', 1 }
    };
```

---

</SwmSnippet>

## Initialize box frequencies :

<SwmSnippet path="/Assets/Script/GameManager.cs" line="64">

---

The <SwmToken path="/Assets/Script/GameManager.cs" pos="64:3:3" line-data="        void InitializeBoxFrequencies()">`InitializeBoxFrequencies`</SwmToken> function sets the spawn frequencies for different types of boxes.

```c#
        void InitializeBoxFrequencies()
        {
            if (boxFrequencies == null)
            {
                boxFrequencies.Add(boxsPrefab[0], 100);//default box  100
                boxFrequencies.Add(boxsPrefab[1], 4);//crusher box  4
                boxFrequencies.Add(boxsPrefab[2], 6);//skull box  6
                boxFrequencies.Add(boxsPrefab[3], 3);//fire box  3
                boxFrequencies.Add(boxsPrefab[4], 2);//magnet box  2
                boxFrequencies.Add(boxsPrefab[5], 3);//bomb box   3
            }
        }

```

---

</SwmSnippet>

# Game manager spawn logic for Gamemode 1

## Generating boxes :

<SwmSnippet path="/Assets/Script/GameManager.cs" line="173">

---

The <SwmToken path="/Assets/Script/GameManager.cs" pos="173:5:5" line-data="        public GameObject GenerateBox()">`GenerateBox`</SwmToken> function selects a box to spawn based on predefined frequencies.

```c#
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
```

---

</SwmSnippet>

## Spawning new boxes :

<SwmSnippet path="/Assets/Script/GameManager.cs" line="330">

---

The <SwmToken path="/Assets/Script/GameManager.cs" pos="330:5:5" line-data="        public IEnumerator SpawnNewBoxs()">`SpawnNewBoxs`</SwmToken> coroutine continuously spawns new boxes at a decreasing interval until the grid is full or the timer reach 0.

```c#
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
```

---

</SwmSnippet>

## Moving boxes :

<SwmSnippet path="/Assets/Script/GameManager.cs" line="407">

---

The <SwmToken path="/Assets/Script/GameManager.cs" pos="407:3:3" line-data="        void MoveBoxs(GameObject go)">`MoveBoxs`</SwmToken> function moves boxes up the secondary grid to show a preview of the boxes who will appear in the main grid.

```c#
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
```

---

</SwmSnippet>

# Grid manager functionalities

## Spawning boxes :

<SwmSnippet path="/Assets/Script/GridManager.cs" line="43">

---

The <SwmToken path="/Assets/Script/GridManager.cs" pos="43:5:5" line-data="    public void SpawnBox(GameObject gameObject)">`SpawnBox`</SwmToken> function places a box in an empty cell on the grid. If the grid is full, it triggers a game over event.

```c#
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
```

---

</SwmSnippet>

## Removing selected boxes :

<SwmSnippet path="/Assets/Script/GridManager.cs" line="143">

---

The <SwmToken path="/Assets/Script/GridManager.cs" pos="143:5:5" line-data="    public IEnumerator RemoveSelectedBox() //Destroy all the boxes used to create a word">`RemoveSelectedBox`</SwmToken> coroutine removes boxes used to form a word and returns them to the pool.

```c#
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
```

---

</SwmSnippet>

## Removing boxes :

<SwmSnippet path="/Assets/Script/GridManager.cs" line="186">

---

The <SwmToken path="/Assets/Script/GridManager.cs" pos="186:5:5" line-data="    public void RemoveBoxs(GameObject gameObject)">`RemoveBoxs`</SwmToken> function removes a specific box from the grid and returns it to the pool (only used when the box was remove by specials boxes).

```c#
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
```

---

</SwmSnippet>

# Default and special types of boxes

## Default box :

<SwmSnippet path="/Assets/Script/BoxPrefabController.cs" line="98">

---

The <SwmToken path="/Assets/Script/BoxPrefabController.cs" pos="98:3:3" line-data="    void ChooseSprite()">`ChooseSprite`</SwmToken> function selects a sprite for the default box based on player preferences.

```c#
    void ChooseSprite()
    {
        string sprite = PlayerPrefs.GetString(BOXSSKIN, "default");

        if (sprite == "default")//default sprite
        {
            System.Random rdm = new System.Random();
            int i = rdm.Next(0, 4);

            goSprite.sprite = sprites[i];
            text.color = Color.black;
            visualGo.transform.localScale = new Vector3(1f, 1f);
        }
        else if (sprite == "kenney")//k sprite
        {
            System.Random rdm = new System.Random();
            int i = rdm.Next(0, 15);

            goSprite.sprite = kSprites[i];
            text.color = Color.black;
            visualGo.transform.localScale = new Vector3(1.45f, 1.45f);

            if (i == 1 || i == 5 || i == 9 || i == 13)
            {
                outline.sprite = outlineSprites[1];
                outlineGO.transform.localScale = new Vector3(0.9f, 0.9f);
                outlineGO.transform.localRotation = Quaternion.Euler(0, 0, -45f);
            }
            if (i == 0 || i == 4 || i == 8 || i == 12 || i == 15)
            {
                outline.sprite = outlineSprites[0];
            }
            else
            {
                outline.sprite = outlineSprites[1];
            }
        }
        else if (sprite == "simple")//s sprite
        {
            System.Random rdm = new System.Random();
            int i = rdm.Next(0, 3);

            text.color = Color.white;

            if (i == 0)
            {
                outline.sprite = outlineSprites[1];
                visualGo.transform.localScale = new Vector3(0.45f, 0.45f);
                outlineGO.transform.localScale = new Vector3(1.05f, 1.05f);
                goSprite.sprite = sSprites[0];
            }
            else
            {
                outline.sprite = outlineSprites[0];
                visualGo.transform.localScale = new Vector3(1.2f, 1.2f);
                outlineGO.transform.localScale = new Vector3(1.05f, 1.05f);
                goSprite.sprite = sSprites[1];
            }

        }
    }
```

---

</SwmSnippet>

<SwmSnippet path="/Assets/Script/BoxPrefabController.cs" line="217">

---

The <SwmToken path="/Assets/Script/BoxPrefabController.cs" pos="217:3:3" line-data="    void ChooseLetter()">`ChooseLetter`</SwmToken>  function  is used to generate a letter for the box depending on the Gamemode playing.

```c#
    void ChooseLetter()
    {
        if (currentScene.name == "Scene_Gamemode_01")
        {
            letter = wordsManager.GenerateLetter();

            text.SetText(letter.ToString());
        }
        else
        {
            letter = wordsManager.AssingLetter();

            text.SetText(letter.ToString());
        }
    }
```

---

</SwmSnippet>

<SwmSnippet path="/Assets/Script/BoxPrefabController.cs" line="253">

---

Once in the main grid the function <SwmToken path="/Assets/Script/BoxPrefabController.cs" pos="253:5:5" line-data="    public void FindCell()//find in witch cell is this gameobject">`FindCell`</SwmToken> will look for this box and update his position (this function is shared by all other types of boxes).

```c#
    public void FindCell()//find in witch cell is this gameobject
    {
        for (int x = 0; x < gridManager.maxgridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                if (gridManager.gridArray[x, y] == this.gameObject)
                {
                    posY = y;
                    posX = x;
                    spawned = true;
                    NewMoveCell(x, y);
                    return;
                }
            }
        }
    }
```

---

</SwmSnippet>

<SwmSnippet path="/Assets/Script/BoxPrefabController.cs" line="278">

---

Then the function  <SwmToken path="/Assets/Script/BoxPrefabController.cs" pos="278:3:3" line-data="    void NewMoveCell(int x, int y)">`NewMoveCell`</SwmToken> will move the box down if an empty space is found (this function is shared by most of the other boxes, something with variation).

```c#
    void NewMoveCell(int x, int y)
    {
        Vector3 newWorldPosition;

        if (currentScene.name == "Scene_Gamemode_01")
        {
            mooveSpeed = 3f;
        }
        else
        {
            mooveSpeed = 0.1f;
        }

        for (int i = 0; i < gridManager.gridHeight; i++)
        {
            if ((i != 0 && gridManager.gridArray[x, i] == null && gridManager.gridArray[x, i - 1] != null) || (i == 0 && gridManager.gridArray[x, i] == null))
            {
                newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(x, i));
                this.gameObject.transform.DOMove(newWorldPosition, mooveSpeed, false).SetEase(Ease.OutCirc);
                gridManager.UpdateArray(this.gameObject, x, i);
                posY = i;
                break;
            }
        }
    }
```

---

</SwmSnippet>

## Magnet box :

<SwmSnippet path="/Assets/Script/MagnetBoxPrefab.cs" line="296">

---

Unlike the default box the magnet will be placed in the right side of the grid and active it's BoxCollider to detect when a box enter its range.

```c#
    void NewMoveCell(int x, int y)
    {
        Vector3 newWorldPosition;

        posX = gridManager.gridWidth - 1;
        posY = y - 5;
            
        newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(posX, posY));
        this.gameObject.transform.DOMove(newWorldPosition, 3f, false).SetEase(Ease.OutCirc).OnComplete(() => {

            _boxCollider2D.enabled = true;
        });

        gridManager.UpdateArray(this.gameObject, posX, posY);
        SendPositionToArray(this.gameObject, posX, posY);
    }
```

---

</SwmSnippet>

<SwmSnippet path="/Assets/Script/MagnetBoxPrefab.cs" line="42">

---

One the BoxCollider of the magnet is activated the <SwmToken path="/Assets/Script/MagnetBoxPrefab.cs" pos="42:5:5" line-data="    public void LockBoxPosition(GameObject gameObject)">`LockBoxPosition`</SwmToken> function locks the position of a box when it's in a cell next to the magnet or next to a box connected to the magnet.

```c#
    public void LockBoxPosition(GameObject gameObject)
    {
        if (!lockedGo.Contains(gameObject))
        {
            for (int x = 0; x < gridManager.gridWidth; x++)
            {
                for (int y = 0; y < gridManager.gridHeight; y++)
                {
                    if (gridManager.gridArray[x, y] == gameObject)//found the gameobject that enter this gameobject box collider
                    {
                        for (int i = x + 1; i < gridManager.gridWidth; i++)//look for every x position at the y position of the gameobject starting from the position of the gameobject
                        {
                            if (gridManager.gridArray[i, posY] == null)//if there is a empty space between this gameobject and the magnet return
                            {
                                return;
                            }
                            else if (gridManager.gridArray[i, posY] == this.gameObject)//else lock the gameobject
                            {
                                gameObject.transform.DOKill();//kill the precedent DOTWEEN !!!IMPORTANT!!
                            
                                Vector3 newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(x, posY));//i is the position null on the X and posY is this gameobject y Position
                                gameObject.transform.DOMove(newWorldPosition, 3f, false).SetEase(Ease.OutElastic);
                                SendPositionToArray(gameObject, x, y);
                                gridManager.UpdateArray(gameObject, x, posY);

                                EventManager.UpdatePosition(gameObject, x, posY);

                                if (!lockedGo.Contains(gameObject))
                                {
                                    lockedGo.Add(gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
```

---

</SwmSnippet>

## Crusher box :

<SwmSnippet path="/Assets/Script/BigBoxPrefabController.cs" line="26">

---

The crusher box has a function <SwmToken path="/Assets/Script/BigBoxPrefabController.cs" pos="26:3:3" line-data="    void OnCollisionEnter2D(Collision2D collision)">`OnCollisionEnter2D`</SwmToken> who will detect any collision with other boxes and remove them will shaking all the boxes in the grid and playing VFX.

```c#
    void OnCollisionEnter2D(Collision2D collision)
    {
        gridManager.RemoveBoxs(collision.gameObject);
        EventManager.ShakeBoxs();
        FindCell();
    }
```

---

</SwmSnippet>

<SwmSnippet path="/Assets/Script/BigBoxPrefabController.cs" line="92">

---

With the function <SwmToken path="/Assets/Script/BigBoxPrefabController.cs" pos="92:3:3" line-data="    void NewMoveCell(int x, int y)">`NewMoveCell`</SwmToken> it will continue to move down and remove all boxes in its path.

```c#
    void NewMoveCell(int x, int y)
    {
        Vector3 newWorldPosition;
        this.gameObject.transform.DOKill();
        this.gameObject.transform.localScale = new Vector3(1, 1, 1);

        for (int i = 0; i < 9; i++)
        {
            if (gridManager.gridArray[x, 0] == null)  
            {
                newWorldPosition = new Vector3( this.gameObject.transform.position.x,  this.gameObject.transform.position.y - 20,  this.gameObject.transform.position.z);
                this.gameObject.transform.DOMove(newWorldPosition, 4f, false).SetEase(Ease.OutBounce).OnComplete(() => {

                    this.gameObject.transform.DOKill();
                    gridManager.RemoveBoxs(this.gameObject);
                });
            }
            
            else if((i != 0 && gridManager.gridArray[x, i] == null && gridManager.gridArray[x, i - 1] != null) || (i == 0 && gridManager.gridArray[x, i] == null))
            {
                newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(x, i + 1));
                this.gameObject.transform.DOMove(newWorldPosition, 2f, false).SetEase(Ease.OutBounce).OnComplete(() => {
                    
                    gridManager.UpdateArray(this.gameObject, x, i);
                    posY = i;
                });
            }
        }
    }
```

---

</SwmSnippet>

## Fire box :

<SwmSnippet path="/Assets/Script/FireBoxPrefab.cs" line="76">

---

When it appears in the grid, the fire box is inactive, but once the <SwmToken path="/Assets/Script/FireBoxPrefab.cs" pos="76:3:3" line-data="    void OnMouseDown()">`OnMouseDown`</SwmToken> function activated the box will be active the sprite will disappear and play a VFX.

```c#
    void OnMouseDown()
    {
        if (spawned)
        {
            isClickable = true;
        }
        if (isClickable)
        {
            AudioManager.instance.PlayAudioClip(1);
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            boxCollider2D.isTrigger = true;
            fire = true;
            goSprite.enabled = false;
            fireParticle.Play();
            BurnBoxs();
            StartCoroutine(KillMyself());
        }
    }
```

---

</SwmSnippet>

<SwmSnippet path="/Assets/Script/FireBoxPrefab.cs" line="106">

---

Once activated, the <SwmToken path="/Assets/Script/FireBoxPrefab.cs" pos="106:3:3" line-data="    void OnTriggerEnter2D(Collider2D other)">`OnTriggerEnter2D`</SwmToken> will add to a list any boxes who enter in its Collider  and call the function <SwmToken path="/Assets/Script/FireBoxPrefab.cs" pos="114:1:3" line-data="                BurnBoxs();">`BurnBoxs()`</SwmToken> who will remove all boxes in the list.

```c#
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != null)
        {
            hitBoxs.Add(other.gameObject);

            if (fire)
            {
                BurnBoxs();
            }
        }
    }

    void BurnBoxs()
    {
        foreach (GameObject go in hitBoxs)
        {
            gridManager.RemoveBoxs(go);
        }
        hitBoxs.Clear();
    }
```

---

</SwmSnippet>

## Bomb box :

<SwmSnippet path="/Assets/Script/BombBoxPrefab.cs" line="161">

---

Once in the grid to bomb box will add any Gameobject who enters its range in a list and remove any that leave the range.

```c#
    public void AddGoToList(GameObject gameObject)
    {
        if (!explosionRange.Contains(gameObject))
        {
            explosionRange.Add(gameObject);
        }
        else
        {
            explosionRange.Remove(gameObject);
        }
    }
```

---

</SwmSnippet>

<SwmSnippet path="/Assets/Script/BombBoxPrefab.cs" line="173">

---

Once in the grid the <SwmToken path="/Assets/Script/BombBoxPrefab.cs" pos="173:3:3" line-data="    IEnumerator BombTicking()">`BombTicking`</SwmToken> function will start the bomb ticking animation and sound effects, and at the end of the coroutine if this box was not selected to create a word  the bomb will explode and remove itself and any box in its list from the grid.

```c#
    IEnumerator BombTicking()
    {
        int timeTicking = 0;
        int maxTicking = 10;
        float timeToTick = 1f;
        int lastTicking = 0;

        Vector3 initScale = visualGo.transform.localScale;
        Vector3 scaleBig = new Vector3(1.5f, 1.5f);

        while (timeTicking < maxTicking)
        {
           //scale up animation
            visualGo.transform.DOScale(scaleBig, timeToTick);
            yield return new WaitForSeconds(timeToTick);

           //scale down animation
            visualGo.transform.DOScale(initScale, timeToTick);
            yield return new WaitForSeconds(timeToTick);

            timeToTick -= 0.1f;
            timeTicking ++;
        }

        while (lastTicking < maxTicking)
        {
           //scale up animation
            visualGo.transform.DOScale(scaleBig, 0.1f);
            yield return new WaitForSeconds(0.1f);

           //scale down animation
            visualGo.transform.DOScale(initScale, 0.1f);
            yield return new WaitForSeconds(0.1f);

            lastTicking ++;
        }

        goSprite.enabled = false;
        AudioManager.instance.PlayAudioClip(4);
        _particleSystem.Play();
        text.SetText("");

        List<GameObject> explosionRangeCopy = new List<GameObject>(explosionRange);

        foreach (GameObject gameObject in explosionRangeCopy)
        {
            if (gameObject != null)
            {
                gridManager.RemoveBoxs(gameObject);
                explosionRange.Remove(gameObject);
            }
        }

        yield return new WaitForSeconds(0.5f);

        gridManager.RemoveBoxs(this.gameObject);
    }
```

---

</SwmSnippet>

## Skull box :

<SwmSnippet path="/Assets/Script/DeathBoxPrefab.cs" line="88">

---

The skull box behaves like a default box but is not selectable and can be removed from the grid only by the other special boxes.

```c#
    void NewMoveCell(int x, int y)
    {
        Vector3 newWorldPosition;

        for (int i = 0; i < gridManager.gridHeight; i++)
        {
            if((i != 0 && gridManager.gridArray[x, i] == null && gridManager.gridArray[x, i - 1] != null) || (i == 0 && gridManager.gridArray[x, i] == null))
            {
                newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(x, i));
                this.gameObject.transform.DOMove(newWorldPosition, 3f, false).SetEase(Ease.OutCirc);
                gridManager.UpdateArray(this.gameObject, x, i);
                posY = i;
                break;
            }  
        }
    }
```

---

</SwmSnippet>

# Gamemode 2 and <SwmToken path="/Assets/PlayFabSDK/Client/PlayFabClientModels.cs" pos="3805:1:1" line-data="        PlayFab,">`PlayFab`</SwmToken> category retrieval

## Spawning new boxes for Gamemode 2 :

<SwmSnippet path="/Assets/Script/GameManager.cs" line="378">

---

The <SwmToken path="/Assets/Script/GameManager.cs" pos="378:5:5" line-data="        public IEnumerator SpawnNewBoxsGamemode2()">`SpawnNewBoxsGamemode2`</SwmToken> coroutine spawns boxes in a different pattern for the second game mode.

```c#
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
```

---

</SwmSnippet>

## Retrieving category from <SwmToken path="/Assets/PlayFabSDK/Client/PlayFabClientModels.cs" pos="3805:1:1" line-data="        PlayFab,">`PlayFab`</SwmToken> :

<SwmSnippet path="/Assets/Script/PlayfabManager.cs" line="163">

---

The <SwmToken path="/Assets/Script/PlayfabManager.cs" pos="163:7:7" line-data="        public async Task GetCategoryAsync(string key)">`GetCategoryAsync`</SwmToken> function retrieves a category from <SwmToken path="/Assets/PlayFabSDK/Client/PlayFabClientModels.cs" pos="3805:1:1" line-data="        PlayFab,">`PlayFab`</SwmToken> and updates the UI and word manager accordingly.

```c#
        public async Task GetCategoryAsync(string key)
        {
            var tsk = new TaskCompletionSource<string>();

            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), result =>
            {
                string category = result.Data[key];
                tsk.SetResult(category);

            }, OnError);

            string categoryResult = await tsk.Task;

            wordsManager = GameObject.Find("WordsManager").GetComponent<WordsManager>();
            wordsManager.AddWordsToCategoryList(categoryResult);

            uIManager = GameObject.Find("UIDocument").GetComponent<UIManager>();
            uIManager.SetCategoryLabel(key);

            Debug.Log(categoryResult);
        }

```

---

</SwmSnippet>

## Choosing words :

<SwmSnippet path="Assets/Script/WordsManager.cs" line="260">

---

The <SwmToken path="/Assets/Script/WordsManager.cs" pos="260:5:5" line-data="        public void ChooseWords()">`ChooseWords`</SwmToken> function selects words from the category chosen and add them to the list of words to find, for each letter of a word, replace it with "\_" to give hints on the number of letters in each word to find.

```
        public void ChooseWords()
        {
            while (lettersForChosenWords.Count < 70 && wordsCategoryChoosen.Count < 12)
            {
                System.Random rand = new System.Random();
                int num = rand.Next(0, wordsCategory.Count);

                if (wordsCategory[num] != null)
                {
                    wordsCategoryChoosen.Add(wordsCategory[num]);
                }

                foreach (char letter in wordsCategory[num])
                {
                    lettersForChosenWords.Add(letter);
                }
            }

            foreach (string word in wordsCategoryChoosen)
            {
                Debug.Log(word);

                char[] hintchar = new char[word.Length];

                for (int i = 0; i < word.Length; i++)
                {
                    hintchar[i] = '_';
                }

                string hint = new string(hintchar);

                uIManager.AddToFindLists(hint);
            }
        }
```

---

</SwmSnippet>

## Assigning letters :

<SwmSnippet path="/Assets/Script/WordsManager.cs" line="301">

---

The <SwmToken path="/Assets/Script/WordsManager.cs" pos="301:5:5" line-data="        public char AssingLetter()">`AssingLetter`</SwmToken> function assigns letters to boxes based on the chosen words or generates random letters if none are available.

```c#
        public char AssingLetter()
        {
            if (lettersForChosenWords.Count > 0)
            {
                System.Random rand = new System.Random();
                int num = rand.Next(0, lettersForChosenWords.Count);

                char letter = lettersForChosenWords[num];

                lettersForChosenWords.Remove(lettersForChosenWords[num]);

                return letter;
            }
            else
            {
                return GenerateLetter();
            }
        }
```

---

</SwmSnippet>

## Giving hints :

<SwmSnippet path="/Assets/Script/WordsManager.cs" line="326">

---

The <SwmToken path="/Assets/Script/WordsManager.cs" pos="326:5:5" line-data="        public void GiveHintOnWord(int pos)">`GiveHintOnWord`</SwmToken> function provides hints for words in the list by revealing one letter at a time.

```c#
        public void GiveHintOnWord(int pos)
        {
            char[] charChosen = wordsCategoryChoosen[pos].ToCharArray();
            char[] charToFind = uIManager.wordsToFind[pos].ToCharArray();

            for (int a = 0; a < wordsCategoryChoosen[pos].Length; a++)
            {
                if (charToFind[a] != charChosen[a])
                {
                    charToFind[a] = charChosen[a];
                    break;
                }
            }
            string hint = new string(charToFind);

            uIManager.RemoveFromToFindList(hint, true, pos);
        }
    }
```

---

</SwmSnippet>

