using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimerScript : MonoBehaviour
{
    [SerializeField] UIManager uIManager;
    [SerializeField] GameManager gameManager;
    [SerializeField] Canvas canvas;
    VisualElement root;
    VisualElement pauseBlackScreen;
    Label timerLabel;
    Label timerTimeAdd;
    Label crusherTimerLabel;
    Label fireTimerLabel;
    Label bombTimerLabel;
    Button pauseButton;
    Button crusherButton;
    Button FireButton;
    Button BombButton;
    public float timeLeft;
    bool timerOn;
    bool crusherTimer;
    public float crusherTime;
    bool fireTimer;
    public float fireTime;
    bool bombTimer;
    public float bombTime;

    // Start is called before the first frame update
    void Start()
    {
        uIManager = GetComponent<UIManager>();
        root = GetComponent<UIDocument>().rootVisualElement; 
        pauseBlackScreen = root.Q<VisualElement>("PauseBlackScreen");
        timerLabel = root.Q<Label>("TimerLabel");
        timerTimeAdd = root.Q<Label>("TimerTimeAdd");
        crusherTimerLabel = root.Q<Label>("CrusherTimer");
        fireTimerLabel = root.Q<Label>("FireTimer");
        bombTimerLabel = root.Q<Label>("BombTimer");
        pauseButton = root.Q<Button>("PauseButton");
        crusherButton = root.Q<Button>("CrusherButton");
        FireButton = root.Q<Button>("FireButton");
        BombButton = root.Q<Button>("BombButton");

        timerOn = true;

        pauseBlackScreen.pickingMode = PickingMode.Ignore;
        crusherTimerLabel.text = "";
        fireTimerLabel.text = "";
        bombTimerLabel.text = "";

        pauseButton.RegisterCallback<ClickEvent>(evt => {
            if(timerOn == true)
            {
                timerOn = false;
                Time.timeScale = 0;
                pauseBlackScreen.pickingMode = PickingMode.Position;
                timerLabel.AddToClassList("TimerWhite");
                pauseBlackScreen.RemoveFromClassList("PauseBlackScreenHidden");
                canvas.enabled = false;
            }
            else
            {
                timerOn = true;
                Time.timeScale = 1;
                pauseBlackScreen.pickingMode = PickingMode.Ignore;
                timerLabel.RemoveFromClassList("TimerWhite");
                pauseBlackScreen.AddToClassList("PauseBlackScreenHidden");
                canvas.enabled = true;
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

        if (crusherTimer)
        {
            if (crusherTime > 0)
            {
                crusherTime -= Time.deltaTime;
                UpdateCrusherTimer(crusherTime);
            }
            else 
            {
                crusherTimer = false;

                crusherButton.pickingMode = PickingMode.Position;
                crusherButton.style.unityBackgroundImageTintColor = Color.white;
                crusherTimerLabel.text = "";
            }
        }
        if (fireTimer)
        {
            if (fireTime > 0)
            {
                fireTime -= Time.deltaTime;
                UpdateFireTimer(fireTime);
            }
            else 
            {
                fireTimer = false;

                FireButton.pickingMode = PickingMode.Position;
                FireButton.style.unityBackgroundImageTintColor = Color.white;
                fireTimerLabel.text = "";
            }
        }
        if (bombTimer)
        {
            if (bombTime > 0)
            {
                bombTime -= Time.deltaTime;
                UpdateBombTimer(crusherTime);
            }
            else 
            {
                bombTimer = false;

                BombButton.pickingMode = PickingMode.Position;
                BombButton.style.unityBackgroundImageTintColor = Color.white;
                bombTimerLabel.text = "";
            }
        }
    }

    public IEnumerator AddTimerTime(float timeAdded)
    {
        timerTimeAdd.text = "+ 0:0" + timeAdded;
        timerTimeAdd.AddToClassList("TimerTimeAddEnd");
        yield return new WaitForSeconds(0.5f);
        timeLeft += timeAdded;
        yield return new WaitForSeconds(0.5f);
        timerTimeAdd.text = "";
        timerTimeAdd.RemoveFromClassList("TimerTimeAddEnd");
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

    public void SetupPowerupTimer(int powerUp)
    {
        switch (powerUp)
        {
            case 0:
                crusherTimer = true;
                crusherTime = 60f;
            break;

            case 1:
                fireTimer = true;
                fireTime = 60f;
            break;

            case 2:
                bombTimer = true;
                bombTime = 60f;
            break;
        }
        
        gameManager.CountPowerupUse(powerUp);
    }

    void UpdateCrusherTimer(float timerTime)
    {
        timerTime += 1;

        float secondes = Mathf.FloorToInt(timerTime % 60);

        crusherTimerLabel.text = string.Format("{00}", secondes);
    }

    void UpdateFireTimer(float timerTime)
    {
        timerTime += 1;

        float secondes = Mathf.FloorToInt(timerTime % 60);

        fireTimerLabel.text = string.Format("{00}", secondes);
    }

    void UpdateBombTimer(float timerTime)
    {
        timerTime += 1;

        float secondes = Mathf.FloorToInt(timerTime % 60);

        bombTimerLabel.text = string.Format("{00}", secondes);
    }
}