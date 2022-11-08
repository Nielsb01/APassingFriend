using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCheckScript : MonoBehaviour
{
    public RenderTexture lightCheckTexture;
    public float lightLevel;
    public int lightLevelRounded;

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

        lightLevel = 0;
        foreach (var color in colors)
        {
            lightLevel += (0.2126f * color.r) + (0.7152f * color.g) + (0.0722f * color.b);
        }

        lightLevelRounded = (int)Math.Round(lightLevel);
        
        Debug.Log(lightLevelRounded);
        Debug.Log(lightLevel);
    }
}
