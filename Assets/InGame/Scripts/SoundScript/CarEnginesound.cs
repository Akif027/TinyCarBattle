using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering;
using System;
using Photon.Realtime;

public class CarEnginesound : MonoBehaviour
{
    public AudioSource engineAudioSource;
    public AudioClip[] clips;
    public Rigidbody carRigidbody;
    public float minSpeed = 0.0f;
    public float maxSpeed = 100.0f;
    public float pitchMultiplier = 0.1f;
    public float volumeMultiplier = 0.1f;
    public float audibleRange = 20f;
    private bool engineStarted = false;
    PhotonView view;



    public float collisionForceThreshold = 5.0f; // Adjust this value as needed
    public float maxVolume = 1.0f; // Maximum volume for the collision sound

    public float audiablMinDistance =0;
    public float audiablMaxDistance =10;
    public Sound[] sounds;

    private void Awake()
    {
      
        view = GetComponent<PhotonView>();
        foreach (Sound s in sounds)
        {

            s.source = gameObject.AddComponent<AudioSource>();
            s.source.spatialBlend = 1;
            s.source.rolloffMode = AudioRolloffMode.Linear;
            s.source.minDistance = audiablMinDistance;
            s.source.maxDistance = audiablMaxDistance;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    private void Start()
    {
       
        if (!view.IsMine)
            return;
        StartCoroutine(PlayStartupSoundAndSwitchToEngine());
    }

    IEnumerator PlayStartupSoundAndSwitchToEngine()
    {
        engineAudioSource.clip = clips[0]; // Set the startup clip
        engineAudioSource.Play();

        yield return new WaitForSeconds(1.5f); // Wait for 2 seconds

        engineAudioSource.clip = clips[1]; // Switch to the engine clip
        engineAudioSource.loop = true;
        engineAudioSource.Play();

        engineStarted = true;
    }



    private void Update()
    {
        if (engineStarted && view.IsMine)
        {
            float currentSpeed = carRigidbody.velocity.magnitude;
            float pitch = Mathf.Lerp(1.0f, 2.0f, (currentSpeed - minSpeed) / (maxSpeed - minSpeed));
            float volume = Mathf.Lerp(0.2f, 1.0f, (currentSpeed - minSpeed) / (maxSpeed - minSpeed));

            engineAudioSource.volume = volume * volumeMultiplier;
            engineAudioSource.pitch = pitch * pitchMultiplier;

         
        
        }


    }

    [PunRPC]
    private void PlayCollsionSoundRPC(float collisionForce)//for collsion sound 
    {
        Debug.Log("collsion sound play ");
        string name = "Collision";

    

        // Find the Sound object
        Sound s = Array.Find(sounds, sound => sound.name == name);

        // Check if the sound is found
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found");
            return;
        }

        // Set the new clip and settings
        s.source.clip = s.clip;
        s.source.pitch = s.pitch;
        s.source.volume = s.volume;

      
        // Play the sound
        s.source.Play();

    }


    [PunRPC]
    private void PlayBasicSound(string name)// for genral sound usage 
    {

     
        // Find the Sound object
        Sound s = Array.Find(sounds, sound => sound.name == name);

        // Check if the sound is found
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found");
            return;
        }

        // Set the new clip and settings
        s.source.clip = s.clip;
        s.source.pitch = s.pitch;
        s.source.volume = s.volume;
        s.source.loop = s.loop;
        // Stop the current playback (if any)

        if (!s.source.isPlaying)
        {
            s.source.Play();
            Debug.Log(name + " Playing");


        }
        // Play the sound

    }

    [PunRPC]
    private void StopSoundRPC(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found");
            return;
        }

        s.source.Stop();
    }

    public void StopSound(string name)
    {
        if (!view.IsMine)
            return;
        view.RPC("StopSoundRPC", RpcTarget.All, name);
    }



    // This is a placeholder method; you should modify this according to your criteria for determining the sound index
    public void PlayCollsionSound(float collisionForce )
    {
        if (!view.IsMine)
            return;

        // Call the RPC to play the collision sound for all players
        view.RPC("PlayCollsionSoundRPC", RpcTarget.All, collisionForce);
    }

    public void PlayskidSound()
    {
        if (!view.IsMine)
            return;
        Debug.Log("Drifting is getting called ");
        view.RPC("PlayBasicSound", RpcTarget.All, "Drift");
    }

    //Weapon sound function 
    public void miniGunshots()
    {
        if (!view.IsMine)
            return;
        view.RPC("PlayBasicSound", RpcTarget.All, "MachineGun");
       
    }

    //this is im sycning over the weaponSysmtem function
    public void SimpleGunshotPlay(string name)
    {
        if (!view.IsMine)
            return;
        // Find the Sound object
        Sound s = Array.Find(sounds, sound => sound.name == name);

        // Check if the sound is found
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found");
            return;
        }

        // Set the new clip and settings
        s.source.clip = s.clip;
        s.source.pitch = s.pitch;
        s.source.volume = s.volume;

        // Stop the current playback (if any)
        s.source.Stop();

        // Play the sound
        s.source.Play();
    }

    public void BulletImpact()
    {
        if (view.IsMine)
        {
            string name = "Pickup";
            Sound s = Array.Find(sounds, sound => sound.name == name);

            s.source.clip =clips[2];
         


            if (s == null)
            {
                Debug.LogWarning("Sound " + name + " not found");
                return;
            }


            Debug.Log("collsion play ");
            s.source.Play();

        }


    }
    public void PickUpsound()
    {
        if (view.IsMine)
        {
            string name = "Pickup";
            Sound s = Array.Find(sounds, sound => sound.name == name);

            s.source.clip = s.clip;
            s.source.pitch = s.pitch;



            if (s == null)
            {
                Debug.LogWarning("Sound " + name + " not found");
                return;
            }


            Debug.Log("collsion play ");
            s.source.Play();

        }


    }


    private void OnDisable()
    {
        foreach (Sound item in sounds)
        {
            item.source.Stop();
        }
    }
}

