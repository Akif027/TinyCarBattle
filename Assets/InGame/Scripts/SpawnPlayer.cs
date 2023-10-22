using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;


public class SpawnPlayer : MonoBehaviour
{

    [SerializeField] private List<Carsdata> Cars = new List<Carsdata>();
    private int CarIndex = 0;
    public Transform[] waypoints;

    // Flag to track whether spawning has already occurred
    private bool hasSpawned = false;

    private void Awake()
    {
        CarIndex = PlayerPrefs.GetInt("CarIndex");
        Debug.Log(CarIndex);

        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints assigned!");
            return;
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.DeleteAll();
    }

    private void Start()
    {
        // Check if spawning has already occurred
        if (!hasSpawned)
        {
            SpawnPlayers();
        }
    }

    private void SpawnPlayers()
    {
        Vector3 spawnPosition ;
        Quaternion spawnRotation;
        // Instantiate players one at a time
       
            int waypointIndex = (CarIndex + PhotonNetwork.LocalPlayer.ActorNumber - 1) % waypoints.Length;
            spawnPosition = waypoints[waypointIndex].position;
           spawnRotation = waypoints[waypointIndex].rotation;

      
        // Instantiate each player separately
        PhotonNetwork.Instantiate(Cars[CarIndex].TypeOfCar.name, spawnPosition, spawnRotation);
        // Set the flag to true, indicating that spawning has occurred
        hasSpawned = true;
    }
}
