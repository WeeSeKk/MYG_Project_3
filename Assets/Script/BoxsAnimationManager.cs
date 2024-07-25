using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using DG.Tweening;

public class BoxsAnimationManager : MonoBehaviour
{
    public List<GameObject> boxsPrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChooseBoxToMoove());
    }

    IEnumerator ChooseBoxToMoove()
    {
        while(this.gameObject.activeSelf == true)
        {
            System.Random rand = new System.Random();
            int num = rand.Next(0, 9);

        
            MooveBoxs(boxsPrefab[num]);

            yield return new WaitForSeconds(1f);
        }
    }

    void MooveBoxs(GameObject gameObject)
    {
        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }

        Vector3 initPos = gameObject.transform.position;
        Vector3 finPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y -20, gameObject.transform.position.z);

        gameObject.transform.DOMove(finPos, 3f).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            gameObject.transform.position = initPos;
            gameObject.SetActive(false);
        });
    }
}
