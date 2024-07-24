using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] GridManager gridManager;
    
    public void BoxsFly(GameObject gameObject)
    {
        System.Random random = new System.Random();
        int i = random.Next(1, 3);

        Vector2 jumpEnd = new Vector2(i, 10);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -1);
        gameObject.transform.DOMoveY(gameObject.transform.position.y + 10f, 1, false).SetEase(Ease.InBack);
    }
}