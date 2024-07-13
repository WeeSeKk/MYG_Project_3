using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] WordsManager wordsManager;
    [SerializeField] VisualTreeAsset elementList;
    [SerializeField] GridManager gridManager;
    VisualElement root;
    VisualElement gameOverTab;
    VisualElement background;
    ListView wordsList;
    Label lettersLabel;
    Button validButton;
    Button retryButton;
    Button undoButton;
    Button CrusherButton;
    Button FireButton;
    Button DeathButton;
    Button MagnetButton;
    Button BombButton;
    List<string> words = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        gameOverTab = root.Q<VisualElement>("GameOverTab");
        background = root.Q<VisualElement>("Background");
        wordsList = root.Q<ListView>("WordsList");
        lettersLabel = root.Q<Label>("LettersLabel");
        validButton = root.Q<Button>("ValidButton");
        undoButton = root.Q<Button>("UndoButton");

        CrusherButton = root.Q<Button>("CrusherButton");
        FireButton = root.Q<Button>("FireButton");
        DeathButton = root.Q<Button>("DeathButton");
        MagnetButton = root.Q<Button>("MagnetButton");
        BombButton = root.Q<Button>("BombButton");

        validButton.RegisterCallback<ClickEvent>(evt => StartCoroutine(wordsManager.IsWordValid(lettersLabel.text)));
        undoButton.RegisterCallback<ClickEvent>(evt => CleanLabel());
        //retryButton.RegisterCallback<ClickEvent>(evt => Retry());

        //CrusherButton.RegisterCallback<ClickEvent>(evt => gridManager.TempCallPowerUp("crusher"));
        //FireButton.RegisterCallback<ClickEvent>(evt => gridManager.TempCallPowerUp("fire"));
        //DeathButton.RegisterCallback<ClickEvent>(evt => gridManager.TempCallPowerUp("death"));
        //MagnetButton.RegisterCallback<ClickEvent>(evt => gridManager.TempCallPowerUp("magnet"));
        //BombButton.RegisterCallback<ClickEvent>(evt => gridManager.TempCallPowerUp("bomb"));

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