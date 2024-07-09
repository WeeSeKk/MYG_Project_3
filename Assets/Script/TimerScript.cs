using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimerScript : MonoBehaviour
{
    [SerializeField] UIManager uIManager;
    VisualElement root;
    VisualElement pauseBlackScreen;
    Label timerLabel;
    Button pauseButton;
    public float timeLeft;
    bool timerOn;

    // Start is called before the first frame update
    void Start()
    {
        uIManager = GetComponent<UIManager>();
        root = GetComponent<UIDocument>().rootVisualElement; 
        pauseBlackScreen = root.Q<VisualElement>("PauseBlackScreen");
        timerLabel = root.Q<Label>("TimerLabel");
        pauseButton = root.Q<Button>("PauseButton");

        timerOn = true;

        pauseBlackScreen.pickingMode = PickingMode.Ignore;

        pauseButton.RegisterCallback<ClickEvent>(evt => {
            if(timerOn == true)
            {
                timerOn = false;
                Time.timeScale = 0;
                pauseBlackScreen.pickingMode = PickingMode.Position;
                timerLabel.AddToClassList("TimerWhite");
                pauseBlackScreen.AddToClassList("PauseBlackScreenVisible");
            }
            else
            {
                timerOn = true;
                Time.timeScale = 1;
                pauseBlackScreen.pickingMode = PickingMode.Ignore;
                timerLabel.RemoveFromClassList("TimerWhite");
                pauseBlackScreen.RemoveFromClassList("PauseBlackScreenVisible");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (timerOn)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                UpdateTimer(timeLeft);
            }
            else 
            {
                EventManager.GameOverEvent();
                ResetTimer();
                timerOn = false;
            }
        } 
    }

    void UpdateTimer(float timerTime)
    {
        timerTime += 1;

        float minutes = Mathf.FloorToInt(timerTime / 60);
        float secondes = Mathf.FloorToInt(timerTime % 60);

        timerLabel.text = string.Format("{0:00}:{1:00}", minutes, secondes);
    }

    public void ResetTimer()
    {
        timerOn = false;
        UpdateTimer(-1);
    }

    public void SetupTimer(float time)
    {
        timerOn = true;
        timeLeft = time;
    }
}