using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateLightDamage : MonoBehaviour
{
    [SerializeField]
    private GameObject LightCheckScriptGameObject;
    [SerializeField]
    private int lightDivisor = 1; 
    
    private LightCheckScript _lightCheckScript;
    private int lightLevel;
    private float timer;
    
    
    //remove this
    private int previousHigh = 0;
    private int previousLightLevel = 0;
    private int highestLevel = 0;
    
    
    private float health = 10000;
    private float previousHealth = 0;
    
    private void Awake()
    {
        _lightCheckScript = LightCheckScriptGameObject.GetComponent<LightCheckScript>();
    }

    public void Update()
    {
        lightLevel = _lightCheckScript.lightLevel;

        if (lightLevel > 0)
        {
            timer += Time.deltaTime;

            
            
            //calculate damage right here
                                     //exponential / linear           = exponential
            var damageMultiplier = lightLevel / (lightDivisor*100000);

            
            // Exponential decay formula
            // var f = (float)Math.Pow(health, (-lightLevel / 2));
            // Debug.Log(Math.Pow(health, (-lightLevel / 2)));
            // Debug.Log("damagemultiplier: " + f);
            
            

            if (damageMultiplier != 0)
            {
                // health -= f;
            
                //       (now)exponential * exponential
                health -= damageMultiplier * timer;
            }
            else
            {
                health += 100;
                if (health > 10000) health = 10000;
            }
        }
        else
        {
            timer = 0;
        }
        
        logHealth();
        // logLuminance();
        
        // Debug.Log(lightLevel);
    }

    private void logHealth()
    {
        if (health < 0) health = 0;
        if (health != previousHealth)
        {
            previousHealth = health;
            Debug.Log("Health: " + health);
        }
    }

    private void logHighestLuminance()
    {
        // highest level so far (point light intensity 30) 2.465.980
        if (lightLevel > highestLevel)
        {
            highestLevel = lightLevel;
            previousHigh = highestLevel;
            Debug.Log("Light: " + highestLevel);
        }
    }

    private void logLuminance()
    {
        if (lightLevel != previousLightLevel)
        {
            previousLightLevel = lightLevel;
            Debug.Log("LightLevel: " + lightLevel);
        }
    }
}
