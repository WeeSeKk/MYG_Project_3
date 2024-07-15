using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

public class BombBoxPrefab : MonoBehaviour
{
    GridManager gridManager;
    WordsManager wordsManager;
    [SerializeField] GameObject child;
    [SerializeField] BoxCollider2D boxCollider2D;
    [SerializeField] SpriteRenderer outline;
    [SerializeField] SpriteRenderer goSprite;
    [SerializeField] ParticleSystem _particleSystem;
    bool isClickable;
    bool spawned;
    TMP_Text text;
    char letter;
    int posX;
    int posY;
    int maxCount = 20;
    int count = 0;  
    float timeToTick = 2f;
    public List<GameObject> explosionRange = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        EventManager.gameOverEvent += GameOver;
        EventManager.updatePosition += UpdatePos;
    }
    
    void Awake()
    {
        EventManager.updatePosition += UpdatePos;

        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        wordsManager = GameObject.Find("WordsManager").GetComponent<WordsManager>();

        text = child.GetComponent<TMP_Text>();
        ChooseLetter();
    }

    void OnEnable()
    {
        boxCollider2D.enabled = false;
        goSprite.enabled = true;
        timeToTick = 2f;
        isClickable = false;
        ChooseLetter();
    }

    void OnDisable()
    {
        StopAllCoroutines();
        spawned = false;
        this.gameObject.transform.DOKill();
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
                for (int y = 0; y < gridManager.gridHeight; y++)
                {
                    if (gridManager.gridArray[x, y] == this.gameObject)
                    {
                        ActivateBox();
                        spawned = true;
                        break;
                    }
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

    public void ActivateBox()
    {
        FindCell();
        boxCollider2D.enabled = true;
        isClickable = true;
        StartCoroutine(BombTicking());
    }

    void ChooseLetter()
    {
        letter = wordsManager.GenerateLetter();

        text.SetText(letter.ToString());
    }

    void OnMouseDown()
    {
        if (!gridManager.selectedBoxs.Contains(this.gameObject) && isClickable)
        {
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
                    NewMoveCell(x, y);
                    return;
                }
            }
        }
    }

    public void AddGoToList(GameObject gameObject)
    {
        if (!explosionRange.Contains(gameObject))
        {
            explosionRange.Add(gameObject);
        }
        else
        {
            explosionRange.Remove(gameObject);
        }
    }

    IEnumerator BombTicking()
    {
        List<GameObject> toRemove = new List<GameObject>();

        bool ticking = true;

        Vector3 initScale = this.gameObject.transform.localScale;
        Vector3 scaleBig = new Vector3(0.4f, 0.4f);

        while (ticking == true)
        {
            if (count == maxCount)
            {
                ticking = false;
                
                text.SetText("");
                goSprite.enabled = false;
                _particleSystem.Play();

                foreach (GameObject go in explosionRange)
                {
                    toRemove.Add(go);
                }

                foreach (GameObject go in toRemove)
                {
                    gridManager.RemoveBoxs(go);
                }

                toRemove.Clear();

                yield return new WaitForSeconds(1f);

    
                gridManager.RemoveBoxs(this.gameObject);
        
            }
            else if (count < maxCount)
            {
                yield return new WaitForSeconds(timeToTick * 2);
                
                this.gameObject.transform.DOScale(scaleBig, timeToTick).SetEase(Ease.OutCirc).OnComplete(() => {

                    this.gameObject.transform.DOScale(initScale, timeToTick).SetEase(Ease.OutCirc).OnComplete(() => {

                        count ++;

                        if (timeToTick > 0.2)
                        {
                            timeToTick -= 0.2f;
                        }
                        else if (timeToTick == 0.2)
                        {
                            timeToTick -= 0.1f;
                        }               
                    });
                });
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

    void GameOver()
    {
        this.gameObject.transform.DOKill();
    }
}