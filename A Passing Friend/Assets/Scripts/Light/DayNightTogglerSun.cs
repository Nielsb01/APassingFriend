using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DayNightTogglerSun : MonoBehaviour, IDataPersistence
{
    [SerializeField] private float _dayRotation; //30
    [SerializeField] private float _nightRotation; //-65

    public void LoadData(GameData gameData)
    {
        if (gameData.isDay)
        {
            gameObject.transform.rotation = Quaternion.Euler(_dayRotation, 90f, gameObject.transform.rotation.z);
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(_nightRotation, 90f, gameObject.transform.rotation.z);
        }
    }

    public void SaveData(ref GameData gameData) {}
}
