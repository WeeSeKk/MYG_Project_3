using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class BoxPrefabController : MonoBehaviour
{
    GridManager gridManager;
    WordsManager wordsManager;
    [SerializeField] GameObject child;
    [SerializeField] SpriteRenderer outline;
    TMP_Text text;
    char letter;
    int posX;
    int posY;
    bool gameOver;
    bool moved;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.gameOverEvent += GameOver;
        EventManager.boxsFly += MakeBoxsFly;
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
        Invoke("FindCell", 0.01f);//broken AF so use invoke
        gameOver = false;
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
    
        if (gridManager.selectedBoxs.Contains(this.gameObject))
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;
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
                    NewMoveCell(x, y);
                    break;
                }
            }
        }
    }

    void NewMoveCell(int x, int y)
    {
        Vector3 newWorldPosition;

        for (int i = 0; i < gridManager.gridHeight; i++)
        {
            if((i != 0 && gridManager.gridArray[x, i] == null && gridManager.gridArray[x, i - 1] != null) || (i == 0 && gridManager.gridArray[x, i] == null))
            {
                newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(x, i));
                this.gameObject.transform.DOMove(newWorldPosition, 3f, false);
                gridManager.UpdateArray(this.gameObject, x, i);
                posY = i;
                moved = true;
                break;
            }  
        }
    }

    void MakeBoxsFly()
    {
        if(gridManager.selectedBoxs.Contains(this.gameObject))
        {
            System.Random random = new System.Random();
            int i = random.Next(0, 20);

            Vector2 jumpEnd = new Vector2(i, 10);
            this.gameObject.transform.DOJump(jumpEnd, 2f, 1, 1f, false);
        }
    }

    void GameOver()
    {
        gameOver = true;
    }
}