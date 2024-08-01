using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using WordsManagerNamespace;

public class MagnetBoxPrefab : MonoBehaviour
{
    GridManager gridManager;
    WordsManager wordsManager;
    [SerializeField] GameObject child;
    [SerializeField] BoxCollider2D _boxCollider2D;
    [SerializeField] SpriteRenderer outline;
    TMP_Text text;
    char letter;
    bool spawned;
    int posX;
    int posY;
    bool isClickable;
    GameObject filler;
    public List<GameObject> lockedGo = new List<GameObject>();
    public GameObject[,] fillerArray;
    
    void Awake()
    {
        EventManager.gameOverEvent += GameOver;
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        wordsManager = GameObject.Find("WordsManager").GetComponent<WordsManager>();

        text = child.GetComponent<TMP_Text>();
        ChooseLetter();
        filler = new GameObject();
    }
    /**
        <summary>
        Look for every x position between gameobject param and this gameobject and if no empty cell is found lock the parm gameobject.
        </summary>
        <param name="gameObject">Gameobject who enter the box collider of this gameobject</param>
        <returns></returns>
    **/
    public void LockBoxPosition(GameObject gameObject)
    {
        if (!lockedGo.Contains(gameObject))
        {
            for (int x = 0; x < gridManager.gridWidth; x++)
            {
                for (int y = 0; y < gridManager.gridHeight; y++)
                {
                    if (gridManager.gridArray[x, y] == gameObject)//found the gameobject that enter this gameobject box collider
                    {
                        for (int i = x + 1; i < gridManager.gridWidth; i++)//look for every x position at the y position of the gameobject starting from the position of the gameobject
                        {
                            if (gridManager.gridArray[i, posY] == null)//if there is a empty space between this gameobject and the magnet return
                            {
                                return;
                            }
                            else if (gridManager.gridArray[i, posY] == this.gameObject)//else lock the gameobject
                            {
                                gameObject.transform.DOKill();//kill the precedent DOTWEEN !!!IMPORTANT!!
                            
                                Vector3 newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(x, posY));//i is the position null on the X and posY is this gameobject y Position
                                gameObject.transform.DOMove(newWorldPosition, 3f, false).SetEase(Ease.OutElastic);
                                SendPositionToArray(gameObject, x, y);
                                gridManager.UpdateArray(gameObject, x, posY);

                                EventManager.UpdatePosition(gameObject, x, posY);

                                if (!lockedGo.Contains(gameObject))
                                {
                                    lockedGo.Add(gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    /**
        <summary>
        Update the position of the param locked gameobject and fill the array under the gameobject with filler gameobject.
        </summary>
        <param name="gameObject">Gameobject who enter the box collider of this gameobject</param>
        <param name="x">Position x in the array of the gameobject who enter the box collider of this gameobject</param>
        <param name="y">Position y in the array of the gameobject who enter the box collider of this gameobject</param>
        <returns></returns>
    **/
    void SendPositionToArray(GameObject gameObject, int x, int y)
    {
        if (gameObject == this.gameObject)//full the array under this gameobject
        {
            for (int i = 0; i < posY; i ++)
            {
                if (gridManager.gridArray[posX, i] == null)
                {
                    gridManager.gridArray[posX, i] = filler;
                    fillerArray[posX, i] = gridManager.gridArray[posX, i];
                }
            }
        }
        else//full the array under any catch gameobject
        {
            for (y = 0; y < posY; y ++)
            {
                if (gridManager.gridArray[x, y] == null)
                {
                    gridManager.gridArray[x, y] = filler;
                    fillerArray[x, y] = gridManager.gridArray[x, y];
                }
            }
        }
    }
    /**
        <summary>
        Find in the array the gameobject in the list of locked gameobject.
        </summary>
        <param name=""></param>
        <returns></returns>
    **/
    public void FindLockedGo()
    {
        for (int x = 0; x < gridManager.maxgridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                if (lockedGo.Contains(gridManager.gridArray[x, y]))
                {
                    foreach(GameObject go in lockedGo)
                    {
                        RemoveFillerGameobject(x, y);
                    }
                }
            }
        }
    }
    /**
        <summary>
        Remove filler gameobject under a locked gameobject who should not be locked anymore.
        </summary>
        <param name="x">Position x in the array where filler gameobject need to be removed</param>
        <param name="y">Position y in the array where filler gameobject need to be removed</param>
        <returns></returns>
    **/
    public void RemoveFillerGameobject(int x, int y) 
    {
        for (int i = x ; i < gridManager.maxgridWidth; i++)
        {
           if (gridManager.gridArray[i, posY] == null)//found a empty spot 
            {
                for (y = 0; y < posY; y ++)
                {
                    if (gridManager.gridArray[i, y] != null)
                    {
                        for (int a = 0; a < i + 1; a ++)
                        {
                            for (int b = 0; b < gridManager.gridHeight; b ++)
                            { 
                                if (fillerArray[a, b] != null)
                                {
                                    gridManager.gridArray[a, b] = null;
                                    fillerArray[a, b] = null;
                                }
                            }
                        }
                    }
                }
            }
            else if (gridManager.gridArray[i, posY] == this.gameObject)//no empty spot between go and magnet
            {
                return;
            }
        }  
    }

    void OnEnable()
    {
        fillerArray = new GameObject[gridManager.gridWidth, gridManager.gridHeight];
        _boxCollider2D.enabled = false;
        ChooseLetter();
        isClickable = false;
    }
    /**
        <summary>
        Remove every filler gameobjects.
        </summary>
        <param name=""></param>
        <returns></returns>
    **/
    void OnDisable()
    {
        _boxCollider2D.enabled = false;

       for (int x = 0; x < gridManager.maxgridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                if (fillerArray[x, y] != null)
                {
                    fillerArray[x, y] = null;
                    gridManager.gridArray[x, y] = null;
                }
            }
        }
        lockedGo.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawned)//If this gameobject is in the maingrid activate it 
        {
            for (int x = 0; x < gridManager.maxgridWidth; x++)
            { 
                if (gridManager.gridArray[x, 9] == this.gameObject)
                {
                    ActivateBox();
                    spawned = true;
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
        Asign letter to the gameobject.
        </summary>
        <param name=""></param>
        <returns></returns>
    **/
    void ChooseLetter()
    {
        letter = wordsManager.GenerateLetter();

        text.SetText(letter.ToString());
    }

    public void ActivateBox()
    {
        isClickable = true;
        FindCell();
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
    public void FindCell()
    {
        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                if (gridManager.gridArray[x, y] == this.gameObject)
                {
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

        posX = gridManager.gridWidth - 1;
        posY = y - 5;
            
        newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(posX, posY));
        this.gameObject.transform.DOMove(newWorldPosition, 3f, false).SetEase(Ease.OutCirc).OnComplete(() => {

            _boxCollider2D.enabled = true;
        });

        gridManager.UpdateArray(this.gameObject, posX, posY);
        SendPositionToArray(this.gameObject, posX, posY);
    }

    void GameOver()
    {
        this.gameObject.transform.DOKill();
    }
}