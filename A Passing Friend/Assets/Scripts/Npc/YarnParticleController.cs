using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnParticleController : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void StartParticle()
    {
        _particleSystem.Play();
    }

    public void StopParticle()
    {
        _particleSystem.Stop();
    }
}
