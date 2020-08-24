using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    // Static reference to current instance of AudioManager in scene
    public static AudioManager instance;

    private void Awake()
    {
        // We only want there to be one instance of our AudioManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Let AudioManager persist between scenes
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds) {
            // add an AudioSource component and transfer attributes of the Sound to that source
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.playOnAwake = true;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mixerGroup;
        }
    }

    private void Start()
    {
        PlaySound("Music");
    }

    public void PlaySound(string name)
    {
        Debug.Log("Playing sound: " + name);

        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Play();
    }
}
