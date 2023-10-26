using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public float disableDelay = 5.0f;


    private PlayerHealth PH;

    private SphereCollider boxCollider;

    public void setPView(PlayerHealth p)
    {
        this.PH = p;
      
    }
    private void OnEnable()
    {
        boxCollider = GetComponent<SphereCollider>();
        boxCollider.enabled = false;
        StartCoroutine(DisableAfterDelay());
        StartCoroutine(EsablethisScript());
      
    }

 
    private IEnumerator EsablethisScript()
    {
        // Disable the collider
        

        Debug.Log("disable");
        // Wait for the specified duration
        yield return new WaitForSeconds(0.2f);

        // Enable the collider after the duration has passed
        boxCollider.enabled = true;
    }

    private IEnumerator DisableAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(disableDelay);

        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
           if (collision.collider.tag == "Enemy")
            {
           

                PlayerHealth otherPlayerH = collision.gameObject.GetComponent<PlayerHealth>();
           
               PH.OtherPlayerHealth = otherPlayerH;

          

        }
    }


}

