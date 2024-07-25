using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] WordsManager wordsManager;
    [SerializeField] VisualTreeAsset elementList;
    [SerializeField] GridManager gridManager;
    [SerializeField] TimerScript timerScript;
    [SerializeField] Canvas canvas;
    VisualElement root;
    Button pauseButton;
    VisualElement gameOverTab;
    VisualElement greenLine;
    public Slider musicSlider;
    public Slider audioSlider;
    VisualElement background;
    VisualElement topleftStar;
    VisualElement topmiddleStar;
    VisualElement toprightStar;
    VisualElement topbottomStar;
    VisualElement middlebottomStar;
    VisualElement bottombottomStar;
    VisualElement settingsTab;
    ListView wordsList;
    Label lettersLabel;
    Label crusherCount;
    Label fireCount;
    Label bombCount;
    Label pointLabel;
    Label scoreLabel;
    Button validButton;
    Button settingsReturnButton;
    Button swapLettersButton;
    Button retryButton;
    Button undoButton;
    Button settingsButton;
    Button crusherButton;
    Button FireButton;
    Button BombButton;
    Button quitButton;
    List<string> words = new List<string>();
    int crusher = 0;
    int fire = 0;
    int bomb = 0;
    bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.gameOverEvent += GameOver;

        root = GetComponent<UIDocument>().rootVisualElement;
        gameOverTab = root.Q<VisualElement>("GameOverTab");
        background = root.Q<VisualElement>("Background");
        greenLine = root.Q<VisualElement>("GreenLine");
        settingsTab = root.Q<VisualElement>("SettingsTab");
        wordsList = root.Q<ListView>("WordsList");
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
        validButton.RegisterCallback<ClickEvent>(evt => {
            StartCoroutine(wordsManager.IsWordValid(lettersLabel.text));
            EventManager.ButtonClicked(0);
        });
        undoButton.RegisterCallback<ClickEvent>(evt => {
            CleanLabel();
            EventManager.ButtonClicked(0);
        });
        swapLettersButton.RegisterCallback<ClickEvent>(evt => {
            swapLettersButton.pickingMode = PickingMode.Ignore;
            swapLettersButton.style.unityBackgroundImageTintColor = Color.grey;
            EventManager.SwapLetters();
            EventManager.ButtonClicked(0);
        });

        retryButton.RegisterCallback<ClickEvent>(evt => {
            GameManager.instance.ResetAll();
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
        audioSlider.RegisterCallback<ClickEvent>(evt => {
            EventManager.ButtonClicked(0);
        });

        musicSlider.value = AudioManager.instance.musicValue;
        audioSlider.value = AudioManager.instance.soundValue;
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

        UpdateList();
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

    void ShowStars()
    {

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

    public void GameOver()
    {
        gameOver = true;
        canvas.enabled = false;
        gameOverTab.RemoveFromClassList("GameOverTabHidden");
        gameOverTab.pickingMode = PickingMode.Position;
    }

    public void Reset()
    {
        canvas.enabled = true;
        if (gameOver)
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
    }
}