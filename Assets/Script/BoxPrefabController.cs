using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class BoxPrefabController : MonoBehaviour
{
    GridManager gridManager;
    WordsManager wordsManager;
    [SerializeField] GameObject child;
    TMP_Text text;
    char letter;
    int posX;
    int posY;
    bool moved;
    bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.gameOverEvent += GameOver;
        FindCell();
    }
    
    void Awake()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        wordsManager = GameObject.Find("WordsManager").GetComponent<WordsManager>();

        text = child.GetComponent<TMP_Text>();
        ChooseLetter();
    }

    void OnEnable()
    {
        ChooseLetter();
        //FindCell();
        gameOver = false;
        Invoke("FindCell", 0.01f);//broken AF don't know why so use invoke
    }

    // Update is called once per frame
    void Update()
    {
        if(posY > 0 && moved == true || posY == gridManager.cellMaxHeight)
        {
            if(gridManager.gridArray[posX, posY - 1] == null)
            {
                FindCell();
            }    
        }
    }

    void ChooseLetter()
    {
        System.Random rand = new System.Random();
        int num = rand.Next(0, 100);

        if (num < 50)
        {
            string chars = "etaoinshr"; 

            System.Random random = new System.Random();
            int i = random.Next(0, chars.Length);
            letter = chars[i];

            text.SetText(letter.ToString());
        }
        else if (num >= 50 && num < 85)
        {
            string chars = "dlcumwfgy"; 

            System.Random random = new System.Random();
            int i = random.Next(0, chars.Length);
            letter = chars[i];

            text.SetText(letter.ToString());
        }
        else if (num >= 85)
        {
            string chars = "pbvkjxqz"; 

            System.Random random = new System.Random();
            int i = random.Next(0, chars.Length);
            letter = chars[i];

            text.SetText(letter.ToString());
        }
    }

    void OnMouseDown()
    {
        if (!gridManager.selectedBoxs.Contains(this.gameObject) && Time.timeScale == 1)
        {
            wordsManager.AddLetter(letter);
            gridManager.selectedBoxs.Add(this.gameObject);//add this box to the list of boxs used to create a word
        }

        Debug.Log(posX + " " + posY);
    }

    public void FindCell()//find in witch cell is this gameobject
    {
        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                if (gridManager.gridArray[x, y] == this.gameObject)
                {
                    posY = y;
                    posX = x;
                    StartCoroutine(MoveCell(x, y));
                    break;
                }
            }
        }
    }

    IEnumerator MoveCell(int x, int y)//if a cell have a empty cell under it then go down by one cell and continue until there is a cell or a max vector
    {
        while (y > 0 && gridManager.gridArray[x, y - 1] == null && !gameOver)
        {
            yield return new WaitForSeconds(1f);
            Vector3 newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(x, y - 1));
            this.gameObject.transform.position = newWorldPosition;
            gridManager.UpdateArray(this.gameObject, x, y);
            y -= 1;
            posY = y;
        }
        moved = true;
    }

    void GameOver()
    {
        gameOver = true;
    }
}