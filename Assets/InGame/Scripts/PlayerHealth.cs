
using Photon.Pun;

using UnityEngine;


public class PlayerHealth : MonoBehaviour
{

    public float maxHealth = 100.0f;
    public float currentHealth = 100.0f;
    public float damageRate = 10.0f; // Rate at which health decreases after taking damage.
    public int killCount = 0;

    public bool isAlive ;

  
    private bool shouldIncrementKill = false;

    private PlayerHealth otherPlayerH =null; // other playerHealth to incremecnt kill count after health is zero 

    //private bool isTakingDamage = false;

    private PhotonView view;

 

  
    public bool isPlayerAlive
    {
        get { return isAlive;
          
        }
      
    }
    public PlayerHealth OtherPlayerHealth
    {
        get
        {
            return otherPlayerH;

        }

        set
        {
            otherPlayerH = value;

        }

    }

    public bool ShouldkillInc
    {
        get
        {
            return shouldIncrementKill;

        }
        set {

            shouldIncrementKill = value;
        }
    }

    private void OnEnable()
    {

        view = GetComponent<PhotonView>();
        if (!view.IsMine)
            return;

         view.RPC("OnPlayerEnable", RpcTarget.AllBuffered);
    }
    private void Start()
    {
      

        if (view.IsMine)
        {

            UImanager.instance.NameText.text = PhotonNetwork.NickName;
            gameObject.name = PhotonNetwork.NickName;
        }
        else
        {
            UImanager.instance.NameText.text = view.Owner.NickName;
            gameObject.name = view.Owner.NickName;
        }
        // Set the initial health and max value of the slider.
        currentHealth = maxHealth;
       UImanager.instance.healthSlider.maxValue = maxHealth;
        UImanager.instance.healthSlider.value = currentHealth;
    }


    private void Update()
    {
        if (view.IsMine)
        {
            // Check if otherPlayerH is not null and isPlayerAlive
            if (otherPlayerH != null && !otherPlayerH.isPlayerAlive)
            {
                // Check if ShouldkillInc is true
                if (otherPlayerH.ShouldkillInc)
                {
                    killCount++;
                    UImanager.instance.killText.text = killCount.ToString();
                    otherPlayerH.ShouldkillInc = false;
                }
            }
        }

    }

    public void TakeDamage(float damageAmount)
    {

          ApplyDamageDirectly(damageAmount);
      //  view.RPC("ApplyDamageDirectly", RpcTarget.AllBuffered, damageAmount);
   
    }


    public void increasekillCount(bool isDead)
    {
        Debug.Log("isdead " + isDead);

        if (!isDead)
        {
            killCount++;
        }

    }



    private void ApplyDamageDirectly(float damageAmount)
    {
      
            // Subtract damage from health
            currentHealth -= damageAmount;

            // Update UI
            UImanager.instance.healthSlider.value = currentHealth;
    

        // Check if the player has died
        if (currentHealth <= 0 )
        {
            
                OnPlayerDie();
                Debug.Log("Dead");
          
           
        }
    }
    public void OnPlayerDie()
    {
        view.RPC("OnPlayerDieRPC", RpcTarget.AllBuffered);
    }
  
    [PunRPC]
    private void OnPlayerDieRPC()
    {
        shouldIncrementKill = true;
        isAlive = false;    
      gameObject.SetActive(false);
    }

    [PunRPC]
    private void OnPlayerEnable() //start
    {
        shouldIncrementKill = false;
        isAlive = true;
    }
}