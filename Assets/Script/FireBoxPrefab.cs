using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class FireBoxPrefab : MonoBehaviour
{
    [SerializeField] SpriteRenderer goSprite;
    [SerializeField] ParticleSystem fireParticle;
    [SerializeField] BoxCollider2D boxCollider2D;
    [SerializeField] Rigidbody2D _rigidbody2D;
    [SerializeField] GameObject visualGo;
    GridManager gridManager;
    int posX;
    int posY;
    bool fire;
    bool isClickable;
    bool spawned;
    public List<GameObject> hitBoxs = new List<GameObject>();
    
    void Awake()
    {
        EventManager.gameOverEvent += GameOver;
        EventManager.updatePosition += UpdatePos;
        EventManager.shakeBoxs += ShakeBoxsAnimation;

        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        isClickable = false;
    }

    void OnEnable()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Static;
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
        isClickable = true;
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
                    hitBoxs.Clear();
                    FindCell();
                    spawned = true;
                    break;
                }
            }
        }
    }

    void OnMouseDown()
    {
        if (spawned)
        {
            isClickable = true;
        }
        if (isClickable)
        {
            AudioManager.instance.PlayAudioClip(1);
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            boxCollider2D.isTrigger = true;
            fire = true;
            goSprite.enabled = false;
            fireParticle.Play();
            BurnBoxs();
            StartCoroutine(KillMyself());
        }
    }

    void OnDisable()
    {
        boxCollider2D.isTrigger = false;
        this.gameObject.transform.DOKill();
        spawned = false;
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
        for (int x = 0; x < gridManager.maxgridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                if (gridManager.gridArray[x, y] == this.gameObject)
                {
                    posY = y;
                    posX = x;
                    spawned = true;
                    ActivateBox();
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

    IEnumerator KillMyself()
    {
        yield return new WaitForSeconds(30f);
        gridManager.RemoveBoxs(this.gameObject);
    }

    void GameOver()
    {
        this.gameObject.transform.DOKill();
    }
}
