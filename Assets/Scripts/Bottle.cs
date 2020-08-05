using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bottle : MonoBehaviour
{
    public float level;

    public Sprite emptySprite;
    public Sprite fullSprite;

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
