using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class cameraSwitch : MonoBehaviour
{
   public GameObject[] CameraC;


    private void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
           
            CameraC[0].SetActive(false);
            CameraC[1].SetActive(true);    

        }
        if (Input.GetKey(KeyCode.G))
        {

            CameraC[1].SetActive(false);
            CameraC[0].SetActive(true);

        }
    }
}
