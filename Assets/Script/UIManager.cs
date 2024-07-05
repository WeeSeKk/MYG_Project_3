using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] WordsManager wordsManager;
    VisualElement root;
    Label lettersLabel;
    Button validButton;
    Button undoButton;

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        lettersLabel = root.Q<Label>("LettersLabel");
        validButton = root.Q<Button>("ValidButton");
        undoButton = root.Q<Button>("UndoButton");

        validButton.RegisterCallback<ClickEvent>(evt => StartCoroutine(wordsManager.IsWordValid(lettersLabel.text)));
        
        undoButton.RegisterCallback<ClickEvent>(evt => CleanLabel());
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
