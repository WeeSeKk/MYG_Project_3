using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;
    public static event Action gameOverEvent;
    public static event Action shakeBoxs;
    public static event Action<float> musicVolulmeChange;
    public static event Action<float> sfxVolulmeChange;
    public static event Action<int> buttonClicked;
    public static event Action swapLetters;
    public static event Action<bool> resetEvent;
    public static event Action<GameObject, int, int>updatePosition;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public static void GameOverEvent()
    {
        gameOverEvent?.Invoke();
    }

    public static void ResetEvent(bool retry)
    {
        resetEvent?.Invoke(retry);
    }

    public static void ShakeBoxs()
    {
        shakeBoxs?.Invoke();
    }

    public static void SwapLetters()
    {
        swapLetters?.Invoke();
    }

    public static void UpdatePosition(GameObject go, int x, int y)
    {
        updatePosition?.Invoke(go, x, y);
    }
    public static void ButtonClicked(int num)
    {
        buttonClicked?.Invoke(0);
    }
    public static void MusicVolumeChange(float value)
    {
        musicVolulmeChange?.Invoke(value);
    }
    public static void SFXVolumeChange(float value)
    {
        sfxVolulmeChange?.Invoke(value);
    }
}