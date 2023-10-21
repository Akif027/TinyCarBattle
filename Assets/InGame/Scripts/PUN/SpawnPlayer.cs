using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnPlayer : MonoBehaviour
{



    public GameObject playerPrefab;
    public Transform[] waypoints;

    private void Awake()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints assigned!");
            return;
        }

        // Instantiate players at each waypoint
        for (int i = 0; i < Mathf.Min(waypoints.Length, PhotonNetwork.CurrentRoom.PlayerCount); i++)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, waypoints[i].position, waypoints[i].rotation);
        }
    }
}
