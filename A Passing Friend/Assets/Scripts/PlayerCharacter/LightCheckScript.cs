#region

using System;
using System.Linq;
using UnityEngine;

#endregion

public class LightCheckScript : MonoBehaviour
{
    public int INITIAL_OBJECT_LIGHTLEVEL = 4;

    [HideInInspector] public int lightLevel;
    public bool calculateLight;
    [SerializeField] private RenderTexture _leftLightCheckTexture;
    [SerializeField] private RenderTexture _rightLightCheckTexture;
    [SerializeField] private RenderTexture _backwardLightCheckTexture;
    [SerializeField] private RenderTexture _forwardLightCheckTexture;

    private Texture2D _tmp2DTexture;

    private void Awake()
    {
        // All renderTextures have to be of the same size
        _tmp2DTexture = new(_leftLightCheckTexture.width, _leftLightCheckTexture.height);
    }

    private void Update()
    {
        if (!calculateLight) return;

        var lightLevels = new float[] 
        {
            GetPixelsFromTexture(_leftLightCheckTexture),
            GetPixelsFromTexture(_rightLightCheckTexture),
            GetPixelsFromTexture(_forwardLightCheckTexture),
            GetPixelsFromTexture(_backwardLightCheckTexture)
        };

        var average = lightLevels.Average();

        lightLevel = (int)Math.Round(average) - INITIAL_OBJECT_LIGHTLEVEL;

        lightLevel = lightLevel < 0 ? 0 : lightLevel;
    }

    private float GetPixelsFromTexture(RenderTexture lightCheckTexture)
    {
        var tmpTexture = RenderTexture.GetTemporary(lightCheckTexture.width, lightCheckTexture.height, 0,
            RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(lightCheckTexture, tmpTexture);
        var previous = RenderTexture.active;
        RenderTexture.active = tmpTexture;

        _tmp2DTexture.ReadPixels(new Rect(0, 0, tmpTexture.width, tmpTexture.height), 0, 0);
        _tmp2DTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmpTexture);

        var pixels = _tmp2DTexture.GetPixels32(3);
        
        // Wikipedia contributors. (2021, 3 november). Relative luminance. Wikipedia. https://en.wikipedia.org/wiki/Relative_luminance#:~:text=Relative%20luminance%20and%20%22gamma%20encoded%22%20colorspaces%5Bedit%5D
        var totalLuminance = pixels.Sum(pixel => 0.2126f * pixel.r + 0.7152f * pixel.g + 0.0722f * pixel.b);

        return totalLuminance / pixels.Length;
    }
}