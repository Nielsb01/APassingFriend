#region

using System.Collections;
using UnityEngine;

#endregion

public class HealthController : MonoBehaviour
{
    [SerializeField] private GameObject _lightCheckController;
    [SerializeField] private DataPersistenceManager _dataPersistenceManager;
    [SerializeField] private GameObject _catBones;
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
    
    public delegate void PlayerEvent();
    public static event PlayerEvent Died;

    public bool IsDead
    {
        get => _isDead;
    }

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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter");
        if (other.gameObject.layer == LayerMask.NameToLayer("Wator"))
        {
            Debug.Log("wator");
            Die();
            // _health -= _damagePerFrame * 2;
            // StartCoroutine(DieByWater());
        }
        // else
        // {
        //     _dyingByWater = false;
        // }
    }
    
    // private void OnTriggerExit(Collider other)
    // {
    //     Debug.Log("Trigger exit");
    //     if (other.gameObject.layer == LayerMask.NameToLayer("Wator"))
    //     {
    //         _onWater = false;
    //     }
    // }
    
    

    // private IEnumerator DieByWater()
    // {
    //     yield return new WaitForEndOfFrame();
    //     _health -= _damagePerFrame * 2;
    // }

    private void FixedUpdate()
    {
        if (!_calculateLight || _isDead) return;

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
            _health = _minHealth;

            _isParticling = false;
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
        Died?.Invoke();
        SetPlayerInactive(true);

        _dyingParticleSystem.Play();
        Instantiate(_catBones, transform.position, transform.rotation);

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3);
        _dataPersistenceManager.LoadGame();
        
        SetPlayerInactive(false);

        _health = _maxHealth;
    }

    private void SetPlayerInactive(bool boolean)
    {
        _isDead = boolean;
        boolean = !boolean;
        
        transform.GetComponent<BoxCollider>().enabled = boolean;
        transform.GetComponent<CharacterController>().enabled = boolean;
        GetComponent<CharacterMovementScript>().enabled = boolean;
        // Set model inactive
        transform.GetChild(0).gameObject.SetActive(boolean);
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

    public float GetVignetteTransparency()
    {
        return 1f - _health / 100f;
    }
}