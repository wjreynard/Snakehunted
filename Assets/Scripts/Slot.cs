using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UIElements;

public class Slot : MonoBehaviour
{
    private Player player;
    private Inventory inventory;

    public GameObject emptyBottleObject;
    public GameObject bottleObject;
    public GameObject berryObject;
    public GameObject stoneObject;

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
            else if (child.CompareTag("Stone"))
            {
                Instantiate(stoneObject, player.transform.position + new Vector3(0, -0.35f, 0), Quaternion.identity);
            }

            // destroy sprite
            GameObject.Destroy(child.gameObject);
        }
    }
    public void UseItem()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Bottle"))
            {
                Bottle bottle = child.GetComponent<Bottle>();

                if (bottle.level < bottle.maxLevel && player.bCouldRefill)
                {
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
                    player.moveSpeed = player.slowSpeed;
                    player.footprintCounterInterval = player.slowFootprintInterval;

                    bottle.level = 0.0f;
                    player.animator.SetBool("WaterEmpty", true);
                }
                else if (bottle.level > 0 && !player.bCouldRefill)
                {
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
            else if (child.CompareTag("Stone"))
            {
                Debug.Log("using stone");

                // place stone if ready
                if (player.bPlacingStone)
                {
                    // instantiate stone as child of stoneholder
                    GameObject newItem = Instantiate(player.stonePrefab, player.stoneHolder.position, Quaternion.identity);
                    newItem.transform.parent = player.stoneHolder.gameObject.transform;

                    // trigger end scene
                    player.EndScene();

                    // disable text prompt
                    player.stoneText.SetActive(false);

                    // remove stone
                    GameObject.Destroy(child.gameObject);
                }
                else
                {
                    // do nothing?
                    // play stone shaking animation?
                }

            }
        }
    }

    public IEnumerator ISetAnimationBool(string name)
    {
        player.animator.SetBool(name, true);
        yield return new WaitForSeconds(0.5f);
        player.animator.SetBool(name, false);
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
