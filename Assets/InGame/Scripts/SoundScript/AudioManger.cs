using System;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManger : MonoBehaviour
{
    public static AudioManger instance;

    public Sound[] sounds;

    private void Awake()
    {
        instance = this;


        foreach (Sound s in sounds)
        {

            s.source = GetComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }


    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            // Debug.LogWarning("fjdsafdsahfdas");
            return;
        }

        if (!s.source.isPlaying)
        {
            //  Debug.Log("playing " + name);
            s.source.Play();
        }
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;

        if (s.source.isPlaying)
            s.source.Stop();
    }
}