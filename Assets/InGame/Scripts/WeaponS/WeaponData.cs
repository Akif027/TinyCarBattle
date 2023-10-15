
using System;

using UnityEngine;

[Serializable]
public class WeaponData 
{
    public string Name;

    public GameObject FirePoint;
    public GameObject AmmoPrefab;
    public GameObject WeaponPrefab;


    public float nextFireTime = 0f;

    public float fireRate = 0.5f;
   

}
