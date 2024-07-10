using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WordsManager : MonoBehaviour
{
    [SerializeField] APIManager aPIManager;
    [SerializeField] GridManager gridManager;
    [SerializeField] UIManager uIManager;
    string createdWord;

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
            Debug.Log("VALID");
            StartCoroutine(gridManager.RemoveSelectedBox());
            uIManager.AddList(createdWord);
            uIManager.CleanLabel();
        }
        else
        {
            //word is not valid
            uIManager.CleanLabel();
            Debug.Log("UNVALID");
        }
        ResetWord(); //reset the created word 
    }
}