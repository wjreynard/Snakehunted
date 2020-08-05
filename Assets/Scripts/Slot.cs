using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Slot : MonoBehaviour
{
    private Player player;
    private Inventory inventory;

    public GameObject bottleObject;
    public GameObject berryObject;

    public int slotPosition;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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
            // the inventory slot images (BottleImage, BerryImage) have tags that identify which item to spawn
            if (child.CompareTag("Bottle"))
            {
                Instantiate(bottleObject, player.transform.position, Quaternion.identity);
            }
            else if (child.CompareTag("Berry"))
            {
                Instantiate(berryObject, player.transform.position, Quaternion.identity);
            }


            // destroy sprite
            GameObject.Destroy(child.gameObject);
        }

    }

    public void UseItem()
    {
        Debug.Log("Using item");

        foreach (Transform child in transform)
        {
            if (child.CompareTag("Bottle"))
            {
                Bottle bottle = child.GetComponent<Bottle>();

                if (bottle.level <= 0)
                {
                    player.animator.SetTrigger("WaterEmpty");
                }
                else
                {
                    // decrease bottle level
                    bottle.level -= 0.1f;
                    player.thirst -= player.hydrationRate;

                    player.moveSpeed = 1.0f;
                    player.animator.SetBool("Drinking", true);

                    //player.moveSpeed = 6.0f;
                    //player.animator.SetBool("Drinking", false);
                }
            }
            else if (child.CompareTag("Berry"))
            {
                // use berry
            }
        }
    }
}
