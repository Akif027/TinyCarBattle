using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Tarodev;
using UnityEngine;

public class WeaponSystem : MonoBehaviourPun
{
  
    public WeaponType Type;

    PhotonView view;

    [Header("HomingMissle")]
    public float detectionRadius = 10f;
    public LayerMask enemyLayer;
   

    public WeaponData[] weaponData = new WeaponData[3];

    private void Start()
    {
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            Type = WeaponType.SimpleGun;

            enemyLayer = LayerMask.GetMask("Enemy");

            for (int i = 1; i < weaponData.Length; i++) //disable the weapon at start execpt the simple gun 
            {
                weaponData[i].WeaponPrefab.SetActive(false);
            }

        }


        int targetLayer = view.IsMine?  LayerMask.NameToLayer("Player"): LayerMask.NameToLayer("Enemy");

            // Check if the layer exists before setting it
            if (targetLayer != -1)
            {
                // Set the layer of the GameObject
                gameObject.layer = targetLayer;
            }
            else
            {
                Debug.LogError("Layer " + "Player" + " does not exist.");
            }

    }

    private void Update()
    {
        if (view.IsMine)
        {
         
            ChangeWeaponOnType();
        }

    }


    private void ChangeWeaponOnType()
    {

        switch (Type)
        {
            case WeaponType.SimpleGun:
                view.RPC("SimpleGunWeapon", RpcTarget.AllBuffered);
                break;
            case WeaponType.miniGun :
                view.RPC("MiniGunWeapon", RpcTarget.AllBuffered);
                break;
            case WeaponType.DoubleminiGun:
                view.RPC("MiniGunWeapon", RpcTarget.AllBuffered);
                break;
            case WeaponType.Rocketlauncher:
                view.RPC("RocketLauncherWeapon", RpcTarget.AllBuffered);
                break;
        }
    }

    [PunRPC]
    private void SimpleGunWeapon()
    {
     
      
       
        // Vector3 Temp = new Vector3(target.transform.position.x, 5, target.transform.position.z);
        if (Input.GetMouseButtonDown(0) && Time.time >= weaponData[0].nextFireTime)
        {
            Debug.Log("gun firing");
        
            weaponData[0].nextFireTime = Time.time + weaponData[0].fireRate;

            GameObject projectile = ObjectPool.Instance.GetPooledObject(weaponData[0].AmmoPrefab);
            float currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
            if (projectile != null)
            {

                projectile.transform.position = weaponData[0].FirePoint.transform.position;
                projectile.transform.rotation = weaponData[0].FirePoint.transform.rotation;
                float playerSpeedThreshold = 1.0f; // You can adjust this threshold as needed
                float bulletBaseSpeed = 15; // The base speed of the bullet
                float currentMoveSpeed = currentSpeed * 1.2f;
                // Calculate the bullet speed based on the player's speed
                float adjustedBulletSpeed = bulletBaseSpeed + Mathf.Max(currentMoveSpeed - playerSpeedThreshold, 0);

                // Apply the adjusted speed to the bullet's velocity
                projectile.GetComponent<Rigidbody>().velocity = weaponData[0].FirePoint.transform.forward * adjustedBulletSpeed;
            }
        }
      

    }

    [PunRPC]
    private void MiniGunWeapon()
    {

        if (Type == WeaponType.miniGun)
        {
           
                weaponData[1].WeaponPrefab.SetActive(true);
            weaponData[2].WeaponPrefab.SetActive(false);

            // Vector3 Temp = new Vector3(target.transform.position.x, 5, target.transform.position.z);
            if (Input.GetMouseButton(0) && Time.time >= weaponData[1].nextFireTime)
            {
                Debug.Log(" mini gun firing");

                weaponData[1].nextFireTime = Time.time + weaponData[1].fireRate;

                GameObject projectile = ObjectPool.Instance.GetPooledObject(weaponData[1].AmmoPrefab);
                float currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
                if (projectile != null)
                {

                    projectile.transform.position = weaponData[1].FirePoint.transform.position;
                    projectile.transform.rotation = weaponData[1].FirePoint.transform.rotation;
                    float playerSpeedThreshold = 1.0f; // You can adjust this threshold as needed
                    float bulletBaseSpeed = 15; // The base speed of the bullet
                    float currentMoveSpeed = currentSpeed * 1.2f;
                    // Calculate the bullet speed based on the player's speed
                    float adjustedBulletSpeed = bulletBaseSpeed + Mathf.Max(currentMoveSpeed - playerSpeedThreshold, 0);

                    // Apply the adjusted speed to the bullet's velocity
                    projectile.GetComponent<Rigidbody>().velocity = weaponData[1].FirePoint.transform.forward * adjustedBulletSpeed;
                }



            }

        }
        else if(Type == WeaponType.DoubleminiGun)
        {
            weaponData[1].WeaponPrefab.SetActive(true);
            weaponData[2].WeaponPrefab.SetActive(true);
            // Vector3 Temp = new Vector3(target.transform.position.x, 5, target.transform.position.z);
            if (Input.GetMouseButton(0) && Time.time >= weaponData[1].nextFireTime)
            {
                Debug.Log("Double mini gun firing");

                weaponData[1].nextFireTime = Time.time + weaponData[1].fireRate;
                for (int i = 1; i < 3; i++)  //doubleMiniGun element which is 2 and 3 so need to loop through it. Note* dont change the element sequence otherwise u need to rearrange it
                {
                    GameObject projectile = ObjectPool.Instance.GetPooledObject(weaponData[1].AmmoPrefab);
                    float currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
                    if (projectile != null)
                    {

                        projectile.transform.position = weaponData[i].FirePoint.transform.position;
                        projectile.transform.rotation = weaponData[i].FirePoint.transform.rotation;
                        float playerSpeedThreshold = 1.0f; // You can adjust this threshold as needed
                        float bulletBaseSpeed = 15; // The base speed of the bullet
                        float currentMoveSpeed = currentSpeed * 1.2f;
                        // Calculate the bullet speed based on the player's speed
                        float adjustedBulletSpeed = bulletBaseSpeed + Mathf.Max(currentMoveSpeed - playerSpeedThreshold, 0);

                        // Apply the adjusted speed to the bullet's velocity
                        projectile.GetComponent<Rigidbody>().velocity = weaponData[i].FirePoint.transform.forward * adjustedBulletSpeed;
                    }

                }

            }
        }


    }


    [PunRPC]
    private void RocketLauncherWeapon()
    {
        weaponData[3].WeaponPrefab.SetActive(true);
      
        if (Input.GetMouseButtonDown(0)  && Time.time >= weaponData[3].nextFireTime )
        {
            GameObject Target = FindTarget();
            Debug.Log("Rocket firing");

            weaponData[3].nextFireTime = Time.time + weaponData[3].fireRate;

            GameObject projectile = ObjectPool.Instance.GetPooledObject(weaponData[3].AmmoPrefab);
             Missile missile = projectile.GetComponent<Missile>();
            float currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
            if (projectile != null)
            {
                // logic for getting the target 
                if (Target != null)
                {
                   
                    missile.enabled = true;
                    missile._target = Target.GetComponent<Target>();
                }
                else
                {
                    Debug.Log("No Target Found");
                }
                float playerSpeedThreshold = 1.0f; // You can adjust this threshold as needed
                float bulletBaseSpeed = 15; // The base speed of the bullet
                float currentMoveSpeed = currentSpeed * 1.2f;
                // Calculate the bullet speed based on the player's speed
                float adjustedBulletSpeed = bulletBaseSpeed + Mathf.Max(currentMoveSpeed - playerSpeedThreshold, 0);
                projectile.transform.position = weaponData[3].FirePoint.transform.position;
                projectile.transform.rotation = weaponData[3].FirePoint.transform.rotation;
                projectile.GetComponent<Rigidbody>().velocity = weaponData[3].FirePoint.transform.forward * adjustedBulletSpeed;
               

            }



        }
       
    }

    private GameObject FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        GameObject nearEnemy = null;

        Transform nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider col in colliders)
        {
            float distance = Vector3.Distance(transform.position, col.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = col.transform;
                nearEnemy = col.gameObject;
            }
        }

        if (nearestEnemy != null)
        {
            
            Debug.Log("Nearest enemy position: " + nearestEnemy.position);
        }

        return nearEnemy;
    }
}


public enum WeaponType
{
    SimpleGun,
    miniGun,
    Rocketlauncher,
    DoubleminiGun

}
