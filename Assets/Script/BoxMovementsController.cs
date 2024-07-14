using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoxMovementsController : MonoBehaviour
{
    [SerializeField] GridManager gridManager;
    int posX;
    int posY;

    void Update()
    {

    }
    
    void LookForMoveableBlock()
    {
        for (int x = 0; x <= gridManager.maxgridWidth; x++)//look for x pos 
        {
            for (int y = 0; y <= gridManager.cellMaxHeight; y++)//look for y pos 
            {
                if (gridManager.gridArray[x, y] != null)//if boxs found call NewMoveCell
                {
                    Debug.Log("test");
                    NewMoveCell(gridManager.gridArray[x, y], x, y);
                }
            }
        }
    }

    void NewMoveCell(GameObject gameObject, int x, int y)
    {
        Vector3 newWorldPosition;

        for (int i = 0; i < gridManager.gridHeight; i++)
        {
            if((i != 0 && gridManager.gridArray[x, i] == null && gridManager.gridArray[x, i - 1] != null) || (i == 0 && gridManager.gridArray[x, i] == null))
            {
                newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(x, i));
                gameObject.transform.DOMove(newWorldPosition, 3f, false).SetEase(Ease.OutCirc);
                gridManager.UpdateArray(gameObject, x, i);
                posY = i;
                posX = x;
                break;
            }  
        }
    }

    
}
