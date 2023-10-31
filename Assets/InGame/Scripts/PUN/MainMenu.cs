
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks
{

    public TMP_InputField CreateInput;
    public TMP_InputField joinInput;
    public TMP_InputField NameInput;
   public GameObject RoomPanel;
 public GameObject Platform;


    public void ChangeName()
    {
        PhotonNetwork.NickName = NameInput.text;

    }

    public void CreateRoom()
    {

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        PhotonNetwork.CreateRoom(CreateInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");

    }

    /*  public void createRoomPanelBtn()
      {
          CreateRoomPanel.SetActive(true);
          JoinRoomPanel.SetActive(false);
      }
      public void JoinRoomPanelBtn()
      {
          JoinRoomPanel.SetActive(true);
          CreateRoomPanel.SetActive(false);
      }
    */

    public void OnStart()
    {
        RoomPanel.SetActive(true);
        Platform.SetActive(false);


    }
    public void ClosePanel()
    {
        if (RoomPanel.activeSelf)
        {
            RoomPanel.SetActive(false);
            Platform.SetActive(true);
        }
     
    }
    public void Quit() { 
    
      Application.Quit();
    }
}
