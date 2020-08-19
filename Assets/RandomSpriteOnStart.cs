using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class RandomSpriteOnStart : MonoBehaviour
{
    public Sprite[] sprites;

    void Start()
    {
        int r = Mathf.FloorToInt(Random.Range(0, sprites.Length));
        Debug.Log("RandomSpriteOnStart, r = " + r);
        if (r >= sprites.Length) r -= 1;
        GetComponent<SpriteRenderer>().sprite = sprites[r];
    }
}
