using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BoxPrefabController : MonoBehaviour
{
    GridManager gridManager;
    public GameObject child;
    TMP_Text text;
    char letter;

    // Start is called before the first frame update
    void Awake()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();

        text = child.GetComponent<TMP_Text>();
        ChooseLetter();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("t"))
        {
            CellMovement();
        }
    }

    void ChooseLetter()
    {
        //choose a random letter to assign to this box

        string chars = "abcdefghijklmnopqrstuvwxyz";

        System.Random rand = new System.Random();
        
        int num = rand.Next(0, chars.Length);

        letter = chars[num];

        text.SetText(letter.ToString());

        Debug.Log(letter);
    }

    void OnMouseDown()
    {
        Debug.Log("THIS IS A TEST" + " " + letter);
    }

    void CellMovement()
    {
        //if a cell have a empty cell under it then go down by one cell and continue until there is a cell or a max vector
        
        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                if (gridManager.gridArray[x, y] == this.gameObject && y > 0 && gridManager.gridArray[x, y - 1] == null)
                {
                    gridManager.UpdateArray(this.gameObject, x, y);
                    Vector3 newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(x, y - 1, 0));
                    this.gameObject.transform.position = newWorldPosition;
                    Debug.Log("I HAVE FOUND MYSELF " + x + " " + y);
                }
            }
        }
    }
}