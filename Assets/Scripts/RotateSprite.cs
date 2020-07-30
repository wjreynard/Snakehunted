using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSprite : MonoBehaviour
{
    public GameObject targetObject;

    private void Start()
    {
        //GetComponent<Renderer>().receiveShadows = true;
        //GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    void Update()
    {
        RotateSpriteToCamera();
    }

    void RotateSpriteToCamera()
    {
        Vector3 target = targetObject.transform.position;
        float angle = Mathf.Atan2(target.z, target.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, -angle + 90.0f, 0);
    }
}
