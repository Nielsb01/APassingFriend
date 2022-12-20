using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightToggleLamp : MonoBehaviour, IDataPersistence
{
    public void LoadData(GameData gameData)
    {
        gameObject.SetActive(!gameData.isDay);
    }

    public void SaveData(ref GameData gameData)
    {}
}
