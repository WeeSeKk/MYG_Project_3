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
    [SerializeField] GameObject visualGo;
    bool isClickable;
    bool spawned;
    TMP_Text text;
    char letter;
    int posX;
    int posY;
    public List<GameObject> explosionRange = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        EventManager.gameOverEvent += GameOver;
        EventManager.updatePosition += UpdatePos;
        EventManager.swapLetters += SwapLetters;
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
        boxCollider2D.enabled = false;
        goSprite.enabled = true;
        isClickable = false;
        ChooseLetter();
        explosionRange.Clear();
        visualGo.transform.localScale= new Vector3(1f, 1f);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        spawned = false;
        this.gameObject.transform.DOKill();
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
        if (spawned)
        {
            isClickable = true;
        }
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
                    spawned = true;
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
        int timeTicking = 0;
        int maxTicking = 10;
        float timeToTick = 1f;
        int lastTicking = 0;

        Vector3 initScale = visualGo.transform.localScale;
        Vector3 scaleBig = new Vector3(1.5f, 1.5f);

        while (timeTicking < maxTicking)
        {
           //scale up animation
            visualGo.transform.DOScale(scaleBig, timeToTick);
            yield return new WaitForSeconds(timeToTick);

           //scale down animation
            visualGo.transform.DOScale(initScale, timeToTick);
            yield return new WaitForSeconds(timeToTick);

            timeToTick -= 0.1f;
            timeTicking ++;
        }

        while (lastTicking < maxTicking)
        {
           //scale up animation
            visualGo.transform.DOScale(scaleBig, 0.1f);
            yield return new WaitForSeconds(0.1f);

           //scale down animation
            visualGo.transform.DOScale(initScale, 0.1f);
            yield return new WaitForSeconds(0.1f);

            lastTicking ++;
        }

        goSprite.enabled = false;
        _particleSystem.Play();
        text.SetText("");

        List<GameObject> explosionRangeCopy = new List<GameObject>(explosionRange);

        foreach (GameObject gameObject in explosionRangeCopy)
        {
            if (gameObject != null)
            {
                gridManager.RemoveBoxs(gameObject);
                explosionRange.Remove(gameObject);
            }
        }

        yield return new WaitForSeconds(0.5f);

        gridManager.RemoveBoxs(this.gameObject);
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