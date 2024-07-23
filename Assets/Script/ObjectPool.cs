using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();

    public static GameObject BoxSpawn(GameObject gameObject, Vector3 spawnPos, Quaternion spawnRot)
    {
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookUpString == gameObject.name);

        if(pool == null)
        {
            pool = new PooledObjectInfo() {LookUpString = gameObject.name};
            ObjectPools.Add(pool);
        }

        GameObject spawnAbleObject = null;
        

        foreach (GameObject obj in pool.InactiveObjects)//look for inactive object in the pool 
        {
            if (obj != null)
            {
                spawnAbleObject = obj;
                break;
            }
        }

        if (spawnAbleObject == null)//if there is no inactive object then create one
        {
            spawnAbleObject = Instantiate(gameObject, spawnPos, spawnRot);
        }
        else//if there is an inactive object then reactive it
        {
            spawnAbleObject.transform.position = spawnPos;
            spawnAbleObject.transform.rotation = spawnRot;
            pool.InactiveObjects.Remove(spawnAbleObject);
            spawnAbleObject.SetActive(true);
        }
        return spawnAbleObject;
    }

    public static void ReturnObjectToPool(GameObject gameObject)
    {
        string goName = gameObject.name.Substring(0, gameObject.name.Length - 7);//removing the "(Clone)" frome the new instantiate gameobject

        PooledObjectInfo pool = ObjectPools.Find(p => p.LookUpString == goName);

        if(pool == null)
        {
            Debug.LogWarning("ERROR");
        }
        else
        {
            gameObject.SetActive(false);
            pool.InactiveObjects.Add(gameObject);
        }
    }
}

public class PooledObjectInfo
{
    public string LookUpString;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}
/*
    ObjectPool<GameObject> boxs;

    void Awake()
    {
        CreateBoxObjectPool();
    }

    public void CreateBoxObjectPool() {
        boxs = new ObjectPool<GameObject>(() => {
            return CreateBox();                     //Creation Function
        }, boxs => {
            OnGet(boxs);                            //On Get
        }, boxs => {
            boxs.SetActive(false);                  //On Release
        }, boxs => {
            Destroy(boxs);                          //On Destroy
        }, false,                                   //Check Collection
        20,                                         //Initial Array Size (to avoid recreations)
        80                                          //Max Array Size
        );
    }

    GameObject CreateBox()
    {
        return Instantiate(GameManager.instance.GenerateBox());
    }

    public GameObject GetBox()
    {
        return boxs.Get();
    }

    void OnGet(GameObject box)
    {
        box.SetActive(true);
    }

    public void ReleaseBox(GameObject box)
    {
        boxs.Release(box);
    }
 */   