using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bottle : MonoBehaviour
{
    public float level;
    public float maxLevel;
    public float fillRate;
    public float drainRate;

    public Sprite emptySprite;
    public Sprite fullSprite;

    private void Awake()
    {
        level = maxLevel;
    }

    private void Update()
    {
        if (level <= 0)
        {
            GetComponent<Image>().sprite = emptySprite;
        }
        else
        {
            GetComponent<Image>().sprite = fullSprite;
        }
    }
}
