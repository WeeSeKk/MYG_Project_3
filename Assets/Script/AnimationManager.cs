using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BoxsFly(GameObject gameObject)
    {
        System.Random random = new System.Random();
        int i = random.Next(1, 3);

        Vector2 jumpEnd = new Vector2(i, 10);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -1);
        gameObject.transform.DOMoveY(gameObject.transform.position.y + 10f, 1, false).SetEase(Ease.InBack);
    }

    public void CrushBoxs()
    {
        //do stuff
    }
}
