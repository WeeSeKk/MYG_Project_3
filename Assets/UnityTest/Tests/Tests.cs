using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using GameManagerNamespace;
using WordsManagerNamespace;
using PlayfabManagerNamespace;

public class Tests
{
    [UnityTest]
    public IEnumerator _0FindUIDocument()
    {
        SceneManager.LoadScene(0);
        yield return null;

        GameObject go = GameObject.Find("UIDocument");
        UIDocument uIDocument = go.GetComponent<UIDocument>();
        Assert.IsNotNull(uIDocument);
    }

    [UnityTest]
    public IEnumerator _1LoginSuccess()
    {
        yield return SceneManager.LoadSceneAsync("IntroScene");

        PlayfabManager.instance.OnLogin("test", "123456");

        yield return new WaitForSeconds(2);

        GameManager.instance.LaunchLobby();

        string playerUsername = PlayfabManager.instance.Player_Username();

        Assert.IsTrue(playerUsername != null);
    }

    [UnityTest]
    public IEnumerator _2LoadGamemode1AndFindWords()
    {
        GameManager.instance.LaunchLobby();

        yield return new WaitForSeconds(2);

        GameManager.instance.LaunchGamemode_1();

        yield return new WaitForSeconds(2);

        GameObject go = GameObject.Find("BoxParent");
        
        Assert.IsTrue(go.transform.childCount > 0);
        
        GameObject go1 = GameObject.Find("WordsManager");
        WordsManager wordsManager = go1.GetComponent<WordsManager>();

        string word = "word";

        foreach (char letter in word)
        {
            wordsManager.AddLetter(letter);
        }

        wordsManager.test(word);

        yield return new WaitForSeconds(2);

        Assert.IsTrue(wordsManager.correctWordsFound.Count > 0, "WordsFound Null");
    }

    [UnityTest]
    public IEnumerator _3LoadGamemode2AndFindWords()
    {
        GameManager.instance.LaunchLobby();

        yield return new WaitForSeconds(2);

        GameManager.instance.LaunchGamemode_2("animals");

        yield return new WaitForSeconds(2);

        GameObject go = GameObject.Find("WordsManager");
        WordsManager wordsManager = go.GetComponent<WordsManager>();

        Assert.IsTrue(wordsManager.wordsCategoryChoosen.Count > 0, "Category Null");

        string word = "lion";

        foreach (char letter in word)
        {
            wordsManager.AddLetter(letter);
        }

        wordsManager.IsWordValidCategory(word);

        Assert.IsTrue(wordsManager.correctWordsFound.Count > 0 || wordsManager.bonusWords.Count > 0);
    }
}