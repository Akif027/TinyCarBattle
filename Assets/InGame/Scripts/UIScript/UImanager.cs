
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UImanager : MonoBehaviourPunCallbacks
{

    public static UImanager instance;

    public TMP_Text WeaponcountdownText;
    public TMP_Text gameStartCountDownTxt;
    public TMP_Text NameText;
    public TMP_Text gameTimeText;
    public TMP_Text killText;
    public Slider healthSlider;
 
    public GameObject pausePanel;
 
    private void Awake()
    {
        instance = this;

    }

   
    public void ExitRoomAndGoToMainMenuScene()
    {
        // Check if connected to the Photon server
        if (PhotonNetwork.IsConnected)
        {
            // Check if currently in a room
            if (PhotonNetwork.InRoom)
            {
                // Leave the current room
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                // If not in a room, go directly to the main menu scene
                GoToMainMenuScene();
            }
        }
        else
        {
            // If not connected, go directly to the main menu scene
            GoToMainMenuScene();
        }
    }


    private void GoToMainMenuScene()
    {
        // Load the main menu scene
        SceneManager.LoadScene(1);
    }

    public override void OnLeftRoom()
    {
        // Go to the main menu scene
        GoToMainMenuScene();
    }

    public void closePausePanel()
    {
        pausePanel.SetActive(false);
    }
    public void OpenPausePanel()
    {
        pausePanel.SetActive(true);
    }




}
