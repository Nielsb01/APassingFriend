#region

using System.Collections;
using UnityEngine;

#endregion

public class HealthController : MonoBehaviour, IDataPersistence
{
    [Header("General Settings")]
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

    [Header("Sound Settings")]
    [SerializeField] private FMODUnity.EventReference _dyingAudioEvent;
    [SerializeField] private FMODUnity.EventReference _deadAudioEvent;

    private FMOD.Studio.EventInstance? _dyingAudioEventInstance = null;

    public bool IsDead
    {
        get => _isDead;
    }

    private void Awake()
    {
        _lightCheckScript = _lightCheckController.GetComponent<LightCheckScript>();
        _health = _maxHealth;
    }

    public void LoadData(GameData gameData)
    {
        SetPlayerInvisible(!gameData.ashaCutsceneComplete);
    }

    public void SaveData(ref GameData gameData) { }

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
        if (other.gameObject.layer == LayerMask.NameToLayer("Wator"))
        {
            Die();
        }
    }

    private void FixedUpdate()
    {
        if (!_calculateLight || _isDead) return;

        if (_lightLevel >= _lightToDamageThreshold)
        {
            if (_dyingAudioEventInstance.HasValue == false)
            {
                _dyingAudioEventInstance = FMODUnity.RuntimeManager.CreateInstance(_dyingAudioEvent);
                _dyingAudioEventInstance.Value.start();
            }

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

        if (_dyingAudioEventInstance.HasValue)
        {
            _dyingAudioEventInstance.Value.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _dyingAudioEventInstance = null;
        }
    }

    private void Die()
    {
        // stop dying audio sound
        if (_dyingAudioEventInstance.HasValue)
        {
            _dyingAudioEventInstance.Value.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _dyingAudioEventInstance = null;
        }

        Died?.Invoke();
        DisablePlayer(true);

        var audioEvent = FMODUnity.RuntimeManager.CreateInstance(_deadAudioEvent);
        audioEvent.start();

        _dyingParticleSystem.Play();
        Instantiate(_catBones, transform.position, transform.rotation);

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3);
        _dataPersistenceManager.LoadGame();

        DisablePlayer(false);

        _health = _maxHealth;
    }

    private void DisablePlayer(bool state)
    {
        _isDead = state;

        transform.GetComponent<CharacterController>().enabled = !state;
        GetComponent<CharacterMovementScript>().enabled = !state;
        SetPlayerInvisible(state);
    }

    public void SetPlayerInvisible(bool state)
    {
        transform.GetChild(0).gameObject.SetActive(!state);
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