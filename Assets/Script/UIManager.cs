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
    VisualElement root;
    VisualElement gameOverTab;
    VisualElement greenLine;
    VisualElement background;
    ListView wordsList;
    Label lettersLabel;
    Label pointLabel;
    Button validButton;
    Button swapLettersButton;
    Button retryButton;
    Button undoButton;
    Button crusherButton;
    Button FireButton;
    Button BombButton;
    List<string> words = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        gameOverTab = root.Q<VisualElement>("GameOverTab");
        background = root.Q<VisualElement>("Background");
        greenLine = root.Q<VisualElement>("GreenLine");
        wordsList = root.Q<ListView>("WordsList");
        lettersLabel = root.Q<Label>("LettersLabel");
        validButton = root.Q<Button>("ValidButton");
        undoButton = root.Q<Button>("UndoButton");
        pointLabel = root.Q<Label>("PointLabel");

        crusherButton = root.Q<Button>("CrusherButton");
        FireButton = root.Q<Button>("FireButton");
        BombButton = root.Q<Button>("BombButton");
        swapLettersButton = root.Q<Button>("SwapLetters");

        validButton.RegisterCallback<ClickEvent>(evt => StartCoroutine(wordsManager.IsWordValid(lettersLabel.text)));
        undoButton.RegisterCallback<ClickEvent>(evt => CleanLabel());
        swapLettersButton.RegisterCallback<ClickEvent>(evt => EventManager.SwapLetters());
        //retryButton.RegisterCallback<ClickEvent>(evt => Retry());

        crusherButton.RegisterCallback<ClickEvent>(evt => {
            crusherButton.pickingMode = PickingMode.Ignore;
            crusherButton.style.unityBackgroundImageTintColor = Color.grey;
            gridManager.SpawnPowerUp(0);
            timerScript.SetupPowerupTimer(0);
        });
        FireButton.RegisterCallback<ClickEvent>(evt => {
            FireButton.pickingMode = PickingMode.Ignore;
            FireButton.style.unityBackgroundImageTintColor = Color.grey;
            gridManager.SpawnPowerUp(1);
            timerScript.SetupPowerupTimer(1);
        });
        BombButton.RegisterCallback<ClickEvent>(evt => {
            BombButton.pickingMode = PickingMode.Ignore;
            BombButton.style.unityBackgroundImageTintColor = Color.grey;
            gridManager.SpawnPowerUp(2);
            timerScript.SetupPowerupTimer(2);
        });

        EventManager.gameOverEvent += GameOver;
    }
    
    public void UpdateLabel(string word)
    {
        lettersLabel.text = word;
    }

    public void CleanLabel()
    {
        lettersLabel.text = null;
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
        //update the label at the gameover screen
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

    public IEnumerator ShowGreenLine()
    {
        greenLine.AddToClassList("GreenLineVisible");
        yield return new WaitForSeconds(0.3f);
        greenLine.AddToClassList("GreenLineHiddenRight");
        yield return new WaitForSeconds(0.3f);

        greenLine.RemoveFromClassList("GreenLineVisible");
        greenLine.RemoveFromClassList("GreenLineHiddenRight");
    }

    public void GameOver()
    {
        gameOverTab.RemoveFromClassList("GameOverTabHidden");
    }

    void Retry()
    {
        EventManager.ResetEvent(true);
        gameOverTab.AddToClassList("GameOverTabHidden");
    }
}