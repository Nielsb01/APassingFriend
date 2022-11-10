#region

using UnityEngine;

#endregion

public class CalculateLightDamage : MonoBehaviour
{
    [SerializeField] private GameObject _lightCheckScriptGameObject;

    [SerializeField] private int _maxHealth = 100;

    [SerializeField] private int _damageMultiplier = 1;

    [SerializeField] private int _regenerationMultiplier = 5;

    [SerializeField] private int _lightToDamageThreshold = 3;

    [SerializeField] private int _lightToHealingThreshold;

    private LightCheckScript _lightCheckScript;
    private float _health;
    private int _lightLevel;
    private float _timer;

    private void Awake()
    {
        _lightCheckScript = _lightCheckScriptGameObject.GetComponent<LightCheckScript>();
        _health = _maxHealth;
    }

    public void Update()
    {
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
        if (_lightLevel >= _lightToDamageThreshold)
        {
            var damage = _lightLevel * _damageMultiplier * _timer / 5;

            logDamageMultiplier(damage);

            _health -= damage;
        }

        if (_lightLevel == _lightToHealingThreshold)
        {
            _health += _regenerationMultiplier;
            if (_health > _maxHealth) _health = _maxHealth;
        }
        
        logHealth();
    }

#if DEBUG
    private int previousHigh;
    private int previousLightLevel;
    private int highestLevel;
    private float previousHealth;
    private float previousDamage;
#endif

#if DEBUG
    private void logHealth()
    {
        if (_health < 0)
        {
            _health = 0;
        }

        if (_health != previousHealth)
        {
            previousHealth = _health;
            Debug.Log("Health: " + _health);
        }
    }

    private void logDamageMultiplier(float damage)
    {
        if (damage < 0)
        {
            damage = 0;
        }

        if (damage != previousDamage)
        {
            previousDamage = damage;
            Debug.Log("Damage: " + damage);
        }
    }

    private void logHighestLuminance()
    {
        if (_lightLevel > highestLevel)
        {
            highestLevel = _lightLevel;
            previousHigh = highestLevel;
            Debug.Log("Light: " + highestLevel);
        }
    }

    private void logLuminance()
    {
        if (_lightLevel != previousLightLevel)
        {
            previousLightLevel = _lightLevel;
            Debug.Log("LightLevel: " + _lightLevel);
        }
    }
#endif
}