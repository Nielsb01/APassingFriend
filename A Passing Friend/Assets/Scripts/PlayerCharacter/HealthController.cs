#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class HealthController : MonoBehaviour
{
    [SerializeField] private GameObject _lightCheckController;
    [SerializeField] private GameObject _catBones;
    [SerializeField] private Image vignette;
    [SerializeField] private ParticleSystem _damageParticleSystem;
    [SerializeField] private ParticleSystem _dyingParticleSystem;
    [SerializeField] private int _maxDamageParticleEmission = 125;
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private float _minHealth;
    [SerializeField] private int _lightToDamageThreshold = 3;
    [SerializeField] private float _damagePerFrame = 1;
    [SerializeField] private int _lightToHealingThreshold;
    [SerializeField] private float _regenerationMultiplier = 5;

    private LightCheckScript _lightCheckScript;
    private bool _calculateLight;
    private int _lightLevel;
    private float _health;
    private bool _isDead;
    private bool _isParticling;

    private void Awake()
    {
        _lightCheckScript = _lightCheckController.GetComponent<LightCheckScript>();
        _health = _maxHealth;
    }

    private void Start()
    {
        _calculateLight = _lightCheckScript.calculateLight;
    }

    private void Update()
    {
        if (!_calculateLight) return;

        _lightLevel = _lightCheckScript.lightLevel;
    }

    private void FixedUpdate()
    {
        if (!_calculateLight || _isDead) return;

        UpdateVignette();

        if (_lightLevel >= _lightToDamageThreshold)
        {
            TakeDamage();
        }
        else
        {
            _isParticling = false;
            _damageParticleSystem.Stop();

            if (_lightLevel <= _lightToHealingThreshold)
            {
                Heal();
            }
        }

#if DEBUG
        logHealth();
        logLuminance();
#endif
    }

    private void TakeDamage()
    {
        if (!_isDead)
        {
            _health -= _damagePerFrame;
            CreateDamageParticles();
        }

        if (_health < 0)
        {
            _isDead = true;
            _health = _minHealth;
        }

        if (_isDead)
        {
            _damageParticleSystem.Stop();
            Die();
        }
    }

    private void Heal()
    {
        _health += _regenerationMultiplier;
        _health = _health > _maxHealth ? _maxHealth : _health;
    }

    private void Die()
    {
        transform.GetComponent<BoxCollider>().enabled = false;
        transform.GetComponent<CharacterController>().enabled = false;
        // Set model inactive
        transform.GetChild(0).gameObject.SetActive(false);

        FindObjectOfType<CharacterMovementScript>().enabled = false;

        _dyingParticleSystem.Play();

        Instantiate(_catBones, transform.position, transform.rotation);
    }

    private void UpdateVignette()
    {
        vignette.color = new Color(vignette.color.r, vignette.color.g, vignette.color.b, 1f - _health / 100f);
    }

    private void CreateDamageParticles()
    {
        if (!_isParticling)
        {
            _damageParticleSystem.Play();
            _isParticling = true;
        }

        ChangeParticleEmission(_maxDamageParticleEmission - _health);
    }

    private void ChangeParticleEmission(float emission)
    {
        var particleSystemEmission = _damageParticleSystem.emission;
        particleSystemEmission.rateOverTime = emission;
    }

#if DEBUG
    private int _previousHigh;
    private int _previousLightLevel;
    private int _highestLevel;
    private float _previousHealth;
    private float _previousDamage;

    private void logHealth()
    {
        if (_health == _previousHealth) return;
        _previousHealth = _health;
        Debug.Log("Health: " + _health);
        if (_health == 0)
        {
            Debug.Log("I DIED");
            Debug.Log("D:");
        }
    }

    private void logDamageMultiplier(float damage)
    {
        if (damage == _previousDamage) return;
        _previousDamage = damage;
        Debug.Log("Damage: " + damage);
    }

    private void logHighestLuminance()
    {
        if (_lightLevel <= _highestLevel) return;
        _highestLevel = _lightLevel;
        _previousHigh = _highestLevel;
        Debug.Log("Light: " + _highestLevel);
    }

    private void logLuminance()
    {
        if (_lightLevel == _previousLightLevel) return;
        _previousLightLevel = _lightLevel;
        Debug.Log("LightLevel: " + _lightLevel);
    }
#endif
}