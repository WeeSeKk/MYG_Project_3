using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static event Action gameOverEvent;
    public static event Action shakeBoxs;
    public static event Action swapLetters;
    public static event Action<bool> resetEvent;
    public static event Action<GameObject, int, int>updatePosition;

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
}