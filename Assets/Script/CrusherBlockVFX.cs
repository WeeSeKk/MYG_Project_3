using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrusherBlockVFX : MonoBehaviour
{
    [SerializeField] ParticleSystem particle_System;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("ouch!");
        particle_System.Play();
    }
}
