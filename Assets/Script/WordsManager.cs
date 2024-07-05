using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordsManager : MonoBehaviour
{
    [SerializeField] APIManager aPIManager;
    [SerializeField] GridManager gridManager;
    [SerializeField] UIManager uIManager;

    string createdWord;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            StartCoroutine(IsWordValid(createdWord));
        }
    }

    public void AddLetter(char letter)
    {
        createdWord += letter;
        uIManager.UpdateLabel(createdWord);
    }

    public void ResetWord()
    {
        createdWord = null;
    }

    public IEnumerator IsWordValid(string word)
    {
        yield return StartCoroutine(aPIManager.SendRequest("https://api.dictionaryapi.dev/api/v2/entries/en/" + word));

        if(aPIManager.isValid == true)
        {
            //word is valid
            uIManager.CleanLabel();
            Debug.Log("VALID");
            gridManager.RemoveBox();
        }
        else
        {
            //word is not valid
            uIManager.CleanLabel();
            Debug.Log("UNVALID");
        }
    }
}