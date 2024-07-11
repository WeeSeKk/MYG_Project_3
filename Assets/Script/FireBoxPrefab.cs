using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class FireBoxPrefab : MonoBehaviour
{
    [SerializeField] SpriteRenderer goSprite;
    [SerializeField] ParticleSystem fireParticle;
    GridManager gridManager;
    WordsManager wordsManager;
    int posX;
    int posY;
    bool gameOver;
    bool moved;
    bool fire;
    public List<GameObject> hitBoxs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        EventManager.gameOverEvent += GameOver;
    }
    
    void Awake()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        wordsManager = GameObject.Find("WordsManager").GetComponent<WordsManager>();
    }

    void OnEnable()
    {
        Invoke("FindCell", 0.01f);//broken AF so use invoke
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(posY > 0 && moved == true || posY == gridManager.cellMaxHeight)
        {
            if(gridManager.gridArray[posX, posY - 1] == null)
            {
                FindCell();
            }    
        }
    }

    void OnMouseDown()
    {
        fire = true;
        goSprite.enabled = false;
        fireParticle.Play();
        BurnBoxs();
    }

    void OnDisable()
    {
        fire = false;
        goSprite.enabled = true;
        fireParticle.Stop();
        hitBoxs.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != null)
        {
            hitBoxs.Add(other.gameObject);

            if (fire)
            {
                BurnBoxs();
            }
        }
    }

    void BurnBoxs()
    {
        foreach (GameObject go in hitBoxs)
        {
            gridManager.RemoveBoxs(go);
        }
        hitBoxs.Clear();
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

        for (int i = 0; i < gridManager.gridHeight; i++)
        {
            if((i != 0 && gridManager.gridArray[x, i] == null && gridManager.gridArray[x, i - 1] != null) || (i == 0 && gridManager.gridArray[x, i] == null))
            {
                newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(x, i));
                this.gameObject.transform.DOMove(newWorldPosition, 3f, false).SetEase(Ease.OutCirc);
                gridManager.UpdateArray(this.gameObject, x, i);
                posY = i;
                moved = true;
                break;
            }  
        }
    }

    void GameOver()
    {
        gameOver = true;
    }
}
