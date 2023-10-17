using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
/*
    private float rotationX;
    public float rotationSpeed = 3.0f; // Adjust the speed as needed.
    private PhotonView view;
    private WeaponSystem weaponSystem;
    void Start()
    {
        view = GetComponentInParent<PhotonView>();
        weaponSystem = GetComponentInParent<WeaponSystem>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // This is the local player's GameObject, send the rotationX data to others.
            stream.SendNext(rotationX);
        }
        else
        {
            // This is a remote player's GameObject, receive the rotationX data.
            rotationX = (float)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        if (view.IsMine)
        {
            ChangeWeaponOnType();

            float mouseX = Input.GetAxis("Mouse X");
            rotationX += mouseX * rotationSpeed;
            rotationX = Mathf.Clamp(rotationX, -90.0f, 90.0f); // Optional: Clamp rotationX within a range.

            // Apply the rotation to the GameObject (only on the Y-axis).
            weaponSystem.weaponData[0].WeaponPrefab.transform.localRotation = Quaternion.Euler(0, rotationX, 0);
        }
        else
        {
            // Ensure the correct GameObject is being synchronized.
            if (weaponSystem.weaponData[0].WeaponPrefab != null)
            {
                weaponSystem.weaponData[0].WeaponPrefab.transform.localRotation = Quaternion.Euler(0, rotationX, 0);
            }
        }
    }

    private void ChangeWeaponOnType()
    {
        // Implement your weapon type change logic here.
    }*/
}
