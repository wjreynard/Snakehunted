using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Slot : MonoBehaviour
{
    private GameObject player;
    private Inventory inventory;

    public int slotPosition;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
    }

    private void Update()
    {
        if (transform.childCount <= 0)
        {
            inventory.isFull[slotPosition] = false;
        }
    }

    public void DropItem()
    {
        // destroy sprite
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

    }
}
