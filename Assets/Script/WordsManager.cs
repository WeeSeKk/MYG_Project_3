using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
}