using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public class WeaponSpawner : MonoBehaviour
{
    public List<GameObject> weapons;
    public float minX, maxX, minZ, maxZ;
    public List<GameObject> weaponHolderChilds;
    public Transform[] waypoints;

    private PhotonView view;
    private List<int> occupiedWaypoints = new List<int>();

    private void Start()
    {
        view = GetComponent<PhotonView>();
        if (!PhotonNetwork.IsMasterClient)
            return;

        SpawnWeapon();
    }

    private void SpawnWeapon()
    {
        if (weapons.Count == 0)
        {
            Debug.LogError("No weapons in the list.");
            return;
        }

        foreach (GameObject weaponPrefab in weapons)
        {
            int randomIndex = GetRandomUnoccupiedWaypoint();
            if (randomIndex == -1)
            {
                Debug.LogError("All waypoints are occupied.");
                break;
            }

            Vector3 randomPos = waypoints[randomIndex].position;
            GameObject weapon = PhotonNetwork.Instantiate(weaponPrefab.name, randomPos, Quaternion.identity);
            weaponHolderChilds.Add(weapon);
            occupiedWaypoints.Add(randomIndex);
        }
    }

    private int GetRandomUnoccupiedWaypoint()
    {
        List<int> unoccupiedIndices = new List<int>();
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (!occupiedWaypoints.Contains(i))
            {
                unoccupiedIndices.Add(i);
            }
        }

        if (unoccupiedIndices.Count > 0)
        {
            int randomIndex = unoccupiedIndices[Random.Range(0, unoccupiedIndices.Count)];
            return randomIndex;
        }

        return -1; // All waypoints are occupied
    }

    private void Update()
    {
        foreach (GameObject child in weaponHolderChilds)
        {
            if (!child.activeSelf)
            {
                StartCoroutine(EnableWeapon(child));
            }
        }
    }

    IEnumerator EnableWeapon(GameObject gameobject)
    {
        int randomIndex = GetRandomUnoccupiedWaypoint();
        if (randomIndex != -1)
        {
            Vector3 spawnPosition = waypoints[randomIndex].position;
            gameobject.transform.position = spawnPosition;

            yield return new WaitForSeconds(3);

            EnableOverTheNetwork(gameobject);
        }
        else
        {
            Debug.LogError("All waypoints are occupied. Cannot respawn the weapon.");
        }
    }

    public void EnableOverTheNetwork(GameObject gameobject)
    {
        view.RPC("EnableOverNetworkRPC", RpcTarget.AllBuffered, gameobject.GetPhotonView().ViewID);
    }

    [PunRPC]
    private void EnableOverNetworkRPC(int viewID)
    {
        PhotonView photonView = PhotonView.Find(viewID);
        if (photonView != null)
        {
            GameObject gameobject = photonView.gameObject;
            gameobject.SetActive(true);
            Debug.Log("Enable " + gameobject.name);
        }
    }
}

