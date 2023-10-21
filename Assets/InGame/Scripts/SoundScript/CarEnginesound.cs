using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEnginesound : MonoBehaviour
{
    public AudioSource engineAudioSource;
    public AudioClip[] clips;
    public Rigidbody carRigidbody;
    public float minSpeed = 0.0f;
    public float maxSpeed = 100.0f;
    public float pitchMultiplier = 0.1f;
    public float volumeMultiplier = 0.1f;

    private bool engineStarted = false;

    private void Start()
    {
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
        if (engineStarted)
        {
            float currentSpeed = carRigidbody.velocity.magnitude;
            float pitch = Mathf.Lerp(1.0f, 2.0f, (currentSpeed - minSpeed) / (maxSpeed - minSpeed));
            float volume = Mathf.Lerp(0.2f, 1.0f, (currentSpeed - minSpeed) / (maxSpeed - minSpeed));

            engineAudioSource.volume = volume * volumeMultiplier;
            engineAudioSource.pitch = pitch * pitchMultiplier;
        }
    }
}
