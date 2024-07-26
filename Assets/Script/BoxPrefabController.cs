using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class BoxPrefabController : MonoBehaviour
{
    GridManager gridManager;
    WordsManager wordsManager;
    [SerializeField] GameObject child;
    [SerializeField] SpriteRenderer freeze;
    [SerializeField] SpriteRenderer outline;
    [SerializeField] SpriteRenderer goSprite;
    [SerializeField] GameObject visualGo;
    TMP_Text text;
    Scene currentScene;
    bool isClickable;
    float mooveSpeed = 0;
    bool spawned;
    char letter;
    int posX;
    int posY;
    public List<Sprite> sprites;
    public List<Sprite> kSprites;
    public List<Sprite> outlineSprites;
    
    void Awake()
    {
        EventManager.gameOverEvent += GameOver;
        EventManager.updatePosition += UpdatePos;
        EventManager.swapLetters += SwapLetters;
        EventManager.shakeBoxs += ShakeBoxsAnimation;

        currentScene = SceneManager.GetActiveScene();
        
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        wordsManager = GameObject.Find("WordsManager").GetComponent<WordsManager>();

        text = child.GetComponent<TMP_Text>();
    }

    public void ActivateBox()
    {
        isClickable = true;
        FindCell();
    }

    void OnEnable()
    {
        ChooseSprite();
        ChooseLetter();
        outline.enabled = false;
        spawned = false;
    }

    void SwapLetters()
    {
        Vector3 rot = new Vector3(0,0,360);

        if (spawned)
        {
            ChooseLetter();

            this.gameObject.transform.DOShakeRotation(2f, rot, 3, 60f, true).SetEase(Ease.OutCirc);
        }
    }

    void OnDisable()
    {
        this.gameObject.transform.DOKill();
        
        if (gridManager.selectedBoxs.Contains(this.gameObject))
        {
            gridManager.selectedBoxs.Remove(this.gameObject);
        }
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
        /*
        System.Random rand = new System.Random();
        int num = rand.Next(0, 15);

        goSprite.sprite = kSprites[num];
        */
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

        if (!spawned)
        {
            for (int x = 0; x < gridManager.maxgridWidth; x++)
            { 
                if (gridManager.gridArray[x, 9] == this.gameObject)
                {
                    spawned = true;
                    ActivateBox();
                    break;
                }
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
        if (currentScene.name == "Scene_Gamemode_01")
        {
            letter = wordsManager.GenerateLetter();

            text.SetText(letter.ToString());
        }
        else
        {
            letter = wordsManager.AssingLetter();

            text.SetText(letter.ToString());
        }
    }

    void OnMouseDown()
    {
        if (spawned)
        {
            isClickable = true;
        }
        if (!gridManager.selectedBoxs.Contains(this.gameObject) && isClickable)
        {
            AudioManager.instance.PlayAudioClip(1);
            wordsManager.AddLetter(letter);
            gridManager.selectedBoxs.Add(this.gameObject);//add this box to the list of boxs used to create a word
        }
    }    

    public void FindCell()//find in witch cell is this gameobject
    {
        for (int x = 0; x < gridManager.maxgridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                if (gridManager.gridArray[x, y] == this.gameObject)
                {
                    posY = y;
                    posX = x;
                    spawned = true;
                    NewMoveCell(x, y);
                    return;
                }
            }
        }
    }

    void NewMoveCell(int x, int y)
    {
        Vector3 newWorldPosition;

        if (currentScene.name == "Scene_Gamemode_01")
        {
            mooveSpeed = 3f;
        }
        else
        {
            mooveSpeed = 0.1f;
        }

        for (int i = 0; i < gridManager.gridHeight; i++)
        {
            if((i != 0 && gridManager.gridArray[x, i] == null && gridManager.gridArray[x, i - 1] != null) || (i == 0 && gridManager.gridArray[x, i] == null))
            {
                newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(x, i));
                this.gameObject.transform.DOMove(newWorldPosition, mooveSpeed, false).SetEase(Ease.OutCirc);
                gridManager.UpdateArray(this.gameObject, x, i);
                posY = i;
                break;
            }  
        }
    }

    void ShakeBoxsAnimation()
    {
        visualGo.transform.DOShakePosition(1f, 0.1f, 10, 60f, false, true).OnComplete(() =>
        {
            visualGo.transform.position = this.gameObject.transform.position;
        });
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
        isClickable = false;
        //this.gameObject.transform.DOKill();
    }
}