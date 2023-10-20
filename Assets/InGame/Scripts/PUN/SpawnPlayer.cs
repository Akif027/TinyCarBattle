using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnPlayer : MonoBehaviour
{



    int Playerindex = 0;
   // public List<ShipDataScriptable> Ships = new List<ShipDataScriptable>();
   public GameObject Player;
    public float minX, maxX, minY, maxY;

    private void Awake()
    {


        Playerindex = PlayerPrefs.GetInt("ShipIndex");

        Vector2 randomPos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        PhotonNetwork.Instantiate(Player.name, randomPos, Quaternion.identity);

    }

}
