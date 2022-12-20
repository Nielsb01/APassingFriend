using System.Collections;
using UnityEngine;

public class FogController : MonoBehaviour
{
    [SerializeField] private float _woodsFogDensity = 0.1f;
    [SerializeField] private float _villageFogDensity = 0.03f;
    [SerializeField] private float _fogSwitchingMultiplier = 0.03f;

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
        if (RenderSettings.fogDensity >= endDensityValue)
        {
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
                RenderSettings.fogDensity += _fogSwitchingMultiplier * Time.deltaTime;
                yield return null;
            }
        }
    }
}
