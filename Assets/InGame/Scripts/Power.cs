using Newtonsoft.Json.Bson;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour
{
    public float rotationSpeed = 45f;
    public float moveSpeed = 2f;
    public float amplitude = 0.5f;

    private float initialY;
    private float time;

    PhotonView view;
   


    void Start()
    {
        view = GetComponent<PhotonView>();  
        // Store the initial Y position of the GameObject
        initialY = transform.position.y;
    }

    void Update()
    {
        // Rotate the GameObject smoothly using Lerp
        Quaternion targetRotation = Quaternion.Euler(0f, Time.time * rotationSpeed, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);

        // Move the GameObject up and down using Mathf.Sin
        time += Time.deltaTime * moveSpeed;
        Vector3 newPosition = transform.position;
        newPosition.y = initialY + Mathf.Sin(time) * amplitude;
        transform.position = newPosition;



    }

    private void OnTriggerEnter(Collider other)
    {


        if (other.CompareTag("Player"))
        {
         
            view.RPC("DisablePowerOverNetworkRPC", RpcTarget.AllBuffered);

        }
     

    }



    [PunRPC]
    private void DisablePowerOverNetworkRPC()
    {

       gameObject.SetActive(false);
        Debug.Log("disable");
    
    }
}