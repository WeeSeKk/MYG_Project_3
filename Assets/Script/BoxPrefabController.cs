using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;
using WordsManagerNamespace;

public class BoxPrefabController : MonoBehaviour
{
    GridManager gridManager;
    WordsManager wordsManager;
    [SerializeField] GameObject child;
    [SerializeField] SpriteRenderer freeze;
    [SerializeField] SpriteRenderer outline;
    [SerializeField] GameObject outlineGO;
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
    const string BOXSSKIN = "Skin";
    public List<Sprite> sprites;
    public List<Sprite> kSprites;
    public List<Sprite> sSprites;
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
    /**
        <summary>
        Change the assigned letter of the gameobject.
        </summary>
        <param name=""></param>
        <returns></returns>
    **/
    void SwapLetters()
    {
        Vector3 rot = new Vector3(0, 0, 360);

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
    /**
        <summary>
        Choose the sprite of the gameobject based on playerpref.
        </summary>
        <param name=""></param>
        <returns></returns>
    **/
    void ChooseSprite()
    {
        string sprite = PlayerPrefs.GetString(BOXSSKIN, "default");

        if (sprite == "default")//default sprite
        {
            System.Random rdm = new System.Random();
            int i = rdm.Next(0, 4);

            goSprite.sprite = sprites[i];
            text.color = Color.black;
            visualGo.transform.localScale = new Vector3(1f, 1f);
        }
        else if (sprite == "kenney")//k sprite
        {
            System.Random rdm = new System.Random();
            int i = rdm.Next(0, 15);

            goSprite.sprite = kSprites[i];
            text.color = Color.black;
            visualGo.transform.localScale = new Vector3(1.45f, 1.45f);

            if (i == 1 || i == 5 || i == 9 || i == 13)
            {
                outline.sprite = outlineSprites[1];
                outlineGO.transform.localScale = new Vector3(0.9f, 0.9f);
                outlineGO.transform.localRotation = Quaternion.Euler(0, 0, -45f);
            }
            if (i == 0 || i == 4 || i == 8 || i == 12 || i == 15)
            {
                outline.sprite = outlineSprites[0];
            }
            else
            {
                outline.sprite = outlineSprites[1];
            }
        }
        else if (sprite == "simple")//s sprite
        {
            System.Random rdm = new System.Random();
            int i = rdm.Next(0, 3);

            text.color = Color.white;

            if (i == 0)
            {
                outline.sprite = outlineSprites[1];
                visualGo.transform.localScale = new Vector3(0.45f, 0.45f);
                outlineGO.transform.localScale = new Vector3(1.05f, 1.05f);
                goSprite.sprite = sSprites[0];
            }
            else
            {
                outline.sprite = outlineSprites[0];
                visualGo.transform.localScale = new Vector3(1.2f, 1.2f);
                outlineGO.transform.localScale = new Vector3(1.05f, 1.05f);
                goSprite.sprite = sSprites[1];
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (posY > 0)
        {
            if (gridManager.gridArray[posX, posY - 1] == null)
            {
                FindCell();
            }
        }

        if (!spawned)//If this gameobject is in the maingrid activate it 
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
    /**
        <summary>
        Update position of the gameobject (Only used by magnet powerUp).
        </summary>
        <param name="gameObject">All gameobject in the list of locked gameobject of the magnet.</param>
        <param name="x">New x position of this gameobject.</param>
        <param name="y">New y position of this gameobject.</param>
        <returns></returns>
    **/
    void UpdatePos(GameObject gameObject, int x, int y)
    {
        if (gameObject == this.gameObject)
        {
            posX = x;
            posY = y;
        }
    }
    /**
        <summary>
        Asign letter to the gameobject.
        </summary>
        <param name=""></param>
        <returns></returns>
    **/
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
    /**
        <summary>
        Find this gameobject on the grid.
        </summary>
        <param name=""></param>
        <returns></returns>
    **/
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
    /**
        <summary>
        Move the gameobject down in the grid if empty.
        </summary>
        <param name="x">x position of this gameobject in the array</param>
        <param name="y">y position of this gameobject in the array</param>
        <returns></returns>
    **/
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
            if ((i != 0 && gridManager.gridArray[x, i] == null && gridManager.gridArray[x, i - 1] != null) || (i == 0 && gridManager.gridArray[x, i] == null))
            {
                newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(x, i));
                this.gameObject.transform.DOMove(newWorldPosition, mooveSpeed, false).SetEase(Ease.OutCirc);
                gridManager.UpdateArray(this.gameObject, x, i);
                posY = i;
                break;
            }
        }
    }
    /**
        <summary>
        <param> </param>
        <returns> Shake all gameobject (Only used by crusher powerUp) </returns>
        </summary>
    **/
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
            //freeze.DOFade(180, 5000f).SetEase(Ease.Linear);
        }
    }

    void GameOver()
    {
        isClickable = false;
        //this.gameObject.transform.DOKill();
    }
}