using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using System.Linq;
using System.Xml;


public class GameManager : MonoBehaviourPunCallbacks
{
   
    public float GameStartcountdownTime = 5f;
    public int minPlayersToStart = 4;
    public static bool gameStarted =false;
    public static bool gameFinished=false;
    [SerializeField] float GameTimeLimit = 10;

    private int minutes = 0;
    private float seconds = 0f;

   [SerializeField] PhotonView view;
    public static GameManager Instance;

    private Dictionary<int, PlayerStats> playerStatsDictionary = new Dictionary<int, PlayerStats>();

    public TMP_Text GlobalkillText;  // Reference to your TMP Text prefab
    public Transform KillBoard;
    public List<TMP_Text> KillListText = new List<TMP_Text>();

    bool CountDownStarted = false;

    public int LocalPlayerKills = 0; 
    private void Awake()
    {
      
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.AddCallbackTarget(this);

        if (!gameStarted)
        {
            CheckPlayers();
        }
        view = GetComponent<PhotonView>();

    }


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.T))
        {
            // Toggle the visibility of the cursor
            Cursor.visible = !Cursor.visible;

            // Lock or unlock the cursor based on visibility
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }

        if (gameStarted && !gameFinished )
        {
            CountDownStarted = false;
            AudioManger.instance.Stop("TextCountDown");

            if (gameStarted && !gameFinished)
            {
                // Update the timer
                seconds += Time.deltaTime;

                // Check if one second has passed
                if (seconds >= 60f)
                {
                    seconds -= 60f; // Reduce seconds by 60
                    minutes += 1;   // Increase minutes by 1
                }

                // Update the UI
                UImanager.instance.gameTimeText.text = string.Format("{0:00}:{1:00}", minutes, Mathf.FloorToInt(seconds));

                // Check if the game time limit is reached
                if (minutes >= GameTimeLimit)
                {
                    gameFinished = true;
                    CheckForWinner();
                    // You might want to add some code to handle the game finishing.
                }
            }

        }
        if (CountDownStarted)
        {
            AudioManger.instance.Play("TextCountDown");
        }

     
    }

    private void CheckForWinner()
    {
        var sortedLeaderboard = playerStatsDictionary.OrderByDescending(x => x.Value.killCount);

        foreach (var entry in sortedLeaderboard)
        {
           

            // Check for the highest kill count and display winner
            if (LocalPlayerKills == sortedLeaderboard.First().Value.killCount)
            {
              
              UImanager.instance.WinnerGameOverPanel.SetActive(true);
            }
            else
            {
                UImanager.instance.DefeatGameOverPanel.SetActive(false);
            }

           
        }

    }

    public void InitializePlayerStats(Player newPlayer)
    {
        // Check if the player already exists in the dictionary
        if (!playerStatsDictionary.ContainsKey(newPlayer.ActorNumber))
        {
           
            TMP_Text tmpText = Instantiate(GlobalkillText, KillBoard);
            tmpText.gameObject.SetActive(true);
            KillListText.Add(tmpText);
            // If not, initialize player stats and add to the dictionary
            tmpText.text = newPlayer.NickName;
            PlayerStats newPlayerStats = new PlayerStats
            {
                playerName = newPlayer.NickName,
               
                killCount = 0
            };

            playerStatsDictionary.Add(newPlayer.ActorNumber, newPlayerStats);
            UpdateLeaderboard();
        }
        else
        {
            // Player already exists, you may want to handle this case accordingly
            Debug.LogWarning("Player " + newPlayer.NickName + " already exists in playerStatsDictionary.");
        }
    }

    public void UpdateLeaderboard()
    {
        // Sort the leaderboard based on kill counts
        var sortedLeaderboard = playerStatsDictionary.OrderByDescending(x => x.Value.killCount);
        int i = 0;
        // Display or update the leaderboard
        foreach (var entry in sortedLeaderboard)
        {

            Debug.Log($"{entry.Value.playerName} : {entry.Value.killCount} kills");
         
            KillListText[i].text = $"{entry.Value.playerName} : {entry.Value.killCount} kills";


            Debug.Log(i + " entries");
            i++;
        }
    }

  
    private void OnDestroy()
    {
        // Unsubscribe from events
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void OnPlayerDeath(int playerID)
    {

        view.RPC("SyncKills", RpcTarget.AllBuffered,playerID);

        // Update leaderboard
        UpdateLeaderboard();
    }

    [PunRPC]
    public void SyncKills(int playerID)
    {
      
        // Increment kill count for the killer
        if (playerStatsDictionary.ContainsKey(playerID))
        {
            Debug.Log("OnplayerDeath iS called " + playerID);
            playerStatsDictionary[playerID].killCount++;

         
        }
        UpdateLeaderboard();

    }
  
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

        // Check the number of players whenever a new player joins
        CheckPlayers();
       
        base.OnPlayerEnteredRoom(newPlayer);
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

        // Update leaderboard
        UpdateLeaderboard();
        // Check the number of players whenever a player leaves
        CheckPlayers();
        base.OnPlayerLeftRoom(otherPlayer);
    }
    void CheckPlayers()
    {
        
        Debug.Log("enterd");
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
        if (  UImanager.instance.gameStartCountDownTxt != null)
        {
            UImanager.instance.gameStartCountDownTxt.text = "Waiting for players... " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + minPlayersToStart;
        }
    }

    IEnumerator StartCountdown()
    {
        float currentTime = GameStartcountdownTime;
       
        while (currentTime > 0)
        {
            CountDownStarted = true;

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
