using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightToggler : MonoBehaviour, IDataPersistence
{
    [SerializeField] private GameObject _directionalLight;
    [SerializeField] private float _dayRotation;
    [SerializeField] private float _nightRotation;
    [SerializeField] private List<GameObject> _lights;
    private bool _isDay;
    public bool IsDay
    {
        get => _isDay;
    }

    void Update()
    {
        //TODO Remove 
        if (Input.GetKeyDown(KeyCode.Q))
        {
           ToggleDayNight();
        } 
    }

    private void ToggleDayNight()
    {
        if (!_isDay)
        {
            ToNight();
        }
        else
        {
            ToDay();
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

    public void LoadData(GameData gameData)
    {
        _isDay = gameData.isDay;
        ToggleDayNight();
    }

    public void SaveData(ref GameData gameData) {}
}
