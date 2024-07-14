using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrusherBlockVFX : MonoBehaviour
{
    [SerializeField] ParticleSystem particle_System;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        particle_System.Play();
    }
}
