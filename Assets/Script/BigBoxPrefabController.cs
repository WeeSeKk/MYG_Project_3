using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BigBoxPrefabController : MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider2D;
    GridManager gridManager;
    int posX;
    int posY;
    bool spawned;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.gameOverEvent += GameOver;
        EventManager.updatePosition += UpdatePos;
    }
    
    void Awake()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        gridManager.RemoveBoxs(collision.gameObject);
        EventManager.ShakeBoxs();
        FindCell();
    }
    void OnDisable()
    {
        spawned = false;
        this.gameObject.transform.DOKill();
    }

    void UpdatePos(GameObject gameObject, int x, int y)
    {
        if(gameObject == this.gameObject)
        {
            posX = x;
            posY = y;
        }
    }

    void OnEnable()
    {
        this.gameObject.transform.localScale = new Vector3(0.4f, 0.4f, 1);

        if (boxCollider2D.enabled == true)
        {
            boxCollider2D.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawned)
        {
            for (int x = 0; x < gridManager.maxgridWidth; x++)
            { 
                if (gridManager.gridArray[x, 9] == this.gameObject)
                {
                    boxCollider2D.enabled = true;
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
                    NewMoveCell(x, y);
                    break;
                }
            }
        }
    }

    void NewMoveCell(int x, int y)
    {
        Vector3 newWorldPosition;
        this.gameObject.transform.DOKill();
        this.gameObject.transform.localScale = new Vector3(1, 1, 1);

        for (int i = 0; i < 9; i++)
        {
            if (gridManager.gridArray[x, 0] == null)  
            {
                newWorldPosition = new Vector3( this.gameObject.transform.position.x,  this.gameObject.transform.position.y - 20,  this.gameObject.transform.position.z);
                this.gameObject.transform.DOMove(newWorldPosition, 4f, false).SetEase(Ease.OutBounce).OnComplete(() => {

                    this.gameObject.transform.DOKill();
                    gridManager.RemoveBoxs(this.gameObject);
                });
            }
            
            else if((i != 0 && gridManager.gridArray[x, i] == null && gridManager.gridArray[x, i - 1] != null) || (i == 0 && gridManager.gridArray[x, i] == null))
            {
                newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(x, i + 1));
                this.gameObject.transform.DOMove(newWorldPosition, 2f, false).SetEase(Ease.OutBounce).OnComplete(() => {
                    
                    gridManager.UpdateArray(this.gameObject, x, i);
                    posY = i;
                });
            }
        }
    }

    void GameOver()
    {
        this.gameObject.transform.DOKill();
    }
}