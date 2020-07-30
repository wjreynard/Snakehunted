using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// code from:
// https://medium.com/@cyrilltoboe/code-first-game-boy-post-processing-shader-for-unity-ef140252fd7d
public class Gameboy : MonoBehaviour
{
    private RenderTexture _downscaledRenderTexture;

    public Material identityMaterial;

    private void OnEnable()
    {
        var camera = GetComponent<Camera>();
        int height = 144;
        int width = Mathf.RoundToInt(camera.aspect * height);
        _downscaledRenderTexture = new RenderTexture(width, height, 16);
        _downscaledRenderTexture.filterMode = FilterMode.Point;
    }

    private void OnDisable()
    {
        Destroy(_downscaledRenderTexture);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, identityMaterial);
    }
}
