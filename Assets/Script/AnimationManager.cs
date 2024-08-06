using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using TMPro;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] GridManager gridManager;
    
    public void BoxsFly(GameObject gameObject)
    {
        SpriteRenderer childSpriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        childSpriteRenderer.sortingOrder = 20;

        TMP_Text text = gameObject.GetComponentInChildren<TMP_Text>();

        MeshRenderer meshRenderer = text.GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = 100;



        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -1);
        gameObject.transform.DOMoveY(gameObject.transform.position.y + 10f, 1, false).SetEase(Ease.InBack);
    }
}