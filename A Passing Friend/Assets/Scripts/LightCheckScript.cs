using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCheckScript : MonoBehaviour
{
    public bool dayTime = true;
    [HideInInspector]
    public int lightLevel;
    [SerializeField]
    private RenderTexture lightCheckTexture;
    
    private float exactLightLevel;
    private const int INITIAL_LIGHTLEVEL_NIGHTTIME = 123583;
    private const int INITIAL_LIGHTLEVEL_DAYTIME = 593746;

    private int _initialLightlevel;

    private void Awake()
    {
        _initialLightlevel = dayTime ? INITIAL_LIGHTLEVEL_DAYTIME : INITIAL_LIGHTLEVEL_NIGHTTIME;
    }

    void Update()
    {
        RenderTexture tmpTexture = RenderTexture.GetTemporary(lightCheckTexture.width, lightCheckTexture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(lightCheckTexture, tmpTexture);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmpTexture;

        Texture2D tmp2DTexture = new(lightCheckTexture.width, lightCheckTexture.height);
        tmp2DTexture.ReadPixels(new Rect(0,0,tmpTexture.width,tmpTexture.height),0,0);
        tmp2DTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmpTexture);

        Color32[] colors = tmp2DTexture.GetPixels32();

        exactLightLevel = 0;
        foreach (var color in colors)
        {
            // Luminance formula
            exactLightLevel += (0.2126f * color.r) + (0.7152f * color.g) + (0.0722f * color.b);
        }
        
        lightLevel = (int)Math.Round(exactLightLevel) - _initialLightlevel;
    }
}
