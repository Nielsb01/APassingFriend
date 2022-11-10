#region

using UnityEngine;

#endregion

public class CalculateLightDamage : MonoBehaviour
{
    [SerializeField] private GameObject lightCheckScriptGameObject;

    [SerializeField] private int maxHealth = 100;

    [SerializeField] private int damageMultiplier = 1;

    [SerializeField] private int regenerationMultiplier = 5;

    [SerializeField] private int lightToDamageThreshold = 3;

    [SerializeField] private int lightToHealingThreshold;

    private LightCheckScript _lightCheckScript;
    private float _health;
    private int _lightLevel;
    private float _timer;

    private void Awake()
    {
        _lightCheckScript = lightCheckScriptGameObject.GetComponent<LightCheckScript>();
        _health = maxHealth;
    }

    public void Update()
    {
        _lightLevel = _lightCheckScript.lightLevel;

        if (_lightLevel >= lightToDamageThreshold)
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
        if (_lightLevel >= lightToDamageThreshold)
        {
            var damage = _lightLevel * damageMultiplier * _timer / 5;

            logDamageMultiplier(damage);

            _health -= damage;
        }

        if (_lightLevel == lightToHealingThreshold)
        {
            _health += regenerationMultiplier;
            if (_health > maxHealth) _health = maxHealth;
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