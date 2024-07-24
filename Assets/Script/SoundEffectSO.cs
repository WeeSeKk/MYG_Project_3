using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundEffect")]

public class SoundEffectSO : ScriptableObject
{
    public AudioClip[] clips;
    public Vector2 volume = new Vector2(0.5f, 0.5f);
    public Vector2 pitch = new Vector2(0.5f, 0.5f);
}