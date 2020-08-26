using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// code adapted from: https://gist.github.com/Laumania/abc0c08715974b7e23ec9e5627401be1
public class BreathParticleSound : MonoBehaviour
{
    private ParticleSystem _parentParticleSystem;
    private int _currentNumberOfParticles = 0;
    public AudioManager audioManager_Effects;
    public Player player;

    private void Awake()
    {
        _parentParticleSystem = this.GetComponent<ParticleSystem>();
        if (_parentParticleSystem == null)
            Debug.LogError("Missing ParticleSystem!", this);
    }

    private void Update()
    {
        var amount = Mathf.Abs(_currentNumberOfParticles - _parentParticleSystem.particleCount);


        if (_parentParticleSystem.particleCount > _currentNumberOfParticles)
        {
            // play sound on spawn
            player.PlayStateSound(false);
        }

        _currentNumberOfParticles = _parentParticleSystem.particleCount;
    }
}
