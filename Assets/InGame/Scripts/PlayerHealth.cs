using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{

    public float maxHealth = 100.0f;
    public float currentHealth = 100.0f;
    public float damageRate = 10.0f; // Rate at which health decreases after taking damage.
  
    private bool isTakingDamage = false;

    PhotonView view;
    private void Start()
    {
        view = GetComponent<PhotonView>();
       
        if (view.IsMine)
        {

            UImanager.instance.NameText.text = PhotonNetwork.NickName;
        }
        else
        {
            UImanager.instance.NameText.text = view.Owner.NickName;
        }
        // Set the initial health and max value of the slider.
        currentHealth = maxHealth;
       UImanager.instance.healthSlider.maxValue = maxHealth;
        UImanager.instance.healthSlider.value = currentHealth;
    }



 
    public void TakeDamageRPC(float damageAmount)
    {
        if (!isTakingDamage)
        {
            isTakingDamage = true;
            StartCoroutine(ApplyDamageGradually(damageAmount));
        }
    }

    private IEnumerator ApplyDamageGradually(float damageAmount)
    {
        float timeElapsed = 0.0f;
        float initialHealth = currentHealth;

        while (timeElapsed < damageAmount / damageRate)
        {
            currentHealth = Mathf.Lerp(initialHealth, initialHealth - damageAmount, timeElapsed / (damageAmount / damageRate));
           UImanager.instance.healthSlider.value = currentHealth;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        currentHealth = Mathf.Max(0, initialHealth - damageAmount);
        UImanager.instance.healthSlider.value = currentHealth;
        isTakingDamage = false;

        if (currentHealth <= 0)
        {
            OnPlayerDie();
        }
    }

    public void OnPlayerDie()
    {
        view.RPC("OnPlayerDieRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void OnPlayerDieRPC()
    {
        gameObject.SetActive(false);
    }

}