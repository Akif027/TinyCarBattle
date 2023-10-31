using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;

public class Ammo : MonoBehaviour
{


    private PlayerHealth PH;

    private BoxCollider boxCollider;
    public GameObject bulletImact;
    public void setPView(PlayerHealth p)
    {
        this.PH = p;
      
    }
    private void OnEnable()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
      
       StartCoroutine(EsablethisScript());
        StartCoroutine(DisableAfterDelay());
    }

    private IEnumerator DisableAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(2);

        gameObject.SetActive(false);
    }

    private IEnumerator EsablethisScript()
    {
        // Disable the collider
        

        Debug.Log("disable");
        // Wait for the specified duration
        yield return new WaitForSeconds(0.1f);

        // Enable the collider after the duration has passed
        boxCollider.enabled = true;
    }

 

    private void OnCollisionEnter(Collision collision)
    {
           if (collision.collider.tag == "Enemy")
            {
           

                PlayerHealth otherPlayerH = collision.gameObject.GetComponent<PlayerHealth>();
           
               PH.OtherPlayerHealth = otherPlayerH;

             
             gameObject.SetActive(false);
           }
        GameObject impact = Instantiate(bulletImact, transform.position, Quaternion.identity);
        Destroy(impact, 1);
        gameObject.SetActive(false);
    }


}

