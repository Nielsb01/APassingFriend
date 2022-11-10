#region

using System;
using System.Linq;
using UnityEngine;

#endregion

public class LightCheckScript : MonoBehaviour
{
    private const int INITIAL_LIGHTLEVEL_NIGHTTIME = 4;
    private const int INITIAL_LIGHTLEVEL_DAYTIME = 6;
    [SerializeField] private bool _dayTime = true;

    [HideInInspector] public int lightLevel;

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
        var lightLevels = new float[5];
        lightLevels[0] = GetPixelsFromTexture(_topLightCheckTexture);
        lightLevels[1] = GetPixelsFromTexture(_leftLightCheckTexture);
        lightLevels[2] = GetPixelsFromTexture(_rightLightCheckTexture);
        lightLevels[3] = GetPixelsFromTexture(_forwardLightCheckTexture);
        lightLevels[4] = GetPixelsFromTexture(_backwardLightCheckTexture);

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

        var totalLuminance = 0f;
        foreach (var pixel in pixels)
        {
            // https://en.wikipedia.org/wiki/Relative_luminance#:~:text=Relative%20luminance%20and%20%22gamma%20encoded%22%20colorspaces%5Bedit%5D
            totalLuminance += 0.2126f * pixel.r + 0.7152f * pixel.g + 0.0722f * pixel.b;
        }

        return totalLuminance / pixels.Length;
    }
}