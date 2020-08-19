using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Slot : MonoBehaviour
{
    private Player player;
    private Inventory inventory;

    public GameObject emptyBottleObject;
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
                Bottle bottle = child.GetComponent<Bottle>();

                if (bottle.level <= 0.0f)
                {
                    Instantiate(emptyBottleObject, player.transform.position, Quaternion.identity);
                }
                else if (bottle.level > 0.0f)
                {
                    Instantiate(bottleObject, player.transform.position, Quaternion.identity);
                }

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
        Debug.Log("Slot::UseItem()");

        foreach (Transform child in transform)
        {
            if (child.CompareTag("Bottle"))
            {
                Bottle bottle = child.GetComponent<Bottle>();

                if (bottle.level < bottle.maxLevel && player.bCouldRefill)
                {
                    Debug.Log("refill water");

                    player.moveSpeed = 0.0f;
                    // bCanMove = false?

                    player.animator.SetBool("Refilling", true);
                    bottle.level += bottle.fillRate * Time.deltaTime;

                    if (bottle.level >= bottle.maxLevel)
                    {
                        bottle.level = bottle.maxLevel;
                        player.animator.SetBool("Refilling", false);
                        player.moveSpeed = player.midSpeed;
                        player.footprintCounterInterval = player.midFootprintInterval;
                        return;
                    }
                }
                else if (bottle.level <= 0 && !player.bCouldRefill)
                {
                    Debug.Log("empty water");

                    player.moveSpeed = player.slowSpeed;
                    player.footprintCounterInterval = player.slowFootprintInterval;

                    bottle.level = 0.0f;
                    player.animator.SetBool("WaterEmpty", true);
                }
                else if (bottle.level > 0 && !player.bCouldRefill)
                {
                    Debug.Log("drink water");

                    player.moveSpeed = player.slowSpeed;
                    player.footprintCounterInterval = player.slowFootprintInterval;

                    bottle.level -= bottle.drainRate * Time.deltaTime;

                    if (player.thirst > 0)
                    {
                        player.thirst -= player.hydrationRate * Time.deltaTime;
                    }

                    player.animator.SetBool("Drinking", true);
                }
            }
            else if (child.CompareTag("Berry"))
            {
                if (!player.bSprinting)
                {
                    // increase movement speed
                    player.Sprint();
                }

                // play eating sound
                //...

                // remove berry
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    public void StopUsingItem()
    {
        //player.animator.speed = 1.0f;
        //player.moveSpeed = 6.0f;
        //player.footprintCounterInterval = 25;

        player.animator.SetBool("Drinking", false);
        player.animator.SetBool("WaterEmpty", false);
        player.animator.SetBool("Refilling", false);
    }
}
