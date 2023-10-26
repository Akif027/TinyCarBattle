using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{

    private WeaponSystem Ws;
 private PlayerHealth P_health;
  [SerializeField]  PhotonView view;
    private void Start()
    {
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
            Debug.Log("collected " + other.name);
            view.RPC("WeaponTypeSnicRPC", RpcTarget.All, WeaponType.Rocketlauncher);

        }
        if (other.CompareTag("DoubleminiGun"))
        {
            if (Ws.isCountingDown is true)
            {
                Ws.isCountingDown = false;
            }
            //  Ws.Type = WeaponType.DoubleminiGun;
            view.RPC("WeaponTypeSnicRPC", RpcTarget.All, WeaponType.DoubleminiGun);
            Debug.Log("DoubleMiniGun collected");
        }
        if (other.CompareTag("ShieldLevel4"))
        {
            Ws.EnableWeapon(4);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!view.IsMine)
            return;

        if (collision.collider.tag == "Bullet")
        {

            P_health.TakeDamage(10);

        }



    }


    [PunRPC]
    private void WeaponTypeSnicRPC(WeaponType type)
    {
        Ws.Type = type;
    }


}
