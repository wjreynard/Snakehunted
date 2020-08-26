
using UnityEngine;
using UnityEngine.Audio;
using System;

public class NewAudioManager : MonoBehaviour
{
    public NewSound[] sounds;
    
    private void Awake()
    {
        foreach (NewSound s in sounds)
        {
            // add component to this object (could add to child)
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    private void Start()
    {
        PlaySound("Death");
    }

    public void PlaySound(string name)
    {
        NewSound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }
}
