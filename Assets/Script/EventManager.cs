using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static event Action gameOverEvent;
    public static event Action<bool> resetEvent;
    public static event Action boxsFly;

    public static void GameOverEvent()
    {
        gameOverEvent?.Invoke();
    }

    public static void ResetEvent(bool retry)
    {
        resetEvent?.Invoke(retry);
    }

    public static void BoxsFlyEvent()
    {
        boxsFly?.Invoke();
    }
}