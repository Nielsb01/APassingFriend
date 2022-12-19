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

    private void Update()
    {
        var particleDir = _particleSystem.velocityOverLifetime;
        particleDir.x = -transform.forward.x;
        particleDir.z = -transform.forward.z;
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
