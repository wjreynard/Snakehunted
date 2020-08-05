using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Slot : MonoBehaviour
{
    private GameObject player;
    private Inventory inventory;

    public GameObject bottleObject;

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

        foreach (Transform child in transform)
        {
            // spawn item
            if (child.CompareTag("Bottle"))
            {
                Instantiate(bottleObject, player.transform.position, Quaternion.identity);
            }


            // destroy sprite
            GameObject.Destroy(child.gameObject);
        }

    }
}
