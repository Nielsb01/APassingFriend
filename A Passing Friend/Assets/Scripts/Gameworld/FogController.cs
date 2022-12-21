using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour, IDataPersistence
{
    [SerializeField] private float _woodsFogDensity = 0.1f;
    [SerializeField] private float _villageFogDensity = 0.03f;
    [SerializeField] private float _fogSwitchingMultiplier = 0.03f;
    private bool running;

    public void GoToWoodsFog()
    {
        StartCoroutine(SlowlyUpdateFogDensity(_woodsFogDensity));
    }

    public void GoToVillageFog()
    {
        StartCoroutine(SlowlyUpdateFogDensity(_villageFogDensity));
    }

    private IEnumerator SlowlyUpdateFogDensity(float endDensityValue)
    {
        if (running) yield break;
        running = true;
        
        if (RenderSettings.fogDensity >= endDensityValue)
        {
            Debug.Log("G");
            while (RenderSettings.fogDensity >= endDensityValue)
            {
                RenderSettings.fogDensity -= _fogSwitchingMultiplier * Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            while (RenderSettings.fogDensity <= endDensityValue)
            {
                Debug.Log("F");
                RenderSettings.fogDensity += _fogSwitchingMultiplier * Time.deltaTime;
                yield return null;
            }
        }

        running = false;
    }

    public void LoadData(GameData gameData)
    {
        RenderSettings.fogDensity = gameData.fogDensity;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.fogDensity = RenderSettings.fogDensity;
    }
}
