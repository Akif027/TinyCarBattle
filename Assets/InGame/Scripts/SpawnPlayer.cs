using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SpawnPlayer : MonoBehaviour
{

    [SerializeField] private List<Carsdata> Cars = new List<Carsdata>();
    [SerializeField] private PlayerHealth player;
 
    public Transform[] waypoints;

    // Flag to track whether spawning has already occurred
    private bool hasSpawned = false;

    private int CarIndex = 0;

    private float RespawnTime = 5f;
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
        player = FindObjectOfType<PlayerHealth>();
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


    private void Update()
    {
     
        if (!player.isAlive)
        {
            StartCoroutine(RepawnPlayer());
        }
    }
    

    IEnumerator RepawnPlayer()
    {
        float currentTime = RespawnTime;
        UImanager.instance.gameStartCountDownTxt.gameObject.SetActive(true);
        while (currentTime > 0)
        {
            // Display the current countdown time in the UI Text
            UImanager.instance.gameStartCountDownTxt.text = currentTime.ToString("0");

            // Wait for one second
            yield return new WaitForSeconds(1f);

            // Decrease the countdown time
            currentTime--;
        }


        UImanager.instance.gameStartCountDownTxt.gameObject.SetActive(false);
        player.OnPlayerRespawn();
    }
}
