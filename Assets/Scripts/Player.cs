using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    public Animator animator;
    public Transform cameraTransform;

    public float moveSpeed = 6.0f;

    int counter = 0;
    int counterInterval = 100;

    public GameObject footprint;
    public Transform FootprintSpawn;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
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
        }

        // footprints
        counter = (counter + 1) % counterInterval;
        if (counter == counterInterval - 1)
            SpawnDecal(footprint, new Vector3(-0.18f, 0, 0));
        else if (counter == (counterInterval / 2) - 1)
        {
            SpawnDecal(footprint, new Vector3(0.18f, 0, 0));
        }
    }

    void SpawnDecal(GameObject prefab, Vector3 offset)
    {
        Instantiate(prefab, FootprintSpawn.position + offset, Quaternion.identity);
    }
}
