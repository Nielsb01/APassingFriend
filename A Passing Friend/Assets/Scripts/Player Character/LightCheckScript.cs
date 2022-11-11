#region

using System;
using System.Linq;
using UnityEngine;

#endregion

public class LightCheckScript : MonoBehaviour
{
    private const int INITIAL_LIGHTLEVEL_NIGHTTIME = 4;
    private const int INITIAL_LIGHTLEVEL_DAYTIME = 6;

    [HideInInspector] public int lightLevel;
    public bool calculateLight;
    [SerializeField] private bool _dayTime = true;
    [SerializeField] private RenderTexture _topLightCheckTexture;
    [SerializeField] private RenderTexture _leftLightCheckTexture;
    [SerializeField] private RenderTexture _rightLightCheckTexture;
    [SerializeField] private RenderTexture _forwardLightCheckTexture;
    [SerializeField] private RenderTexture _backwardLightCheckTexture;

    private int _initialLightlevel;

    private void Awake()
    {
        _initialLightlevel = _dayTime ? INITIAL_LIGHTLEVEL_DAYTIME : INITIAL_LIGHTLEVEL_NIGHTTIME;
    }

    private void Update()
    {
        if (!calculateLight) return;

        var lightLevels = new float[] 
        {
            GetPixelsFromTexture(_topLightCheckTexture),
            GetPixelsFromTexture(_leftLightCheckTexture),
            GetPixelsFromTexture(_rightLightCheckTexture),
            GetPixelsFromTexture(_forwardLightCheckTexture),
            GetPixelsFromTexture(_backwardLightCheckTexture)
        };

        var average = lightLevels.Average();

        lightLevel = (int)Math.Round(average) - _initialLightlevel;
    }

    private float GetPixelsFromTexture(RenderTexture lightCheckTexture)
    {
        var tmpTexture = RenderTexture.GetTemporary(lightCheckTexture.width, lightCheckTexture.height, 0,
            RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(lightCheckTexture, tmpTexture);
        var previous = RenderTexture.active;
        RenderTexture.active = tmpTexture;

        Texture2D tmp2DTexture = new(lightCheckTexture.width, lightCheckTexture.height);
        tmp2DTexture.ReadPixels(new Rect(0, 0, tmpTexture.width, tmpTexture.height), 0, 0);
        tmp2DTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmpTexture);

        var pixels = tmp2DTexture.GetPixels32();
        Destroy(tmp2DTexture);

        // Wikipedia contributors. (2021, 3 november). Relative luminance. Wikipedia. https://en.wikipedia.org/wiki/Relative_luminance#:~:text=Relative%20luminance%20and%20%22gamma%20encoded%22%20colorspaces%5Bedit%5D
        var totalLuminance = pixels.Sum(pixel => 0.2126f * pixel.r + 0.7152f * pixel.g + 0.0722f * pixel.b);

        return totalLuminance / pixels.Length;
    }
}