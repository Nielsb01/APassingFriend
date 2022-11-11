#region

using UnityEngine;

#endregion

public class CalculateLightDamage : MonoBehaviour
{
    private const int DAMAGE_REDUCTION_FACTOR = 5;

    [SerializeField] private GameObject _lightCheckScriptGameObject;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _damageMultiplier = 1;
    [SerializeField] private int _regenerationMultiplier = 5;
    [SerializeField] private int _lightToDamageThreshold = 3;
    [SerializeField] private int _lightToHealingThreshold;

    private bool _calculateLight;
    private float _health;
    private LightCheckScript _lightCheckScript;
    private int _lightLevel;
    private float _timer;

    private void Awake()
    {
        _lightCheckScript = _lightCheckScriptGameObject.GetComponent<LightCheckScript>();
        _calculateLight = _lightCheckScript.calculateLight;
        _health = _maxHealth;
    }

    public void Update()
    {
        if (!_calculateLight) return;

        _lightLevel = _lightCheckScript.lightLevel;

        if (_lightLevel >= _lightToDamageThreshold)
        {
            _timer += Time.deltaTime;
        }
        else
        {
            _timer = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!_calculateLight) return;

        if (_lightLevel >= _lightToDamageThreshold)
        {
            var damage = _lightLevel * _damageMultiplier * _timer / DAMAGE_REDUCTION_FACTOR;

            _health -= damage;
        }

        if (_lightLevel == _lightToHealingThreshold)
        {
            _health += _regenerationMultiplier;
            if (_health > _maxHealth) _health = _maxHealth;
        }

#if DEBUG
        logHealth();
#endif
    }

#if DEBUG
    private int _previousHigh;
    private int _previousLightLevel;
    private int _highestLevel;
    private float _previousHealth;
    private float _previousDamage;

    private void logHealth()
    {
        if (_health < 0)
        {
            _health = 0;
        }

        if (_health != _previousHealth)
        {
            _previousHealth = _health;
            Debug.Log("Health: " + _health);
        }
    }

    private void logDamageMultiplier(float damage)
    {
        if (damage < 0)
        {
            damage = 0;
        }

        if (damage != _previousDamage)
        {
            _previousDamage = damage;
            Debug.Log("Damage: " + damage);
        }
    }

    private void logHighestLuminance()
    {
        if (_lightLevel > _highestLevel)
        {
            _highestLevel = _lightLevel;
            _previousHigh = _highestLevel;
            Debug.Log("Light: " + _highestLevel);
        }
    }

    private void logLuminance()
    {
        if (_lightLevel != _previousLightLevel)
        {
            _previousLightLevel = _lightLevel;
            Debug.Log("LightLevel: " + _lightLevel);
        }
    }
#endif
}