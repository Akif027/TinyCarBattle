
using System;

using UnityEngine;

[Serializable]
public class WeaponData 
{
    public string Name;

    public GameObject FirePoint;
    public GameObject AmmoPrefab;
    public GameObject WeaponPrefab;
    public float WeaponTime = 10;
    public float speedofBullet = 1.2f;
    public ParticleSystem MuzzleFlash =null;
    [HideInInspector]
    public float nextFireTime = 0f;

    public float fireRate = 0.5f;
   

}
