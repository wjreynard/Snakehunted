using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimationOnStart : MonoBehaviour
{
    public int[] animations;
    void Start()
    {
        int r = Mathf.FloorToInt(Random.Range(0, animations.Length));
        Debug.Log("RandomSpriteOnStart, r = " + r);
        if (r >= animations.Length) r -= 1; // safe guard
        GetComponent<Animator>().SetInteger("Seed", r);
    }
}
