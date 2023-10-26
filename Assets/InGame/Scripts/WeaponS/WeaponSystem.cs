using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections;
using Tarodev;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSystem : MonoBehaviourPun
{

    public WeaponType Type;

    PhotonView view;

    [Header("HomingMissle")]
    public float detectionRadius = 10f;
    public LayerMask enemyLayer;
    


    [Header("Weapon Setting")]
    [SerializeField] TMP_Text countdownText; // this Text is for weaponTimeTextDisplay
    public float countdownTime = 10.0f; // Adjust this time as needed

    [HideInInspector]
    public bool isCountingDown = false;//for weapon
    public WeaponData[] weaponData = new WeaponData[3];
    public float rotationSpeed = 5.0f;
    private float rotationX = 0;
    private float rotationY = 0;



    private void Start()
    {
        view = GetComponent<PhotonView>();

        Type = WeaponType.SimpleGun;

        enemyLayer = LayerMask.GetMask("Enemy");
      
      

   
        for (int i = 1; i < weaponData.Length; i++) //disable the weapon at start execpt the simple gun 
        {
            weaponData[i].WeaponPrefab.SetActive(false);

        }


        countdownText = UImanager.instance.WeaponcountdownText;

        //setting tag and layer based on local Player
        int targetLayer = view.IsMine ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("Enemy");
        string SetTag = view.IsMine ? "Player" : "Enemy";
        gameObject.tag = SetTag;
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
            // Rotation for SimpleGun
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Adjust the rotation speed based on your preference
         
            rotationX -= mouseY * rotationSpeed;
            rotationX = Mathf.Clamp(rotationX, -2.0f, 10.0f);

            rotationY += mouseX * rotationSpeed;
            rotationY = Mathf.Clamp(rotationY, -90.0f, 90.0f);

            // Create a Quaternion for rotationX and rotationY
            Quaternion newRotation = Quaternion.Euler(rotationX, rotationY, 0);

            Transform TurretTop = weaponData[0].WeaponPrefab.transform.GetChild(0).GetChild(1).transform;
            // Apply the rotation to the GameObject (both on the X-axis and Y-axis).
            TurretTop.localRotation = newRotation;

            // Send the rotation change to other players using an RPC.
            photonView.RPC("SyncRotationRPC", RpcTarget.Others, newRotation);
        }
    }

    [PunRPC]
    private void SyncRotationRPC(Quaternion newRotation)
    {
        Transform TurretTop = weaponData[0].WeaponPrefab.transform.GetChild(0).GetChild(1).transform;
        // This RPC is called for other players. Synchronize their view.
        TurretTop.localRotation  = newRotation;
    }
    #region LogicForWeaponAttack

    private void ChangeWeaponOnType()
    {

        switch (Type)
        {
            case WeaponType.SimpleGun:
                if (Input.GetMouseButtonDown(0) && Time.time >= weaponData[0].nextFireTime)
                {
                    view.RPC("SimpleGunWeapon", RpcTarget.All);

                }
                break;
            case WeaponType.miniGun:
                if (!isCountingDown)
                {
                    EnableWeapon(1);
                    DisableWeapon(2);
                    countdownTime = 0; // just to be sure
                    countdownTime = weaponData[1].WeaponTime;
                    isCountingDown = true;
                   
                }
                DisableObjectAfterTime(1);
                if (Input.GetMouseButton(0) && Time.time >= weaponData[1].nextFireTime)
                {
                    view.RPC("MiniGunWeapon", RpcTarget.All);
                }
                break;
            case WeaponType.DoubleminiGun:
                if (!isCountingDown)
                {
                    EnableWeapon(2);
                    EnableWeapon(1);
                    countdownTime = 0;
                    countdownTime = weaponData[2].WeaponTime;
                    isCountingDown = true;
                  
                }
                DisableObjectAfterTime(1,2);
                if (Input.GetMouseButton(0) && Time.time >= weaponData[1].nextFireTime)
                {
                    view.RPC("MiniGunWeapon", RpcTarget.All);
                }
                break;
            case WeaponType.Rocketlauncher:
                if (!isCountingDown)
                {
                    EnableWeapon(3);
                    countdownTime = 0;
                    countdownTime = weaponData[3].WeaponTime;
                    isCountingDown = true;

                }
                DisableObjectAfterTime(3,2,1);
            
                if (Input.GetMouseButtonDown(0) && Time.time >= weaponData[3].nextFireTime)
                {
                    view.RPC("RocketLauncherWeapon", RpcTarget.All);
                }
                break;
        }
    }

    private void DisableObjectAfterTime(int WeaponIndex, int WeaponIndex2 = 0 , int WeaponIndex3 = 0)
    {
        if (countdownTime > 0)
        {
            countdownText.gameObject.SetActive(true);
            countdownTime -= Time.deltaTime;
            if (countdownText != null)
            {
                countdownText.text = "Weapon Time left: " + Mathf.Floor(countdownTime).ToString();// Display with one decimal place
            }
        }
        else
        {
            if (countdownText != null)
            {
                countdownText.text = "Weapon Time left: 0";
            }

            // Disable the GameObject when the countdown reaches zero
            DisableWeapon(WeaponIndex);
            countdownText.gameObject.SetActive(false);
            if (WeaponIndex2 != 0)
            {
                DisableWeapon(WeaponIndex2);
            }
            if (WeaponIndex3 != 0)
            {
                DisableWeapon(WeaponIndex3);
            }
            isCountingDown = false;
            Type = WeaponType.SimpleGun;
        }
    }

    [PunRPC]
    private void SimpleGunWeapon()
    {
     


        Debug.Log("gun firing");

        weaponData[0].nextFireTime = Time.time + weaponData[0].fireRate;

        GameObject projectile = ObjectPool.Instance.GetPooledObject(weaponData[0].AmmoPrefab);
        projectile.GetComponent<Ammo>().setPView(GetComponent<PlayerHealth>());
        float currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
        if (projectile != null)
        {

            projectile.transform.position = weaponData[0].FirePoint.transform.position;
            projectile.transform.rotation = weaponData[0].FirePoint.transform.rotation;
            float playerSpeedThreshold = 1.0f; // You can adjust this threshold as needed
            float bulletBaseSpeed = weaponData[0].speedofBullet; // The base speed of the bullet
            float currentMoveSpeed = currentSpeed;
            // Calculate the bullet speed based on the player's speed
            float adjustedBulletSpeed = bulletBaseSpeed + Mathf.Max(currentMoveSpeed - playerSpeedThreshold, 0);

            // Apply the adjusted speed to the bullet's velocity
            projectile.GetComponent<Rigidbody>().velocity = weaponData[0].FirePoint.transform.forward * adjustedBulletSpeed;
        }



    }

    [PunRPC]
    private void MiniGunWeapon()
    {

        if (Type == WeaponType.miniGun)
        {



            // Vector3 Temp = new Vector3(target.transform.position.x, 5, target.transform.position.z);

            Debug.Log(" mini gun firing");

            weaponData[1].nextFireTime = Time.time + weaponData[1].fireRate;

            GameObject projectile = ObjectPool.Instance.GetPooledObject(weaponData[1].AmmoPrefab);
            projectile.GetComponent<Ammo>().setPView(GetComponent<PlayerHealth>());
            float currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
            if (projectile != null)
            {

                projectile.transform.position = weaponData[1].FirePoint.transform.position;
                projectile.transform.rotation = weaponData[1].FirePoint.transform.rotation;
                float playerSpeedThreshold = 1.0f; // You can adjust this threshold as needed
                float bulletBaseSpeed = weaponData[1].speedofBullet; // The base speed of the bullet
                float currentMoveSpeed = currentSpeed ;
                // Calculate the bullet speed based on the player's speed
                float adjustedBulletSpeed = bulletBaseSpeed + Mathf.Max(currentMoveSpeed - playerSpeedThreshold, 0);

                // Apply the adjusted speed to the bullet's velocity
                projectile.GetComponent<Rigidbody>().velocity = weaponData[1].FirePoint.transform.forward * adjustedBulletSpeed;
            }




        }
        else if (Type == WeaponType.DoubleminiGun)
        {

            // Vector3 Temp = new Vector3(target.transform.position.x, 5, target.transform.position.z);

            Debug.Log("Double mini gun firing");

            weaponData[1].nextFireTime = Time.time + weaponData[1].fireRate;
            for (int i = 1; i < 3; i++)  //doubleMiniGun element which is 2 and 3 so need to loop through it. Note* dont change the element sequence otherwise u need to rearrange it
            {
                GameObject projectile = ObjectPool.Instance.GetPooledObject(weaponData[1].AmmoPrefab);
                projectile.GetComponent<Ammo>().setPView(GetComponent<PlayerHealth>());
                float currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
                if (projectile != null)
                {

                    projectile.transform.position = weaponData[i].FirePoint.transform.position;
                    projectile.transform.rotation = weaponData[i].FirePoint.transform.rotation;
                    float playerSpeedThreshold = 1.0f; // You can adjust this threshold as needed
                    float bulletBaseSpeed = weaponData[2].speedofBullet; // The base speed of the bullet
                    float currentMoveSpeed = currentSpeed;
                    // Calculate the bullet speed based on the player's speed
                    float adjustedBulletSpeed = bulletBaseSpeed + Mathf.Max(currentMoveSpeed - playerSpeedThreshold, 0);

                    // Apply the adjusted speed to the bullet's velocity
                    projectile.GetComponent<Rigidbody>().velocity = weaponData[i].FirePoint.transform.forward * adjustedBulletSpeed;
                }

            }


        }


    }

    [PunRPC]
    private void RocketLauncherWeapon()
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
            float bulletBaseSpeed = weaponData[3].speedofBullet; // The base speed of the bullet
            float currentMoveSpeed = currentSpeed;
            // Calculate the bullet speed based on the player's speed
            float adjustedBulletSpeed = bulletBaseSpeed + Mathf.Max(currentMoveSpeed - playerSpeedThreshold, 0);
            projectile.transform.position = weaponData[3].FirePoint.transform.position;
            projectile.transform.rotation = weaponData[3].FirePoint.transform.rotation;
            projectile.GetComponent<Rigidbody>().velocity = weaponData[3].FirePoint.transform.forward * adjustedBulletSpeed;


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

    #endregion


    #region DisbaleOrEnableWeapon
    public void EnableWeapon(int I)
    {
        view.RPC("EnableWeaponRPC", RpcTarget.All, I);
    }

    public void DisableWeapon(int I)
    {
        view.RPC("DisableWeaponRPC", RpcTarget.All, I);
    }

    [PunRPC]
    private void EnableWeaponRPC(int I)
    {
        weaponData[I].WeaponPrefab.SetActive(true);
    }

    [PunRPC]
    private void DisableWeaponRPC(int I)
    {
        weaponData[I].WeaponPrefab.SetActive(false);
    }

    #endregion

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}


public enum WeaponType
{
    SimpleGun,
    miniGun,
    Rocketlauncher,
    DoubleminiGun

}
