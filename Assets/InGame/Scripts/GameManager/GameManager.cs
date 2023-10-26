using Photon.Pun;
using System.Collections;

using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
   
    public float GameStartcountdownTime = 5f;
    public int minPlayersToStart = 4;
    public static bool gameStarted =false;
    public static bool gameFinished=false;
    [SerializeField] float GameTimeLimit = 10;
    private float timer = 0f; // GameTime

    
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        // Check the number of players when the first player joins
    /*    if (PhotonNetwork.IsMasterClient)
        {
            CheckPlayers();
        }*/
        
            CheckPlayers();
       

    }

    private void Update()
    {
        if (gameStarted)
        {
            if (timer >= 1f)
            {

                UImanager.instance.gameTimeText.text = Mathf.FloorToInt(timer).ToString();

            }
            float elapsedTime = Time.time - timer;

            // Calculate minutes and seconds
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
        
            // Update the timer text
            UImanager.instance.gameTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            if (elapsedTime >= GameTimeLimit)
            {
                gameFinished = true;
            }


         
        }
       
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        // Check the number of players whenever a new player joins
        CheckPlayers();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        // Check the number of players whenever a player leaves
        CheckPlayers();
    }

    void CheckPlayers()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= minPlayersToStart)
        {
            // If minimum players have joined, start the game
            MinPlayerJoined();
        }
        else
        {
            DisplayWaitingMessage();
            Debug.Log("Waiting for more players to join...");
        }
    }

    void DisplayWaitingMessage()
    {
        if (UImanager.instance.gameStartCountDownTxt != null)
        {
            UImanager.instance.gameStartCountDownTxt.text = "Waiting for players... " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + minPlayersToStart;
        }
    }

    IEnumerator StartCountdown()
    {
        float currentTime = GameStartcountdownTime;

        while (currentTime > 0)
        {
            // Display the current countdown time in the UI Text
            UImanager.instance.gameStartCountDownTxt.text = currentTime.ToString("0");

            // Wait for one second
            yield return new WaitForSeconds(1f);

            // Decrease the countdown time
            currentTime--;
        }

        // When the countdown reaches zero, start the game
       // UImanager.instance.gameStartCountDownTxt.text = "Start!!";
        StartGame();
    }

    void MinPlayerJoined()
    {

        // Start the countdown when the script is enabled
        StartCoroutine(StartCountdown());
    }

    void StartGame()
    {
        gameStarted = true;
        UImanager.instance.gameStartCountDownTxt.gameObject.SetActive(false);
        // Add your game start logic here
        Debug.Log("Game started!");
    }
 
}