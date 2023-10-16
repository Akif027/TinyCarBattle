using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{

    private WeaponSystem Ws;
  
    private void Start()
    {
        Ws =GetComponent<WeaponSystem>();
     
    }

    private void OnTriggerEnter(Collider other)
    {

       
            if (other.CompareTag("SimpleGun"))
            {
                Ws.Type = WeaponType.SimpleGun;
              
              
              Debug.Log("collected " + other.name);
            

            }

            if (other.CompareTag("miniGun"))
            {
                Ws.Type = WeaponType.miniGun;

              
            Debug.Log("collected " + other.name);
     
            }

            if (other.CompareTag("Rocketlauncher"))
            {
                Ws.Type = WeaponType.Rocketlauncher;
              
            Debug.Log("collected " + other.name);
        

            }
            if (other.CompareTag("DoubleminiGun"))
            {
                Ws.Type = WeaponType.DoubleminiGun;
             
                Debug.Log("DoubleMiniGun collected");
            }
  
      
    }



}
