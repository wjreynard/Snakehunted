using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    public Animator animator;
    public Transform cameraTransform;

    public float moveSpeed = 6.0f;

    int counter = 0;
    int counterInterval = 80;
    public GameObject footprint;
    public Transform FootprintSpawn;

    private bool bDrinking = false;


    [Header("Stats")]
    public float stat_hydration_level = 50;

    [Space(10)]
    public float stat_water_level = 50;
    public float stat_water_min = 0;
    public float stat_water_max = 100;

    [Header("UI")]
    public Image ui_water_level;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    IEnumerator WaitCoroutine(float t)
    {
        yield return new WaitForSeconds(t);
    }

    void FixedUpdate()
    {

        if (Input.GetKey(KeyCode.Space))
        {
            moveSpeed = 1.0f;

            if (stat_water_level <= 0)
            {
                Debug.Log("Water empty");
                animator.SetBool("Drinking", false);
                animator.SetBool("WaterEmpty", true);
                bDrinking = false;
            }
            else
            {
                bDrinking = true;
                animator.SetBool("Drinking", true);
            }
        }
        else
        {
            bDrinking = false;
            moveSpeed = 6.0f;
            animator.SetBool("Drinking", false);
        }
        //animator.SetBool("WaterEmpty", false);

        if (bDrinking)
        {
            HydratePlayer();
        }

        MovePlayer();
    }

    void HydratePlayer()
    {
        //Debug.Log("Player::HydratePlayer()");

        // decrease and increase movement speed by some curve
        // ...

        // change animation
        animator.SetBool("Drinking", true);

        // increase hydration level
        stat_hydration_level += 0.2f;

        // decrease water level
        stat_water_level -= 0.1f;

        // update ui
        float fill = ExtensionMethods.LinearRemap(stat_water_level, stat_water_min, stat_water_max, 0, 1);
        ui_water_level.fillAmount = fill;

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
            counter = (counter + 1) % counterInterval;
            if (counter == counterInterval - 1)
                SpawnDecal(footprint, FootprintSpawn, new Vector3(-0.185f, 0, 0));
            else if (counter == (counterInterval / 2) - 1)
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
