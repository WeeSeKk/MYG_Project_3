namespace WordsManagerNamespace
{
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
        private static readonly Dictionary<char, int> letterFrequencies = new Dictionary<char, int>//List of letters with the frequencies of which they can appear
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
        public List<string> bonusWords = new List<string>();
        public List<char> lettersForChosenWords = new List<char>();

        void Update()
        {
            if (Input.GetKeyDown("w"))//test for debug
            {
                IsWordValidCategory("lion");

            }
            if (Input.GetKeyDown("q"))//test for debug
            {
                //ChooseWords();
            }
        }
        /**
        <summary>
        Return a char based on the frequencies of the letters.
        </summary>
        <param name=""></param>
        <returns></returns>
        **/
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
        /**
        <summary>
        Count the point for a correct word.
        </summary>
        <param name="word">Word created by player.</param>
        <returns></returns>
        **/
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
        /**
        <summary>
        Add letter asigned to gameobject to the label.
        </summary>
        <param name="letter">Letter of the clicked gameobject.</param>
        <returns></returns>
        **/
        public void AddLetter(char letter)
        {
            createdWord += letter;
            uIManager.UpdateLabel(createdWord);
        }
        /**
        <summary>
        Empty the word created by selecting gameobject.
        </summary>
        <param name=""></param>
        <returns></returns>
        **/
        public void ResetWord()
        {
            createdWord = null;
            gridManager.selectedBoxs.Clear();
        }

        public void test(string word)
        {
            StartCoroutine(IsWordValid(word));


        }
        /**
        <summary>
        Call the APIManager to check if wordcreated is a valid word when playing Gamemode 1.
        </summary>
        <param name="word">Word created by player.</param>
        <returns></returns>
        **/
        public IEnumerator IsWordValid(string word)//make an API call to check if the word is valid or not
        {
            if (word.Length > 1)//the word as to be at least 2 letters long
            {
                var apiCallTsk = aPIManager.SendRequestAsync("https://api.dictionaryapi.dev/api/v2/entries/en/" + word, OnValidationReceived);
                yield return new WaitUntil(() => apiCallTsk.IsCompleted);
            }
        }
        /**
        <summary>
        Check if wordcreated is in the list of words chosen when playing Gamemode 2.
        </summary>
        <param name="word">Word created by player.</param>
        <returns></returns>
        **/
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
        /**
        <summary>
        In case of correct word created remove the word from the ToFindList and add it to the FoundList when playing Gamemode 2.
        </summary>
        <param name="word">Word created by player.</param>
        <returns></returns>
        **/
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
        /**
        <summary>
        Add string from playfab to the wordsCategory List based on category choosed when playing Gamemode 2.
        </summary>
        <param name="isValid">Bool returned by APIManager to know if word is correct.</param>
        <returns></returns>
        **/
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
        /**
        <summary>
        When playing Gamemode 2 after a correct word is found check if there is still words to find.
        </summary>
        <param name=""></param>
        <returns></returns>
        **/
        void CheckIfGameOver()
        {
            if (wordsCategoryChoosen.Count <= 0)
            {
                Debug.Log("ALL WORDS FOUND");
                EventManager.GameOverEvent();
            }
        }
        /**
        <summary>
        Add string from playfab to the wordsCategory List based on category choosed when playing Gamemode 2.
        </summary>
        <param name="jsonList">String returned by Playfab Manager.</param>
        <returns></returns>
        **/
        public void AddWordsToCategoryList(string jsonList)
        {
            wordsCategory = new List<string>(jsonList.Split(','));
        }
        /**
        <summary>
        Choose what words of the category list will be a word to find when playing Gamemode 2.
        </summary>
        <param name=""></param>
        <returns></returns>
        **/
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

                string hint = new string(hintchar);

                uIManager.AddToFindLists(hint);
            }
        }
        /**
        <summary>
        Choose a letter for the gameobjects when playing Gamemode 2.
        </summary>
        <param name=""></param>
        <returns></returns>
        **/
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
        /**
        <summary>
        Show the first letter that is not already shown when selecting a word with the hint button while playing Gamemode 2.
        </summary>
        <param name="pos">Position of the word chosen in the list.</param>
        <returns></returns>
        **/
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
            string hint = new string(charToFind);

            uIManager.RemoveFromToFindList(hint, true, pos);
        }
    }
}