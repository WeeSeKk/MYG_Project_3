using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GridManager gridManager;
    [SerializeField] WordsManager wordsManager;
    [SerializeField] UIManager uIManager;
    [SerializeField] TimerScript timerScript;
    int currentGamemode;


    // Start is called before the first frame update
    void Start()
    {
        EventManager.gameOverEvent += GameOver;
        EventManager.resetEvent += ResetCurrentGamemode;

        currentGamemode = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))//test for debug
        {
            ResetCurrentGamemode(false);
        }
    }

    void SetupGameMode(int gamemode)
    {
        //do different parameters for different gamemodes 
        //gamemode 1
        //gamemode 2
        //etc ..

        if(gamemode == 1)
        {
            //gamemode parameters

            gridManager.StartSpawningBoxes();
            timerScript.SetupTimer(6f);

        }
    }

    void ResetCurrentGamemode(bool retry)
    {
        //reset all parameters for all gamemodes

        gridManager.ResetGridAndArray();
        timerScript.ResetTimer();

        if(retry == true)
        {
           SetupGameMode(currentGamemode); 
        }
    }

    void GameOver()
    {
        Debug.Log("THE GAME IS OVER");
    }
}