using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsMine : MonoBehaviour
{
    [Header("Only Enble for this player")]
    [SerializeField] GameObject Camera;
    PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();

        if (!view.IsMine)
        {
           
            Camera.SetActive(false);
        }
    }
}
