#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

public class LightCheckScript : MonoBehaviour
{
    [HideInInspector] public int lightLevel;
    public bool calculateLight;
    [SerializeField] private int _initialObjectLightlevel = 44;
    [SerializeField] private List<RenderTexture> _renderTextures;
    private const int MIP_MAP_LEVEL = 6;
    private Texture2D _tmp2DTexture;

    private void Awake()
    {
        // All renderTextures have to be of the same size
        _tmp2DTexture = new Texture2D(_renderTextures.First().width, _renderTextures.First().height);
    }

    private void Update()
    {
        if (!calculateLight) return;

        var lightLevels = new List<float>();

        foreach (var renderTexture in _renderTextures)
        {
            lightLevels.Add(GetLuminanceFromTexture(renderTexture));
        }

        var average = lightLevels.Average();

        lightLevel = (int)Math.Round(average) - _initialObjectLightlevel;

        lightLevel = lightLevel < 0 ? 0 : lightLevel;
    }

    private float GetLuminanceFromTexture(RenderTexture lightCheckTexture)
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

        var pixels = _tmp2DTexture.GetPixels32(MIP_MAP_LEVEL);

        // Wikipedia contributors. (2021, 3 november). Relative luminance. Wikipedia. https://en.wikipedia.org/wiki/Relative_luminance#:~:text=Relative%20luminance%20and%20%22gamma%20encoded%22%20colorspaces%5Bedit%5D
        var totalLuminance = pixels.Sum(pixel => 0.2126f * pixel.r + 0.7152f * pixel.g + 0.0722f * pixel.b);

        return totalLuminance / pixels.Length;
    }
}