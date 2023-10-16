using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{

    private WeaponSystem Ws;
    PhotonView view;
    private void Start()
    {
        Ws = GetComponent<WeaponSystem>();
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


    }

    [PunRPC]
    private void WeaponTypeSnicRPC(WeaponType type)
    {
        Ws.Type = type;
    }


}
