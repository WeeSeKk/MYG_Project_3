using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;

public class MagnetBoxPrefab : MonoBehaviour
{
    GridManager gridManager;
    WordsManager wordsManager;
    [SerializeField] GameObject child;
    [SerializeField] SpriteRenderer outline;
    TMP_Text text;
    char letter;
    int posX;
    int posY;
    bool moved;
    int x1;
    int y2;
    GameObject filler;
    public List<GameObject> lockedGo = new List<GameObject>();
    public GameObject[,] fillerArray;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.gameOverEvent += GameOver;
        fillerArray = new GameObject[gridManager.gridWidth, gridManager.gridHeight];
    }
    
    void Awake()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        wordsManager = GameObject.Find("WordsManager").GetComponent<WordsManager>();

        text = child.GetComponent<TMP_Text>();
        ChooseLetter();
        filler = new GameObject();
    }

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
                        for (int i = x + 1; i < gridManager.gridHeight; i++)//look for every x position at the y position of the gameobject starting from the position of the gameobject
                        {
                            if (gridManager.gridArray[i, posY] == null)//if there is a empty space between this gameobject and the magnet return
                            {
                                return;
                            }
                            else if (gridManager.gridArray[i, posY] == this.gameObject)//else lock the gameobject
                            {
                                gameObject.transform.DOKill();//kill the precedent DOTWEEN !!!IMPORTANT!!
                            
                                Vector3 newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(x, posY));//i is the position null on the X and posY is this gameobject y Position
                                gameObject.transform.DOMove(newWorldPosition, 3f, false).SetEase(Ease.OutCirc);
                                SendPositionToArray(gameObject, x, y);
                                gridManager.UpdateArray(gameObject, x, posY);

                                EventManager.UpdatePosition(gameObject, x, posY);

                                if (!lockedGo.Contains(gameObject))
                                {
                                    lockedGo.Add(gameObject);
                                }
                                Debug.Log(lockedGo.Count);
                            }
                        }
                    }
                }
            }
        }
    }

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

    public void FindLockedGo()
    {
        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                if (lockedGo.Contains(gridManager.gridArray[x, y]))
                {
                    foreach(GameObject go in lockedGo)
                    {
                        RemoveFillerGameobject(go, x, y);
                    }
                }
            }
        }
    }

    public void RemoveFillerGameobject(GameObject gameObject, int x, int y) 
    {
        for (int i = x ; i < gridManager.gridWidth; i++)
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
                                    Debug.Log(gridManager.gridArray[a, b] + " " + a + " " + b);
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
        ChooseLetter();
        Invoke("FindCell", 0.01f);//broken AF so use invoke
    }

    void OnDisable()
    {
        for (int x = 0; x < gridManager.gridWidth; x++)
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

        //gridManager.RemoveBoxs(this.gameObject);
        Debug.Log(posX + " " +posY);
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
                    SendPositionToArray(this.gameObject, x, y);
                    return;
                }
            }
        }
    }

    void GameOver()
    {
        this.gameObject.transform.DOKill();
    }
}