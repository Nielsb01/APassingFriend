using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightToggler : MonoBehaviour
{
    [SerializeField] private GameObject directionalLight;
    [SerializeField] private float dayRotation;
    [SerializeField] private float nightRotation;
    [SerializeField] private List<GameObject> lights;
    private bool _isDay;

    void Start()
    {
        // ensure that level starts in day time
        _isDay = false;
        ToggleDayNight();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
           ToggleDayNight();
        } 
    }

    private void ToggleDayNight()
    {
        if (_isDay)
        {
            ToNight();
            _isDay = false;
        }
        else
        {
            ToDay();
            _isDay = true;
        }
    }

    private void ToNight()
    {
        foreach (GameObject light in lights)
        {
            light.gameObject.SetActive(true);
        }
        directionalLight.transform.rotation = Quaternion.Euler(nightRotation, 90f, directionalLight.transform.rotation.z);
    }

    private void ToDay()
    {
        foreach(GameObject light in lights)
        {
            light.gameObject.SetActive(false);
        }
        directionalLight.transform.rotation = Quaternion.Euler(dayRotation, 90f, directionalLight.transform.rotation.z);
    }
}
