using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public GameObject[] items;

    private void Start()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].GetComponent<Pickup>().index = i;
        }
    }
}
