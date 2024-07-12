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
    [SerializeField] SpriteRenderer freeze;
    [SerializeField] SpriteRenderer outline;
    [SerializeField] SpriteRenderer goSprite;
    TMP_Text text;
    char letter;
    int posX;
    int posY;
    public List<Sprite> sprites;
    public List<Sprite> outlineSprites;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.gameOverEvent += GameOver;
        EventManager.updatePosition += UpdatePos;
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
        ChooseSprite();
        ChooseLetter();
        Invoke("FindCell", 0.01f);//broken AF so use invoke
    }

    void OnDisable()
    {
        this.gameObject.transform.DOKill();
    }

    void ChooseSprite()
    {
        System.Random rand = new System.Random();
        int num = rand.Next(0, 4);

        goSprite.sprite = sprites[num];

        if (num > 1)
        {
            outline.sprite = outlineSprites[1];
        }
        else
        {
            outline.sprite = outlineSprites[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(posY > 0)
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

    void UpdatePos(GameObject gameObject, int x, int y)
    {
        if(gameObject == this.gameObject)
        {
            posX = x;
            posY = y;
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

        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                if (gridManager.gridArray[x, y] == this.gameObject)
                {
                    Debug.Log(x + " " + y);
                }
            }
        }
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
                    return;
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
                this.gameObject.transform.DOMove(newWorldPosition, 3f, false).SetEase(Ease.OutCirc);
                gridManager.UpdateArray(this.gameObject, x, i);
                posY = i;
                break;
            }  
        }
    }

    public void Freezing(GameObject gameObject)
    {
        if (gameObject == this.gameObject)
        {
            freeze.DOFade(180, 5000f).SetEase(Ease.Linear);
        }
    }

    void GameOver()
    {
        this.gameObject.transform.DOKill();
    }
}