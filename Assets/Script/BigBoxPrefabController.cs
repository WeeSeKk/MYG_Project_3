using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BigBoxPrefabController : MonoBehaviour
{
    GridManager gridManager;
    int posX;
    int posY;
    public List<GameObject> hitBoxs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        EventManager.gameOverEvent += GameOver;
    }
    
    void Awake()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hitBoxs.Contains(collision.gameObject))
        {
            hitBoxs.Add(collision.gameObject);
        }

        Debug.Log(hitBoxs.Count);

        foreach (GameObject go in hitBoxs)
        {
            gridManager.RemoveBoxs(go);
        }

        EventManager.ShakeBoxs();
        FindCell();
        hitBoxs.Clear();
    }

    void OnEnable()
    {
        Invoke("FindCell", 0.01f);//broken AF so use invoke
    }

    // Update is called once per frame
    void Update()
    {
    
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
        this.gameObject.transform.DOKill();

        for (int i = 0; i < 9; i++)
        {
            if (gridManager.gridArray[x, 0] == null)  
            {
                newWorldPosition = new Vector3( this.gameObject.transform.position.x,  this.gameObject.transform.position.y - 10,  this.gameObject.transform.position.z);
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