using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] WordsManager wordsManager;
    [SerializeField] VisualTreeAsset elementList;
    [SerializeField] VisualTreeAsset elementListToFind;
    [SerializeField] GridManager gridManager;
    [SerializeField] TimerScript timerScript;
    [SerializeField] Canvas canvas;
    [SerializeField] Sprite emptyStar;
    [SerializeField] Sprite goldenStar;
    VisualElement root;
    Button pauseButton;
    VisualElement gameOverTab;
    VisualElement greenLine;
    Slider musicSlider;
    Slider audioSlider;
    VisualElement background;
    VisualElement topleftStar;
    VisualElement topmiddleStar;
    VisualElement toprightStar;
    VisualElement topbottomStar;
    VisualElement middlebottomStar;
    VisualElement bottombottomStar;
    VisualElement settingsTab;
    ListView wordsList;
    ListView rightListView;
    ListView leftListView;
    ListView goWordsToFindList;
    ListView goWordsFoundList;
    Label lettersLabel;
    Label crusherCount;
    Label fireCount;
    Label bombCount;
    Label pointLabel;
    Label scoreLabel;
    Button validButton;
    Button hintButton;
    Button settingsReturnButton;
    Button swapLettersButton;
    Button retryButton;
    Scene currentScene;
    Button undoButton;
    Button settingsButton;
    Button crusherButton;
    Button FireButton;
    Button BombButton;
    Button quitButton;
    Button listButton01;
    Button listButton02;
    Button listButton03;
    Button listButton04;
    Button listButton05;
    Button listButton06;
    Button listButton07;
    Button listButton08;
    Button listButton09;
    Button listButton10;
    Button listButton11;
    Button listButton12;
    List<string> words = new List<string>();
    public List<string> wordsToFind = new List<string>();
    public List<string> wordsFound = new List<string>();
    int crusher = 0;
    int fire = 0;
    int bomb = 0;
    int hint = 0;
    bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.gameOverEvent += GameOver;

        currentScene = SceneManager.GetActiveScene();

        root = GetComponent<UIDocument>().rootVisualElement;
        gameOverTab = root.Q<VisualElement>("GameOverTab");
        background = root.Q<VisualElement>("Background");
        greenLine = root.Q<VisualElement>("GreenLine");
        settingsTab = root.Q<VisualElement>("SettingsTab");
        wordsList = root.Q<ListView>("WordsList");
        rightListView = root.Q<ListView>("RightListView");
        leftListView = root.Q<ListView>("LeftListView");
        goWordsToFindList = root.Q<ListView>("GOWordsToFindList");
        goWordsFoundList = root.Q<ListView>("GOWordsFoundList");
        lettersLabel = root.Q<Label>("LettersLabel");
        validButton = root.Q<Button>("ValidButton");
        settingsButton = root.Q<Button>("SettingsButton");
        settingsReturnButton = root.Q<Button>("SettingsReturnButton");
        undoButton = root.Q<Button>("UndoButton");
        pointLabel = root.Q<Label>("PointLabel");
        retryButton = root.Q<Button>("RetryButton");
        crusherCount = root.Q<Label>("CrusherCount");
        fireCount = root.Q<Label>("FireCount");
        bombCount = root.Q<Label>("BombCount");
        scoreLabel = root.Q<Label>("ScoreLabel");
        musicSlider = root.Q<Slider>("MusicSlider");
        audioSlider = root.Q<Slider>("AudioSlider");
        quitButton = root.Q<Button>("QuitButton");
        pauseButton = root.Q<Button>("PauseButton");

        listButton01 = root.Q<Button>("ListButton01");
        listButton02 = root.Q<Button>("ListButton02");
        listButton03 = root.Q<Button>("ListButton03");
        listButton04 = root.Q<Button>("ListButton04");
        listButton05 = root.Q<Button>("ListButton05");
        listButton06 = root.Q<Button>("ListButton06");
        listButton07 = root.Q<Button>("ListButton07");
        listButton08 = root.Q<Button>("ListButton08");
        listButton09 = root.Q<Button>("ListButton09");
        listButton10 = root.Q<Button>("ListButton10");
        listButton11 = root.Q<Button>("ListButton11");
        listButton12 = root.Q<Button>("ListButton12");
        hintButton = root.Q<Button>("HintButton");

        crusherButton = root.Q<Button>("CrusherButton");
        FireButton = root.Q<Button>("FireButton");
        BombButton = root.Q<Button>("BombButton");
        swapLettersButton = root.Q<Button>("SwapLetters");

        topleftStar = root.Q<VisualElement>("TopleftStar");
        topmiddleStar = root.Q<VisualElement>("TopmiddleStar");
        toprightStar = root.Q<VisualElement>("ToprightStar");
        topbottomStar = root.Q<VisualElement>("TopbottomStar");
        middlebottomStar = root.Q<VisualElement>("MiddlebottomStar");
        bottombottomStar = root.Q<VisualElement>("BottombottomStar");

        HideStars();

        if (currentScene.name == "Scene_Gamemode_01")
        {
            swapLettersButton.RegisterCallback<ClickEvent>(evt => {
                swapLettersButton.pickingMode = PickingMode.Ignore;
                swapLettersButton.style.unityBackgroundImageTintColor = Color.grey;
                timerScript.SetupPowerupTimer(3);
                EventManager.SwapLetters();
                EventManager.ButtonClicked(0);
            });
            crusherButton.RegisterCallback<ClickEvent>(evt => {
                crusherButton.pickingMode = PickingMode.Ignore;
                crusherButton.style.unityBackgroundImageTintColor = Color.grey;
                gridManager.SpawnPowerUp(0);
                timerScript.SetupPowerupTimer(0);
                UpdatePowerupUsed(crusherButton);
                EventManager.ButtonClicked(0);
            });
            FireButton.RegisterCallback<ClickEvent>(evt => {
                FireButton.pickingMode = PickingMode.Ignore;
                FireButton.style.unityBackgroundImageTintColor = Color.grey;
                gridManager.SpawnPowerUp(1);
                timerScript.SetupPowerupTimer(1);
                UpdatePowerupUsed(FireButton);
                EventManager.ButtonClicked(0);
            });
            BombButton.RegisterCallback<ClickEvent>(evt => {
                BombButton.pickingMode = PickingMode.Ignore;
                BombButton.style.unityBackgroundImageTintColor = Color.grey;
                gridManager.SpawnPowerUp(2);
                timerScript.SetupPowerupTimer(2);
                UpdatePowerupUsed(BombButton);
                EventManager.ButtonClicked(0);
            });
            validButton.RegisterCallback<ClickEvent>(evt => {
                StartCoroutine(wordsManager.IsWordValid(lettersLabel.text));
                EventManager.ButtonClicked(0);
            });
        }
        else 
        {
            HideHintsButton();

            validButton.RegisterCallback<ClickEvent>(evt => {
                wordsManager.IsWordValidCategory(lettersLabel.text);
                EventManager.ButtonClicked(0);
            });
            hintButton.RegisterCallback<ClickEvent>(evt => {
                hintButton.pickingMode = PickingMode.Ignore;
                hintButton.style.unityBackgroundImageTintColor = Color.grey;
                timerScript.SetupPowerupTimer(4);
                ShowHintButtons();
                EventManager.ButtonClicked(0);
            });
            listButton01.RegisterCallback<ClickEvent>(evt => {
                wordsManager.GiveHintOnWord(0);
                HideHintsButton();
                EventManager.ButtonClicked(0);
            });
            listButton02.RegisterCallback<ClickEvent>(evt => {
                wordsManager.GiveHintOnWord(1);
                HideHintsButton();
                EventManager.ButtonClicked(0);
            });
            listButton03.RegisterCallback<ClickEvent>(evt => {
                wordsManager.GiveHintOnWord(2);
                HideHintsButton();
                EventManager.ButtonClicked(0);
            });
            listButton04.RegisterCallback<ClickEvent>(evt => {
                wordsManager.GiveHintOnWord(3);
                HideHintsButton();
                EventManager.ButtonClicked(0);
            });
            listButton05.RegisterCallback<ClickEvent>(evt => {
                wordsManager.GiveHintOnWord(4);
                HideHintsButton();
                EventManager.ButtonClicked(0);
            });
            listButton06.RegisterCallback<ClickEvent>(evt => {
                wordsManager.GiveHintOnWord(5);
                HideHintsButton();
                EventManager.ButtonClicked(0);
            });
            listButton07.RegisterCallback<ClickEvent>(evt => {
                wordsManager.GiveHintOnWord(6);
                HideHintsButton();
                EventManager.ButtonClicked(0);
            });
            listButton08.RegisterCallback<ClickEvent>(evt => {
                wordsManager.GiveHintOnWord(7);
                HideHintsButton();
                EventManager.ButtonClicked(0);
            });
            listButton09.RegisterCallback<ClickEvent>(evt => {
                wordsManager.GiveHintOnWord(8);
                HideHintsButton();
                EventManager.ButtonClicked(0);
            });
            listButton10.RegisterCallback<ClickEvent>(evt => {
                wordsManager.GiveHintOnWord(9);
                HideHintsButton();
                EventManager.ButtonClicked(0);
            });
            listButton11.RegisterCallback<ClickEvent>(evt => {
                wordsManager.GiveHintOnWord(10);
                HideHintsButton();
                EventManager.ButtonClicked(0);
            });
            listButton12.RegisterCallback<ClickEvent>(evt => {
                wordsManager.GiveHintOnWord(11);
                HideHintsButton();
                EventManager.ButtonClicked(0);
            });
        }
        undoButton.RegisterCallback<ClickEvent>(evt => {
            CleanLabel();
            EventManager.ButtonClicked(0);
        });
        quitButton.RegisterCallback<ClickEvent>(evt => {
            GameManager.instance.LaunchLobby();
            EventManager.ButtonClicked(0);
        });
        settingsReturnButton.RegisterCallback<ClickEvent>(evt => {
            ShowSettings(settingsReturnButton);
            EventManager.ButtonClicked(0);
        });
        settingsButton.RegisterCallback<ClickEvent>(evt => {
            ShowSettings(settingsButton);
            EventManager.ButtonClicked(0);
        });
        retryButton.RegisterCallback<ClickEvent>(evt => {
            if (currentScene.name == "Scene_Gamemode_01")
            {
                GameManager.instance.ResetGamemode(1);
            }
            else
            {
                GameManager.instance.ResetGamemode(2);
            }
            EventManager.ButtonClicked(0);
        });

        SetSlidersValue();
        
        audioSlider.RegisterValueChangedCallback(evt => {
            EventManager.SFXVolumeChange(audioSlider.value);
            EventManager.ButtonClicked(0);
        });
        musicSlider.RegisterValueChangedCallback(evt => {
            EventManager.MusicVolumeChange(musicSlider.value);
        });
    }

    void SetSlidersValue()
    {
        musicSlider.value = AudioManager.instance.MusicSliderValue();
        audioSlider.value = AudioManager.instance.SFXSliderValue();
    }
    
    public void UpdateLabel(string word)
    {
        lettersLabel.text = word;
    }

    void ShowSettings(Button button)
    {
        if (button == settingsButton)
        {
           settingsTab.RemoveFromClassList("SettingsTabHidden");
           settingsTab.pickingMode = PickingMode.Position;
           pauseButton.pickingMode = PickingMode.Ignore;
        }
        else
        {
            settingsTab.AddToClassList("SettingsTabHidden");
            settingsTab.pickingMode = PickingMode.Ignore;
            pauseButton.pickingMode = PickingMode.Position;
        }
    }

    void ShowHintButtons()
    {
        int wordNumber = wordsToFind.Count;

        listButton01.style.opacity = 100;
        listButton01.pickingMode = PickingMode.Position;
        listButton02.style.opacity = 100;
        listButton02.pickingMode = PickingMode.Position;
        listButton03.style.opacity = 100;
        listButton03.pickingMode = PickingMode.Position;
        listButton04.style.opacity = 100;
        listButton04.pickingMode = PickingMode.Position;
        listButton05.style.opacity = 100;
        listButton05.pickingMode = PickingMode.Position;
        listButton06.style.opacity = 100;
        listButton06.pickingMode = PickingMode.Position;
        listButton07.style.opacity = 100;
        listButton07.pickingMode = PickingMode.Position;
        listButton08.style.opacity = 100;
        listButton08.pickingMode = PickingMode.Position;

        switch (wordNumber)
        {
            case 9:
            listButton09.style.opacity = 100;
            listButton09.pickingMode = PickingMode.Position;
            break;

            case 10:
            listButton09.style.opacity = 100;
            listButton09.pickingMode = PickingMode.Position;
            listButton10.style.opacity = 100;
            listButton10.pickingMode = PickingMode.Position;
            break;

            case 11:
            listButton09.style.opacity = 100;
            listButton09.pickingMode = PickingMode.Position;
            listButton10.style.opacity = 100;
            listButton10.pickingMode = PickingMode.Position;
            listButton11.style.opacity = 100;
            listButton11.pickingMode = PickingMode.Position;
            break;

            case 12:
            listButton09.style.opacity = 100;
            listButton09.pickingMode = PickingMode.Position;
            listButton10.style.opacity = 100;
            listButton10.pickingMode = PickingMode.Position;
            listButton11.style.opacity = 100;
            listButton11.pickingMode = PickingMode.Position;
            listButton12.style.opacity = 100;
            listButton12.pickingMode = PickingMode.Position;
            break;
        }
    }

    void HideHintsButton()
    {
        listButton01.style.opacity = 0;
        listButton01.pickingMode = PickingMode.Ignore;
        listButton02.style.opacity = 0;
        listButton02.pickingMode = PickingMode.Ignore;
        listButton03.style.opacity = 0;
        listButton03.pickingMode = PickingMode.Ignore;
        listButton04.style.opacity = 0;
        listButton04.pickingMode = PickingMode.Ignore;
        listButton05.style.opacity = 0;
        listButton05.pickingMode = PickingMode.Ignore;
        listButton06.style.opacity = 0;
        listButton06.pickingMode = PickingMode.Ignore;
        listButton07.style.opacity = 0;
        listButton07.pickingMode = PickingMode.Ignore;
        listButton08.style.opacity = 0;
        listButton08.pickingMode = PickingMode.Ignore;
        listButton09.style.opacity = 0;
        listButton09.pickingMode = PickingMode.Ignore;
        listButton10.style.opacity = 0;
        listButton10.pickingMode = PickingMode.Ignore;
        listButton11.style.opacity = 0;
        listButton11.pickingMode = PickingMode.Ignore;
        listButton12.style.opacity = 0;
        listButton12.pickingMode = PickingMode.Ignore;
    }

    public void CleanLabel()
    {
        if (lettersLabel != null)
        {
            lettersLabel.text = null;
        }
        wordsManager.ResetWord();
    }

    public void AddList(string word)//add the found word to the list 
    {
        if (!words.Contains(word))
        {
            words.Add(word);
        }

        if (wordsList != null)
        {
            UpdateList();
        }
    }

    public void UpdateScoreLabel(int score)
    {
        scoreLabel.text = score.ToString();
    }

    public IEnumerator ShowPointUp(int point)
    {
        pointLabel.text = "+" + " " + point;
        pointLabel.AddToClassList("PointLabelEnd");
        yield return new WaitForSeconds(1f);
        pointLabel.text = "";
        pointLabel.RemoveFromClassList("PointLabelEnd");
    }

    void UpdateList()//update the list to add new words 
    {
        wordsList.Clear();
        wordsList.itemsSource = words;
        wordsList.makeItem = () => elementList.CloneTree();
        wordsList.bindItem = (element, index) =>
        {
            var label = element.Q<Label>();
            label.text = $"{words[index]}";
        };
        wordsList.fixedItemHeight = 60;
        wordsList.Rebuild();
        lettersLabel.text = null;
    }

    public void UpdatePowerupUsed(Button button)
    {
        if (button == crusherButton)
        {
            crusher ++;
            crusherCount.text = "x " + crusher;
        }
        else if (button == FireButton)
        {
            fire ++;
            fireCount.text = "x " + fire;
        }
        else
        {
            bomb ++;
            bombCount.text = "x " + bomb;
        }
    }
    void HideStars()
    {
        topleftStar.style.backgroundImage = new StyleBackground(emptyStar);
        topbottomStar.style.backgroundImage = new StyleBackground(emptyStar);
        topmiddleStar.style.backgroundImage = new StyleBackground(emptyStar);
        middlebottomStar.style.backgroundImage = new StyleBackground(emptyStar);
        topleftStar.style.backgroundImage = new StyleBackground(emptyStar);
        bottombottomStar.style.backgroundImage = new StyleBackground(emptyStar);
    }

    void ShowStars()
    {
        if (currentScene.name == "Scene_Gamemode_01")
        {
            if (timerScript.timePlaying > 300f)
            {
                topleftStar.style.backgroundImage = new StyleBackground(goldenStar);
                topbottomStar.style.backgroundImage = new StyleBackground(goldenStar);
            }
            if (crusher + fire + bomb < 5)
            {
                topmiddleStar.style.backgroundImage = new StyleBackground(goldenStar);
                middlebottomStar.style.backgroundImage = new StyleBackground(goldenStar);
            }
            if (wordsFound.Count > 20)
            {
                toprightStar.style.backgroundImage = new StyleBackground(goldenStar);
                bottombottomStar.style.backgroundImage = new StyleBackground(goldenStar);
            }
        }
        else
        {
            if (timerScript.timeLeft > 0)
            {
                topleftStar.style.backgroundImage = new StyleBackground(goldenStar);
                topbottomStar.style.backgroundImage = new StyleBackground(goldenStar);
            }
            if (hint < 5)
            {
                topmiddleStar.style.backgroundImage = new StyleBackground(goldenStar);
                middlebottomStar.style.backgroundImage = new StyleBackground(goldenStar);
            }
            if (wordsManager.wordsCategoryChoosen.Count < wordsFound.Count)
            {
                toprightStar.style.backgroundImage = new StyleBackground(goldenStar);
                bottombottomStar.style.backgroundImage = new StyleBackground(goldenStar);
            }
        }
    }

    public IEnumerator ShowGreenLine()
    {
        greenLine.RemoveFromClassList("LineRed");
        greenLine.AddToClassList("LineGreen");
        greenLine.AddToClassList("GreenLineVisible");
        yield return new WaitForSeconds(0.3f);
        greenLine.AddToClassList("GreenLineHiddenRight");
        yield return new WaitForSeconds(0.3f);

        greenLine.RemoveFromClassList("GreenLineVisible");
        greenLine.RemoveFromClassList("GreenLineHiddenRight");
    }

    public IEnumerator ShowRedLine()
    {
        greenLine.RemoveFromClassList("LineGreen");
        greenLine.AddToClassList("LineRed");
        greenLine.AddToClassList("GreenLineVisible");
        yield return new WaitForSeconds(0.3f);
        greenLine.AddToClassList("GreenLineHiddenRight");
        yield return new WaitForSeconds(0.3f);

        greenLine.RemoveFromClassList("GreenLineVisible");
        greenLine.RemoveFromClassList("GreenLineHiddenRight");
    }

    public void AddToFindLists(string word)//add the found word to the list 
    {
        wordsToFind.Add(word);
        UpdateToFindLists();
    }

    public void RemoveFromToFindList(string word, bool replace, int pos)
    {
        if (!replace)
        {
            wordsToFind.Remove(wordsToFind[pos]);
        }
        else 
        {
            wordsToFind[pos] = word;
        }

        UpdateToFindLists();
    }

    void UpdateToFindLists()//update the list to add new words 
    {
        leftListView.Clear();
        leftListView.itemsSource = wordsToFind;
        leftListView.makeItem = () => elementListToFind.CloneTree();
        leftListView.bindItem = (element, index) =>
        {
            var label = element.Q<Label>();
            label.text = $"{wordsToFind[index]}";
        };
        leftListView.fixedItemHeight = 60;
        leftListView.Rebuild();
        lettersLabel.text = null;
    }

    public void AddToFoundLists(string word)//add the found word to the list 
    {
        wordsFound.Add(word);
        UpdateFoundLists();
    }

    void UpdateFoundLists()//update the list to add new words 
    {
        rightListView.Clear();
        rightListView.itemsSource = wordsFound;
        rightListView.makeItem = () => elementList.CloneTree();
        rightListView.bindItem = (element, index) =>
        {
            var label = element.Q<Label>();
            label.text = $"{wordsFound[index]}";
        };
        rightListView.fixedItemHeight = 60;
        rightListView.Rebuild();
        lettersLabel.text = null;
    }

    public void AddToGameOverLists(string wordFound, string wordsToFind)//add the found word to the list 
    {
        UpdateGameOverToFindList();
        UpdateGameOverFoundList();
    }

    void UpdateGameOverToFindList()//update the list to add new words 
    {
        goWordsToFindList.Clear();
        goWordsToFindList.itemsSource = wordsManager.wordsCategoryChoosen;
        goWordsToFindList.makeItem = () => elementList.CloneTree();
        goWordsToFindList.bindItem = (element, index) =>
        {
            var label = element.Q<Label>();
            label.text = $"{wordsManager.wordsCategoryChoosen[index]}";
        };
        goWordsToFindList.fixedItemHeight = 60;
        goWordsToFindList.Rebuild();
        lettersLabel.text = null;

    }

    void UpdateGameOverFoundList()
    {
        goWordsFoundList.Clear();
        goWordsFoundList.itemsSource = wordsFound;
        goWordsFoundList.makeItem = () => elementList.CloneTree();
        goWordsFoundList.bindItem = (element, index) =>
        {
            var label = element.Q<Label>();
            label.text = $"{wordsFound[index]}";
        };
        goWordsFoundList.fixedItemHeight = 60;
        goWordsFoundList.Rebuild();
        lettersLabel.text = null;
    }

    public void GameOver()
    {
        if (currentScene.name == "Scene_Gamemode_02")
        {
            UpdateGameOverToFindList();
            UpdateGameOverFoundList();
        }
        ShowStars();
        gameOver = true;
        canvas.enabled = false;
        gameOverTab.RemoveFromClassList("GameOverTabHidden");
        gameOverTab.pickingMode = PickingMode.Position;
    }

    public void ResetUI(int scene)
    {
        canvas.enabled = true;
        if (gameOver && scene == 1)
        {
            gameOverTab.AddToClassList("GameOverTabHidden");
            gameOverTab.pickingMode = PickingMode.Ignore;
            scoreLabel.text = "";
            crusher = 0;
            fire = 0;
            bomb = 0;
            crusherCount.text = "x 0";
            fireCount.text = "x 0";
            bombCount.text = "x 0";
            words.Clear(); 
            UpdateList();
            gameOver = false;
        }
        if (gameOver && scene == 2)
        {
            gameOverTab.AddToClassList("GameOverTabHidden");
            gameOverTab.pickingMode = PickingMode.Ignore;
            scoreLabel.text = "";
            wordsFound.Clear();
            wordsToFind.Clear();
            gameOver = false;
        }
    }
}