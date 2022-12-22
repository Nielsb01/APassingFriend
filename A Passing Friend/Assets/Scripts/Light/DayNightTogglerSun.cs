using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DayNightTogglerSun : MonoBehaviour, IDataPersistence
{
    [SerializeField] private float _dayRotation; //30
    [SerializeField] private float _nightRotation; //-65
    [SerializeField] private Material _nightSkyBox;
    [SerializeField] private Material _daySkyBox;

    public void LoadData(GameData gameData)
    {
        if (gameData.isDay)
        {
            gameObject.transform.rotation = Quaternion.Euler(_dayRotation, 90f, gameObject.transform.rotation.z);
            RenderSettings.skybox = _daySkyBox;
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(_nightRotation, 90f, gameObject.transform.rotation.z);
            RenderSettings.skybox = _nightSkyBox;

        }
    }

    public void SaveData(ref GameData gameData) {}
}
