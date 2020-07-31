using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footprint : MonoBehaviour
{
    int age = 0;
    public int lifetime = 400;

    private void Update()
    {
        age++;
        if (age >= lifetime)
            Destroy(this.gameObject);
    }
}
