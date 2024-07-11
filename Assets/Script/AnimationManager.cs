using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] GridManager gridManager;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.shakeBoxs += ShakeBoxsAnimation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BoxsFly(GameObject gameObject)
    {
        System.Random random = new System.Random();
        int i = random.Next(1, 3);

        Vector2 jumpEnd = new Vector2(i, 10);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -1);
        gameObject.transform.DOMoveY(gameObject.transform.position.y + 10f, 1, false).SetEase(Ease.InBack);
    }

    void ShakeBoxsAnimation()
    {
        /*
        List<GameObject> boxsToShake = new List<GameObject>();

        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                if (gridManager.gridArray[x, y] != null)
                {
                    boxsToShake.Add(gridManager.gridArray[x, y]);
                } 
            }
        }

        foreach(GameObject go in boxsToShake)
        {
            if(go.name != "bigBoxSquare(Clone)")
            {
                Vector3Int cellPosition = gridManager.grid.WorldToCell(go.transform.position);
                Vector3 originalPosition = gridManager.grid.CellToWorld(cellPosition);


                go.transform.DOShakePosition(1f, 0.1f, 10, 60f, false, true).OnComplete(() =>
                {
                    if(go != null)
                    {
                        go.transform.position = originalPosition;
                    }
                });
            }
        }
        boxsToShake.Clear();
        */
    }

    public void CrushBoxs()
    {
        //do stuff
    }
}