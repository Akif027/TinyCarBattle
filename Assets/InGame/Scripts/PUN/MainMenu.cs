
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks
{

    public InputField CreateInput;
    public InputField joinInput;
    public InputField[] NameInput;
  //  public GameObject JoinRoomPanel;
  //  public GameObject CreateRoomPanel;


    public void ChangeName()
    {
        PhotonNetwork.NickName = NameInput[0].text;

    }

    public void ChangeNameForJoinRoom()
    {
        PhotonNetwork.NickName = NameInput[1].text;
    }
    public void CreateRoom()
    {

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
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

    public void ClosePanel()
    {
        if (!JoinRoomPanel.activeSelf)
        {
            CreateRoomPanel.SetActive(false);
        }
        else
        {
            JoinRoomPanel.SetActive(false);

        }
    }*/
}
