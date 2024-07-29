using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Rendering;
using GameManagerNamespace;

public class WordsManager : MonoBehaviour
{
    [SerializeField] APIManager aPIManager;
    [SerializeField] GridManager gridManager;
    [SerializeField] UIManager uIManager;
    private static readonly Dictionary<char, int> letterFrequencies = new Dictionary<char, int>
    {
        { 'a', 8 }, { 'b', 2 }, { 'c', 3 }, { 'd', 4 }, { 'e', 13 }, { 'f', 2 }, { 'g', 2 }, { 'h', 6 },
        { 'i', 7 }, { 'j', 1 }, { 'k', 1 }, { 'l', 4 }, { 'm', 2 }, { 'n', 7 }, { 'o', 8 }, { 'p', 2 },
        { 'q', 1 }, { 'r', 6 }, { 's', 6 }, { 't', 9 }, { 'u', 3 }, { 'v', 1 }, { 'w', 2 }, { 'x', 1 },
        { 'y', 2 }, { 'z', 1 }
    };
    string createdWord;
    System.Random random = new System.Random();
    public List<string> correctWordsFound = new List<string>();
    List<string> wordsCategory;
    public List<string> wordsCategoryChoosen = new List<string>();
    List<string> bonusWords = new List<string>();
    public List<char> lettersForChosenWords = new List<char>();

    void Update()
    {
        if (Input.GetKeyDown("w"))//test for debug
        {
            //Time.timeScale = 5;

        }
        if (Input.GetKeyDown("q"))//test for debug
        {
            //Time.timeScale = 5;
            ChooseWords();
        }
    }

    public char GenerateLetter()
    {
        int totalWeight = 0;

        foreach (var weight in letterFrequencies.Values)
        {
            totalWeight += weight;
        }

        int randomValue = random.Next(0, totalWeight);

        foreach (var letter in letterFrequencies.Keys)
        {
            if (randomValue < letterFrequencies[letter])
            {
                return letter;
            }
            randomValue -= letterFrequencies[letter];
        }

        return 'a'; 
    }

    void CountWordPoint(string word)
    {
        int value;
        int wordScore = 0;
        
        foreach (char letter in word)
        {
            letterFrequencies.TryGetValue(letter, out value);
            GameManager.instance.CountScore(value);
            
        }

        foreach (char letter in word)
        {
            letterFrequencies.TryGetValue(letter, out value);

            wordScore += (15 - value);
        }

        StartCoroutine(uIManager.ShowPointUp(wordScore));
    }

    public void AddLetter(char letter)
    {
        createdWord += letter;
        uIManager.UpdateLabel(createdWord);
    }

    public void ResetWord()
    {
        createdWord = null;
        gridManager.selectedBoxs.Clear();
    }

    public IEnumerator IsWordValid(string word)//make an API call to check if the word is valid or not
    {
        if (word.Length > 1)//the word as to be at least 2 letters long
        {
            yield return StartCoroutine(aPIManager.SendRequest("https://api.dictionaryapi.dev/api/v2/entries/en/" + word, OnValidationReceived));
        }
    }

    public void IsWordValidCategory(string word)
    {
        if (wordsCategoryChoosen.Contains(word))
        {
            OnValidationReceived(true);
            CallReplaceCategoryWord(word);
            uIManager.AddToFoundLists(word);
            CheckIfGameOver();
        }
        else if (!wordsCategoryChoosen.Contains(word) && wordsCategory.Contains(word) && !bonusWords.Contains(word))
        {
            Debug.Log("BONUS POINTS");
            bonusWords.Add(word);
            AudioManager.instance.PlayAudioClip(2);
            CountWordPoint(createdWord);
            gridManager.selectedBoxs.Clear();
            uIManager.CleanLabel();
            StartCoroutine(uIManager.ShowGreenLine());
            ResetWord();
        }
        else 
        {
            OnValidationReceived(false);
        }
    }

    void CallReplaceCategoryWord(string word)
    {
        for (int i = 0; i < wordsCategoryChoosen.Count; i++)
        {
            if (word == wordsCategoryChoosen[i])
            {
                uIManager.RemoveFromToFindList(word, false, i);
                wordsCategoryChoosen.Remove(wordsCategoryChoosen[i]);
            }
        }
    }

    void OnValidationReceived(bool isValid)
    {
        if (isValid)
        {
            //word is valid
            AudioManager.instance.PlayAudioClip(2);
            Debug.Log("VALID");
            StartCoroutine(gridManager.RemoveSelectedBox());
            uIManager.AddList(createdWord);
            correctWordsFound.Add(createdWord);
            CountWordPoint(createdWord);
            uIManager.CleanLabel();
            StartCoroutine(uIManager.ShowGreenLine());
        }
        else
        {
            //word is not valid
            AudioManager.instance.PlayAudioClip(3);
            uIManager.CleanLabel();
            Debug.Log("UNVALID");
            StartCoroutine(uIManager.ShowRedLine());
        }
        
        ResetWord(); //reset the created word 
    }

    void CheckIfGameOver()
    {
        if (wordsCategoryChoosen.Count == 0)
        {
            Debug.Log("ALL WORDS FOUND");
            EventManager.GameOverEvent();
        }
    }

    public void AddWordsToCategoryList(string jsonList)
    {
        wordsCategory = new List<string>(jsonList.Split(','));
    }

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

            string hint = new string (hintchar);

            uIManager.AddToFindLists(hint);
        }
    }

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
        string hint = new string (charToFind);

        uIManager.RemoveFromToFindList(hint, true, pos);
    }
}