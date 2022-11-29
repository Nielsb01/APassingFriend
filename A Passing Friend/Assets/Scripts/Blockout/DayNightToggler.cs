using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightToggler : MonoBehaviour
{
    [SerializeField] private GameObject _directionalLight;
    [SerializeField] private float _dayRotation;
    [SerializeField] private float _nightRotation;
    [SerializeField] private List<GameObject> _lights;
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
        foreach (GameObject light in _lights)
        {
            light.gameObject.SetActive(true);
        }
        _directionalLight.transform.rotation = Quaternion.Euler(_nightRotation, 90f, _directionalLight.transform.rotation.z);
    }

    private void ToDay()
    {
        foreach(GameObject light in _lights)
        {
            light.gameObject.SetActive(false);
        }
        _directionalLight.transform.rotation = Quaternion.Euler(_dayRotation, 90f, _directionalLight.transform.rotation.z);
    }
}
