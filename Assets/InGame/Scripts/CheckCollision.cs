using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{

    private WeaponSystem Ws;//weapon enum
    private PlayerHealth P_health;//player health
   [SerializeField]  PhotonView view;

    CarEnginesound sound ;//sound 
    private void Start()
    {
        sound = GetComponentInChildren<CarEnginesound>();
        Ws = GetComponent<WeaponSystem>();
      P_health = GetComponent<PlayerHealth>();
        view = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {


        if (other.CompareTag("SimpleGun"))
        {
            // Ws.Type = WeaponType.SimpleGun;
            if (Ws.isCountingDown is true)
            {
                Ws.isCountingDown = false;
            }
           sound.PickUpsound();
            view.RPC("WeaponTypeSnicRPC", RpcTarget.All, WeaponType.SimpleGun);
            Debug.Log("collected " + other.name);

       
        }

        if (other.CompareTag("miniGun"))
        {
            //  Ws.Type = WeaponType.miniGun;
            if (Ws.isCountingDown is true)
            {
                Ws.isCountingDown = false;
            }
            sound.PickUpsound();
            view.RPC("WeaponTypeSnicRPC", RpcTarget.All, WeaponType.miniGun);
            Debug.Log("collected " + other.name);

        }

        if (other.CompareTag("Rocketlauncher"))
        {
            //  Ws.Type = WeaponType.Rocketlauncher;
            if (Ws.isCountingDown is true)
            {
                Ws.isCountingDown = false;
            }
            sound.PickUpsound();
            Debug.Log("collected " + other.name);
            view.RPC("WeaponTypeSnicRPC", RpcTarget.All, WeaponType.Rocketlauncher);

        }
        if (other.CompareTag("DoubleminiGun"))
        {
            if (Ws.isCountingDown is true)
            {
                Ws.isCountingDown = false;
            }
            sound.PickUpsound();
            //  Ws.Type = WeaponType.DoubleminiGun;
            view.RPC("WeaponTypeSnicRPC", RpcTarget.All, WeaponType.DoubleminiGun);
            Debug.Log("DoubleMiniGun collected");
        }
        if (other.CompareTag("ShieldLevel4"))
        {
            Ws.EnableWeapon(4);
            sound.PickUpsound();
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!view.IsMine)
            return;

        if (collision.collider.tag == "Bullet")
        {

            P_health.TakeDamage(10);

          sound.BulletImpact();
        }

        if (collision.collider.tag == "Missile")
        {
            Debug.Log("missile hit");
            P_health.TakeDamage(100);

        }
        if (/*collision.collider.tag == "Ground" || */collision.collider.tag == "Objects")
        {
         
           
            // Check the magnitude of the collision force
            float collisionForce = collision.impulse.magnitude;
            sound.PlayCollsionSound(collisionForce);

        }



    }


    [PunRPC]
    private void WeaponTypeSnicRPC(WeaponType type)
    {
        Ws.Type = type;
    }


}
