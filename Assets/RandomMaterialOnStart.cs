using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterialOnStart : MonoBehaviour
{
    public Material[] materials;

    void Start()
    {
        int r = Mathf.FloorToInt(Random.Range(0, materials.Length));
        //Debug.Log("RandomMaterialOnStart, r = " + r);
        if (r >= materials.Length) r -= 1;
        GetComponent<MeshRenderer>().material = materials[r];
    }
}
