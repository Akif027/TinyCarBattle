using System;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManger : MonoBehaviour
{
    public static AudioManger instance;

    public Sound[] sounds;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
       
        foreach (Sound s in sounds)
        {

            s.source = gameObject.GetComponent<AudioSource>();

          
        }
    }

    public void Play(string name)
    {
        Debug.Log(name + "Playing");
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found");
            return;
        }

        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;

        if (!s.source.isPlaying)
        {
            s.source.Play();
        }
       
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found");
            return;
        }

        s.source.Stop();
    }
}