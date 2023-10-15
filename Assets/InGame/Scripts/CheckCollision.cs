using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{

    private WeaponSystem Ws;
    PhotonView view;
    GameObject Target;
    private void Start()
    {
        Ws =GetComponent<WeaponSystem>();
        view = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!view.IsMine)
            return;

        if (other.CompareTag("SimpleGun"))
        {
            Ws.Type = WeaponType.SimpleGun;
            Target = other.gameObject;
            view.RPC("DisableOverNetwork", RpcTarget.All);

        }

        if (other.CompareTag("miniGun"))
        {
            Ws.Type = WeaponType.miniGun;

            Target = other.gameObject;
            view.RPC("DisableOverNetwork", RpcTarget.All);
        }

        if (other.CompareTag("Rocketlauncher"))
        {
            Ws.Type = WeaponType.Rocketlauncher;
            Target = other.gameObject;
            view.RPC("DisableOverNetwork", RpcTarget.All);
        }
        if (other.CompareTag("DoubleminiGun"))
        {
            Ws.Type = WeaponType.DoubleminiGun;
            Target = other.gameObject;
            view.RPC("DisableOverNetwork", RpcTarget.All);
        }
    }

    [PunRPC]
    private void DisableOverNetwork()
    {

        Target.SetActive(false);
    }
}
