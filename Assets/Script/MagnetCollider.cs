using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetCollider : MonoBehaviour
{
    [SerializeField] MagnetBoxPrefab magnetBoxPrefab;

    void OnTriggerStay2D(Collider2D other)
    {
        magnetBoxPrefab.LockBoxPosition(other.gameObject);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        magnetBoxPrefab.FindLockedGo();
    }
}