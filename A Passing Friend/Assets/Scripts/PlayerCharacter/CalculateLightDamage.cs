#region

using System;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class CalculateLightDamage : MonoBehaviour
{
    [SerializeField] private GameObject _lightCheckScriptGameObject;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _minHealth = 0;
    [SerializeField] private float _damagePerFrame = 1;
    [SerializeField] private int _lightToDamageThreshold = 3;
    [SerializeField] private int _regenerationMultiplier = 5;
    [SerializeField] private int _lightToHealingThreshold;

    private bool _calculateLight;
    private int _health;
    private LightCheckScript _lightCheckScript;
    private int _lightLevel;
    private float _minDamage = 0;
    public Image vignette;

    private void Awake()
    {
        _lightCheckScript = _lightCheckScriptGameObject.GetComponent<LightCheckScript>();
        _health = _maxHealth;
    }

    private void Start()
    {
        _calculateLight = _lightCheckScript.calculateLight;
    }

    public void Update()
    {
        if (!_calculateLight) return;

        _lightLevel = _lightCheckScript.lightLevel;
    }

    private void FixedUpdate()
    {
        if (!_calculateLight) return;

        vignette.color = new Color(vignette.color.r, vignette.color.g, vignette.color.b, 1f - _health / 100f);

        if (_lightLevel >= _lightToDamageThreshold)
        {
            _health -= (int)Math.Round(_damagePerFrame);
            _health = _health < _minHealth ? _minHealth : _health;
        }

        if (_lightLevel == _lightToHealingThreshold)
        {
            _health += _regenerationMultiplier;
            _health = _health > _maxHealth ? _maxHealth : _health;
        }

#if DEBUG
        logHealth();
        logLuminance();
#endif
    }

#if DEBUG
    private int _previousHigh;
    private int _previousLightLevel;
    private int _highestLevel;
    private int _previousHealth;
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