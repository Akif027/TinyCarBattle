using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPool : MonoBehaviourPun
{
    public static ObjectPool Instance;
    public GameObject BulletPrefab;
    public GameObject MissilePrefab;

    [SerializeField] int bulletAmount;
    [SerializeField] int MissileAmount;

    // Define a dictionary to store pools for different object types
    private Dictionary<GameObject, List<GameObject>> pooledObjectsDictionary = new Dictionary<GameObject, List<GameObject>>();

    private void Awake()
    {
        Instance = this;
        CreatePool(BulletPrefab, bulletAmount);
        CreatePool(MissilePrefab, MissileAmount);
    }

    // Create a pool for the specified object type
    public void CreatePool(GameObject objectToPool, int amountToPool)
    {
        if (!pooledObjectsDictionary.ContainsKey(objectToPool))
        {
            List<GameObject> pooledObjectsList = new List<GameObject>();

            for (int i = 0; i < amountToPool; i++)
            {
                GameObject temp = Instantiate(objectToPool, transform);
            
                temp.SetActive(false);
                pooledObjectsList.Add(temp);
            }

            pooledObjectsDictionary.Add(objectToPool, pooledObjectsList);
        }
    }

    // Get an object from the specified pool
    public GameObject GetPooledObject(GameObject objectToGet)
    {
        if (pooledObjectsDictionary.ContainsKey(objectToGet))
        {
            foreach (GameObject obj in pooledObjectsDictionary[objectToGet])
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }
        }

        return null;
    }
}
