using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private Inventory inventory;
    public GameObject itemSprite;

    private void Awake()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("");
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Space))
        {
            // find empty slot
            for (int i = 0; i < inventory.slots.Length; i++)
            {
                if (inventory.isFull[i] == false)
                {
                    inventory.isFull[i] = true;
                    Instantiate(itemSprite, inventory.slots[i].transform, false);
                    Destroy(gameObject);    // remove item from screen
                    break;
                }
            }
        }
    }
}
