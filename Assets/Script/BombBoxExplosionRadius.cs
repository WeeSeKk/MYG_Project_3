using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBoxExplosionRadius : MonoBehaviour
{
    [SerializeField] BombBoxPrefab bombBoxPrefab;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != bombBoxPrefab.gameObject)
        {
            bombBoxPrefab.AddGoToList(other.gameObject);
        }
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject != bombBoxPrefab.gameObject)
        {
            bombBoxPrefab.AddGoToList(other.gameObject);
        }
    }
}
