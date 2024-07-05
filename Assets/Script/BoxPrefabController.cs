using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class BoxPrefabController : MonoBehaviour
{
    GridManager gridManager;
    WordsManager wordsManager;
    public GameObject child;
    TMP_Text text;
    char letter;
    int posX;
    int posY;
    bool moved;

    // Start is called before the first frame update
    void Start()
    {
        FindCell();
    }
    
    void Awake()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        wordsManager = GameObject.Find("WordsManager").GetComponent<WordsManager>();

        text = child.GetComponent<TMP_Text>();
        ChooseLetter();
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

    void ChooseLetter()
    {
        //choose a random letter to assign to this box
        string chars = "abcdefghijklmnopqrstuvwxyz"; 

        System.Random rand = new System.Random();
        int num = rand.Next(0, chars.Length);
        letter = chars[num];

        text.SetText(letter.ToString());
    }

    void OnMouseDown()
    {
        wordsManager.AddLetter(letter);
        gridManager.boxs.Add(this.gameObject);//add this box to the list of boxs used to create a word
        Debug.Log(posY);
    }

    public void FindCell()
    {
        //if a cell have a empty cell under it then go down by one cell and continue until there is a cell or a max vector
        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                if (gridManager.gridArray[x, y] == this.gameObject)
                {
                    posY = y;
                    StartCoroutine(MoveCell(x, y));
                    break;
                }
            }
        }
    }

    IEnumerator MoveCell(int x, int y)
    {
        while (y > 0 && gridManager.gridArray[x, y - 1] == null)
        {
            yield return new WaitForSeconds(1f);
            gridManager.UpdateArray(this.gameObject, x, y);
            Vector3 newWorldPosition = gridManager.grid.CellToWorld(new Vector3Int(x, y - 1, 0));
            this.gameObject.transform.position = newWorldPosition;
            y -= 1;
            posX = x;
            posY = y;
        }
        moved = true;
    }
}