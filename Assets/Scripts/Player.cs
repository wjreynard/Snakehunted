﻿using System;
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

    [Header("Footprints")]
    public GameObject footprint;
    public Transform FootprintSpawn;
    private int footprintCounter = 0;
    private int footprintCounterInterval = 20;

    [Space(10)]
    [Header("Stats")]
    public float maxHealth;
    public float maxThirst;
    public float thirstRate;
    private float health, thirst;



    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inventory = GetComponent<Inventory>();
    }

    private void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        // thirst increase
        if (!bDead)
        {
            thirst += thirstRate * Time.deltaTime;
        }

        if (thirst >= maxThirst)
        {
            Die();
        }

        UseInventory();

        if (Input.GetKeyDown(KeyCode.R))
        {
            //inventory.slots[0].GetComponent<Slot>().DropItem();
            //GetComponent<BottleSpawn>().SpawnDroppedItem();
        }



        UpdateInventorySlot(Input.mouseScrollDelta.y);
    }

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

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Pickup"))
        //{
        //    pickupMessage.SetActive(true);
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Pickup"))
        //{
        //    pickupMessage.SetActive(false);
        //}
    }

    private void UseInventory()
    {
        // key inputs
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.slots[0].GetComponent<Slot>().DropItem();
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventory.slots[1].GetComponent<Slot>().DropItem();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            inventory.slots[2].GetComponent<Slot>().DropItem();
        }

    }

    private void Die()
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
