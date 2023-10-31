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

 
    [SerializeField]private float RespawnTime = 5f;

    [SerializeField] PhotonView view;
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
        view = player.GetComponent<PhotonView>();
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
        if (player != null)
        {
            if (!player.isAlive)
            {
                if (view.IsMine)
                {
                    RespawnTime -= Time.deltaTime;

                    if (RespawnTime <= 0f)
                    {
                        UImanager.instance.gameStartCountDownTxt.gameObject.SetActive(false);
                        player.OnPlayerRespawn();
                        RespawnTime = 5f; // Reset the timer when the player respawns
                    }
                    else
                    {
                        UImanager.instance.gameStartCountDownTxt.gameObject.SetActive(true);
                        // Display the current countdown time in the UI Text
                        UImanager.instance.gameStartCountDownTxt.text = Mathf.CeilToInt(RespawnTime).ToString("0");
                    }
                }
            }
            else
            {
                // Reset the timer when the player is alive
                RespawnTime = 5f;
            }
        }

    }
    

}
