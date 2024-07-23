using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeathBoxPrefab : MonoBehaviour
{
    [SerializeField] GameObject visualGo;
    GridManager gridManager;
    WordsManager wordsManager;
    int posX;
    int posY;
    bool spawned;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.gameOverEvent += GameOver;
        EventManager.updatePosition += UpdatePos;
        EventManager.shakeBoxs += ShakeBoxsAnimation;
    }
    
    void Awake()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        wordsManager = GameObject.Find("WordsManager").GetComponent<WordsManager>();
    }

    public void ActivateBox()
    {
        FindCell();
    }

    void UpdatePos(GameObject gameObject, int x, int y)
    {
        if(gameObject == this.gameObject)
        {
            posX = x;
            posY = y;
        }
    }

    void OnDisable()
    {
        this.gameObject.transform.DOKill();
        spawned = false;
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
                    FindCell();
                    spawned = true;
                    break;
                }
            }
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
                this.gameObject.transform.DOMove(newWorldPosition, 3f, false).SetEase(Ease.OutCirc);
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

    void GameOver()
    {
        this.gameObject.transform.DOKill();
    }
}
