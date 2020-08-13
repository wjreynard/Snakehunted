using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBob : MonoBehaviour
{
    [SerializeField]
    float height = 0.1f;

    [SerializeField]
    float period = 1;

    private float initialBrightness;

    private void Awake()
    {
        initialBrightness = GetComponent<Light>().range;
    }

    private void Update()
    {
        GetComponent<Light>().range = initialBrightness + Mathf.Sin((Time.time) * period) * height;
    }
}
