
using Photon.Pun;
using UnityEngine;


public class PlayerHealth :  MonoBehaviourPun
{

    public float maxHealth = 100.0f;
    public float currentHealth = 100.0f;
    public float damageRate = 10.0f; // Rate at which health decreases after taking damage.
    public int killCount = 0;

    public bool isAlive ;

  
    private bool shouldIncrementKill = false;

    private PlayerHealth otherPlayerH =null; // other playerHealth to incremecnt kill count after health is zero 

    //private bool isTakingDamage = false;

    public PhotonView view;

   
    public GameObject ExplosionVFX;
  
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

    private void Awake()
    {

        view = GetComponent<PhotonView>();

        ObjectPool.Instance.CreatePool(ExplosionVFX, 1);
    }



    private void Start()
    {

        OnPlayerRespawn();

        if (view.IsMine)
        {

            // Check if PhotonNetwork.NickName is null or empty
            if (string.IsNullOrEmpty(PhotonNetwork.NickName))
            {
                // If null or empty, generate a random integer as a nickname
                int randomNickname = Random.Range(1000, 9999);
                PhotonNetwork.NickName = "Player" + randomNickname.ToString();
            }

            // Update the UI with the player's nickname
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


    private void Update()
    {
        if (view.IsMine)
        {
            GameManager.Instance.LocalPlayerKills = killCount;
            // Check if otherPlayerH is not null and isPlayerAlive
            if (otherPlayerH != null && !otherPlayerH.isPlayerAlive)
            {
                // Check if ShouldkillInc is true
                if (otherPlayerH.ShouldkillInc)
                {
                    int playerID = otherPlayerH.view.Owner.ActorNumber;

                    killCount++;
                    UImanager.instance.killText.text = killCount.ToString();
                    otherPlayerH.ShouldkillInc = false;
                    GameManager.Instance.OnPlayerDeath(playerID);
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

        if (currentHealth<=0)
        {
            OnPlayerDie();
        }
    }
  
    private void healthBuff(float heatlh)
    {

        // Subtract damage from health
        currentHealth = heatlh;

        // Update UI
        UImanager.instance.healthSlider.value = currentHealth;


      
    }
/*    private void OnDisable()
    {
       
    }*/
    public void OnPlayerDie()
    {
   
        view.RPC("OnPlayerDieRPC", RpcTarget.AllBuffered);
       
    }
  
    [PunRPC]
    private void OnPlayerDieRPC()
    {

   
          GameObject Explosion = ObjectPool.Instance.GetPooledObject(ExplosionVFX);
        if (Explosion != null)
        {

            Explosion.transform.position = transform.position;
            Explosion.transform.rotation = transform.rotation;

        }
   
        shouldIncrementKill = true;
        isAlive = false;    
        gameObject.SetActive(false);
    }

    [PunRPC]
    private void OnPlayerEnable() //start
    {


        GameManager.Instance.InitializePlayerStats(view.Owner);
        gameObject.SetActive(true);
        shouldIncrementKill = false;
        isAlive = true;
        CarEnginesound sounds = GetComponentInChildren<CarEnginesound>();
        foreach (Sound s in sounds.sounds)
        {
            s.source.Stop();
        }
        if (view.IsMine)
        {
            healthBuff(100);

            // gameObject.transform.position =new Vector3(StartPostion.x,StartPostion.y +5,StartPostion.z);

        }
      
    }
    public void OnPlayerRespawn()
    {

        view.RPC("OnPlayerEnable", RpcTarget.AllBuffered);

    }
}