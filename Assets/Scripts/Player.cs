using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    public Animator animator;
    public Transform cameraTransform;
    public float moveSpeed;
    private bool bDead;

    [Header("Inventory")]
    private int selectedSlot = 0;
    public GameObject selectedSlotHighlight;
    public GameObject pickupMessage;
    private Inventory inventory;
    public bool bPickingUpItem;

    [Header("Footprints")]
    public GameObject footprint;
    public Transform FootprintSpawn;
    private int footprintCounter = 0;
    private int footprintCounterInterval = 20;

    [Space(10)]
    [Header("Stats")]
    public float maxHealth;
    public float maxThirst;
    public float midThirst;
    public float thirstRate;
    public float hydrationRate;
    public float health, thirst;



    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inventory = GetComponent<Inventory>();

        health = maxHealth;
        thirst = 0;
    }

    private void Update()
    {
        UpdateStats();

        UpdateInventorySlot(Input.mouseScrollDelta.y);
        UseInventory();
    }

    private void UpdateStats()
    {
        // thirst increase
        if (!bDead)
        {
            thirst += thirstRate * Time.deltaTime;

            if (thirst >= midThirst)
            {
                // sweatparticles = more;
                moveSpeed = 3.0f;
                footprintCounterInterval = 10;
            }
            else if (thirst < midThirst)
            {
                // sweatparticles = less;
                moveSpeed = 6.0f;
                footprintCounterInterval = 20;
            }
        }

        if (thirst >= maxThirst)
        {
            PlayerReset();
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Pickup"))
    //    {
    //        bTriggerStay = true;

    //        foreach (Transform child in other.transform)
    //        {
    //            if (child.CompareTag("Bottle"))
    //            {
    //                itemSprite = bottleSprite;
    //            }
    //            else if (child.CompareTag("Berry"))
    //            {
    //                itemSprite = berrySprite;
    //            }
    //        }

    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Pickup"))
    //    {
    //        bTriggerStay = false;
    //        itemSprite = null;
    //    }
    //}

    void UpdateInventorySlot(float delta)
    {

        if (delta > 0 || delta < 0)
        {
            if (delta > 0)
            {
                selectedSlot = (selectedSlot + 1) % inventory.slots.Length;
            }
            else if (delta < 0)
            {
                selectedSlot = (selectedSlot - 1) % inventory.slots.Length;
                if (selectedSlot <= -1)
                    selectedSlot = inventory.slots.Length - 1;
            }

            float x = Mathf.Lerp(-50.0f, 50.0f, (float) selectedSlot / (float) (inventory.slots.Length - 1));
            selectedSlotHighlight.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, -100.0f, 0);
        }
    }

    private void UseInventory()
    {
        //if (Input.GetKeyDown(KeyCode.E) && bTriggerStay)
        //{
        //    // pickup item
        //    if (itemSprite != null)
        //    {
        //        for (int i = 0; i < inventory.slots.Length; i++)
        //        {
        //            if (inventory.isFull[i] == false)
        //            {
        //                inventory.isFull[i] = true;
        //                Instantiate(itemSprite, inventory.slots[i].transform, false);
        //                //Destroy(gameObject);    // remove item from screen
        //                break;
        //            }
        //        }
        //    }
        //}

        bPickingUpItem = Input.GetKey(KeyCode.E);

        if (Input.GetKey(KeyCode.R))
        {
            if (inventory.isFull[selectedSlot])
            {
                inventory.slots[selectedSlot].GetComponent<Slot>().UseItem();
            }
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            inventory.slots[selectedSlot].GetComponent<Slot>().DropItem();
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            if (inventory.isFull[selectedSlot])
            {
                inventory.slots[selectedSlot].GetComponent<Slot>().StopUsingItem();
            }
        }

    }

    private void PlayerReset()
    {
        bDead = true;
        //Debug.Log("Player::Die()");

    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(inputX, 0.0f, inputY).normalized;

        animator.SetFloat("Velocity", direction.magnitude);

        if (direction.magnitude >= 0.01f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);

            // footprints
            footprintCounter = (footprintCounter + 1) % footprintCounterInterval;
            if (footprintCounter == footprintCounterInterval - 1)
                SpawnDecal(footprint, FootprintSpawn, new Vector3(-0.185f, 0, 0));
            else if (footprintCounter == (footprintCounterInterval / 2) - 1)
            {
                SpawnDecal(footprint, FootprintSpawn, new Vector3(0.185f, 0, 0));
            }
        }

    }

    void SpawnDecal(GameObject prefab, Transform spawn, Vector3 offset)
    {
        Instantiate(prefab, spawn.position + offset, Quaternion.identity);
    }
}
