using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class WeaponSpawner : MonoBehaviour
{
    public List<GameObject> weapons;
    public float minX, maxX, minZ, maxZ;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (weapons.Count == 0)
        {
            Debug.LogError("No weapons in the list.");
            return;
        }

        foreach (GameObject weaponPrefab in weapons)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(minX, maxX),
                1f, // Set the Y-coordinate to zero
                Random.Range(minZ, maxZ)
            );

            GameObject weapon = PhotonNetwork.Instantiate(weaponPrefab.name, randomPos, Quaternion.identity);
        }
    }
}
