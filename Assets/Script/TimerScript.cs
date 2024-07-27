using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimerScript : MonoBehaviour
{
    [SerializeField] UIManager uIManager;
    [SerializeField] Canvas canvas;
    VisualElement root;
    VisualElement pauseBlackScreen;
    VisualElement pauseVisualPaused;
    VisualElement pauseVisualPlay;
    Label timerLabel;
    Label timerTimeAdd;
    Label timeLabel;
    Label swapLettersTimer;
    Label crusherTimerLabel;
    Label hintTimerLabel;
    Label fireTimerLabel;
    Label bombTimerLabel;
    Button pauseButton;
    Button crusherButton;
    Button FireButton;
    Button BombButton;
    Button swapButton;
    Button hintButton;
    public float timeLeft;
    bool timerOn;
    bool crusherTimer;
    public float crusherTime;
    bool fireTimer;
    public float fireTime;
    bool bombTimer;
    public float bombTime;
    bool swapTimer;
    public float swapTime;
    bool hintTimer;
    public float hintTime;
    float timePlaying = 0;
    bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.gameOverEvent += GameOver;
        uIManager = GetComponent<UIManager>();
        root = GetComponent<UIDocument>().rootVisualElement; 
        swapLettersTimer = root.Q<Label>("SwapLettersTimer");
        pauseBlackScreen = root.Q<VisualElement>("PauseBlackScreen");
        pauseVisualPlay = root.Q<VisualElement>("PauseVisualPlay");
        pauseVisualPaused = root.Q<VisualElement>("PauseVisualPaused");
        timerLabel = root.Q<Label>("TimerLabel");
        timeLabel = root.Q<Label>("TimeLabel");
        hintTimerLabel = root.Q<Label>("HintTimerLabel");
        timerTimeAdd = root.Q<Label>("TimerTimeAdd");
        crusherTimerLabel = root.Q<Label>("CrusherTimer");
        fireTimerLabel = root.Q<Label>("FireTimer");
        bombTimerLabel = root.Q<Label>("BombTimer");
        pauseButton = root.Q<Button>("PauseButton");
        crusherButton = root.Q<Button>("CrusherButton");
        FireButton = root.Q<Button>("FireButton");
        BombButton = root.Q<Button>("BombButton");
        swapButton = root.Q<Button>("SwapLetters");
        hintButton = root.Q<Button>("HintButton");

        timerOn = true;

        pauseBlackScreen.pickingMode = PickingMode.Ignore;

        if (crusherTimerLabel != null)
        {
            crusherTimerLabel.text = "";
            fireTimerLabel.text = "";
            bombTimerLabel.text = "";
            swapLettersTimer.text = "";
        }

        pauseButton.RegisterCallback<ClickEvent>(evt => {
            if(timerOn == true)
            {
                pauseVisualPlay.style.opacity = 0;
                pauseVisualPaused.style.opacity = 100;
                AudioManager.instance.PauseMusic();
                EventManager.ButtonClicked(0);
                timerOn = false;
                Time.timeScale = 0;
                pauseBlackScreen.pickingMode = PickingMode.Position;
                timerLabel.AddToClassList("TimerWhite");
                pauseBlackScreen.RemoveFromClassList("PauseBlackScreenHidden");
                canvas.enabled = false;
            }
            else
            {
                pauseVisualPlay.style.opacity = 100;
                pauseVisualPaused.style.opacity = 0;
                AudioManager.instance.PauseMusic();
                EventManager.ButtonClicked(0);
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
                timePlaying += Time.deltaTime;
                UpdateTimer(timeLeft);
                UpdateFinalTime(timePlaying);
            }
            else 
            {
                StopAllCoroutines();
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
                UpdateBombTimer(bombTime);
            }
            else 
            {
                bombTimer = false;

                BombButton.pickingMode = PickingMode.Position;
                BombButton.style.unityBackgroundImageTintColor = Color.white;
                bombTimerLabel.text = "";
            }
        }
        if (swapTimer)
        {
            if (swapTime > 0)
            {
                swapTime -= Time.deltaTime;
                UpdateSwapTimer(swapTime);
            }
            else 
            {
                swapTimer = false;

                swapButton.pickingMode = PickingMode.Position;
                swapButton.style.unityBackgroundImageTintColor = Color.white;
                swapLettersTimer.text = "";
            }
        }
        if (hintTimer)
        {
            if (hintTime > 0)
            {
                hintTime -= Time.deltaTime;
                UpdateHintTimer(hintTime);
            }
            else 
            {
                hintTimer = false;

                hintButton.pickingMode = PickingMode.Position;
                hintButton.style.unityBackgroundImageTintColor = Color.white;
                hintButton.text = "";
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

            case 3:
                swapTimer = true;
                swapTime = 60f;
            break;

            case 4:
                hintTimer = true;
                hintTime = 60f;
            break;
        }
        GameManager.instance.CountPowerupUse(powerUp);
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
    void UpdateSwapTimer(float timerTime)
    {
        timerTime += 1;

        float secondes = Mathf.FloorToInt(timerTime % 60);

        swapLettersTimer.text = string.Format("{00}", secondes);
    }

    void UpdateHintTimer(float timerTime)
    {
        timerTime += 1;

        float secondes = Mathf.FloorToInt(timerTime % 60);

        hintTimerLabel.text = string.Format("{00}", secondes);
    }

    void UpdateFinalTime(float timerTime)
    {
        timerTime += 1;

        float minutes = Mathf.FloorToInt(timerTime / 60);
        float secondes = Mathf.FloorToInt(timerTime % 60);

        timeLabel.text = string.Format("{0:00}:{1:00}", minutes, secondes);
    }
    
    void GameOver()
    {
        gameOver = true;
        timerOn = false;
        crusherTimer = false;
        fireTimer = false;
        bombTimer = false;
    }

    public void ResetTimers(int scene)
    {
        if (gameOver && scene == 1)
        {
            timerOn = true;

            timePlaying = 0;

            BombButton.pickingMode = PickingMode.Position;
            BombButton.style.unityBackgroundImageTintColor = Color.white;
            bombTimerLabel.text = "";

            FireButton.pickingMode = PickingMode.Position;
            FireButton.style.unityBackgroundImageTintColor = Color.white;
            fireTimerLabel.text = "";

            crusherButton.pickingMode = PickingMode.Position;
            crusherButton.style.unityBackgroundImageTintColor = Color.white;
            crusherTimerLabel.text = "";
        }
        else if (gameOver && scene == 2)
        {
            timerOn = true;

            timePlaying = 0;

            hintButton.pickingMode = PickingMode.Position;
            hintButton.style.unityBackgroundImageTintColor = Color.white;
            hintTimerLabel.text = "";
        }
    }
}