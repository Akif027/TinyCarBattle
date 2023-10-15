using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public float disableDelay = 1.0f;
   // public GameObject LaserImpact;

    private void OnEnable()
    {
        StartCoroutine(DisableAfterDelay());
    }
    private IEnumerator DisableAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(disableDelay);

        gameObject.SetActive(false);
    }

}
