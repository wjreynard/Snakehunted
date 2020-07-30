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
    public Material gameboyMaterial;

    private void OnEnable()
    {
        var camera = GetComponent<Camera>();
        int height = 640;
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
        // first pass is to blit the Source (render image without effects) into our downscaled texture using our posterize-n-colorize shader. And then we copy this downscaled texture into a fullscreen texture as OnRenderImage function logic requires us.
        Graphics.Blit(src, _downscaledRenderTexture, gameboyMaterial);
        Graphics.Blit(_downscaledRenderTexture, dst, identityMaterial);
    }
}
