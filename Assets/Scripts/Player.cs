using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    public Animator animator;
    public Transform cameraTransform;
    public SpriteRenderer playerSprite;
    public Sprite playerFrozenSprite;
    public float moveSpeed;
    private bool bDead;
    public bool bCanMove = false;
    private bool bAlreadyMoved = false;
    public CinemachineBrain cinemachineBrain;
    public CinemachineFreeLook cinemachineFreeLook;
    //public ObjectManager objectManager;
    public LoadScene gameManager;
    public GameObject resetPanel;
    public LightBob lightBob;

    [Header("Effects")]
    public ParticleSystem sweatParticles;
    public ParticleSystem breathParticlesLess;
    public ParticleSystem breathParticlesMore;
    public ParticleSystem snowParticles;

    [Header("Inventory")]
    private int selectedSlot = 0;
    public GameObject selectedSlotHighlight;
    private Inventory inventory;
    public bool bPickingUpItem;
    public bool bCouldRefill;
    public GameObject pickupText;
    public GameObject refillText;

    [Header("Footprints")]
    public GameObject footprint;
    public Transform FootprintSpawn;
    private int footprintCounter = 0;
    private int footprintCounterInterval = 50;

    [Space(10)]
    [Header("Stats")]
    public float maxHealth;
    public float maxThirst;
    public float midThirst;
    public float thirstRate;
    public float hydrationRate;
    public float health, thirst;
    public Image thirstBar;
    private int flashCounter = 0;
    private int flashCounterInterval = 10;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inventory = GetComponent<Inventory>();

        cinemachineBrain.enabled = false;

        health = maxHealth;
        thirst = 0;
    }

    private void Update()
    {
        cinemachineBrain.enabled = bCanMove;

        // triggers once when player movement is enabled
        if (bCanMove && !bAlreadyMoved)
        {
            StartCoroutine(FadeFOV(cinemachineFreeLook, 4.0f, 45.0f));
            bAlreadyMoved = true;
        }

        if (bCanMove)
        {
            UpdateStats();
            SelectInventorySlot(-Input.mouseScrollDelta.y);
            UseInventory();
        }
    }

    void FixedUpdate()
    {
        if (bCanMove)
        {
            MovePlayer();
        }
    }

    public IEnumerator WaitForTime(float t)
    {
        Debug.Log("Started Coroutine at timestamp : " + Time.time);
        yield return new WaitForSeconds(t);
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    public IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(5.0f);
        gameManager.LoadByIndex(0);
    }

    private void PlayerDeath()
    {
        Debug.Log("player dead");

        // freeze player
        bDead = true;
        moveSpeed = 0.0f;
        bCanMove = false;

        // disable all animations, particles, prompts
        animator.SetFloat("Velocity", 0.0f);
        animator.SetBool("Drinking", false);
        animator.SetBool("WaterEmpty", false);
        animator.SetBool("Refilling", false);

        refillText.SetActive(false);
        pickupText.SetActive(false);

        breathParticlesLess.Stop();
        breathParticlesMore.Stop();
        sweatParticles.Stop();
        snowParticles.Pause();

        lightBob.period = 0;

        // change sprite
        animator.speed = 1.0f;
        animator.SetBool("Dead", true);

        // zoom camera in
        StartCoroutine(FadeFOV(cinemachineFreeLook, 2.0f, 35.0f));

        // increase volume of breathing
        // decrease volume of everything else

        StartCoroutine(ResetGame());
    }

    private void UpdateStats()
    {
        if (!bDead)
        {
            ManageThirst();
        }

        if (thirst >= maxThirst)
        {
            PlayerDeath();
        }
    }

    public static IEnumerator FadeFOV(CinemachineFreeLook cam, float duration, float targetFOV)
    {
        Debug.Log("FadeFOV()");

        cam.m_CommonLens = true;

        float currentTime = 0;
        float start = cam.m_Lens.FieldOfView;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            cam.m_Lens.FieldOfView = Mathf.Lerp(start, targetFOV, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    private void ManageThirst()
    {
        thirst += thirstRate * Time.deltaTime;

        if (thirst < 0) thirst = 0.0f;

        float newEmissionRate = ExtensionMethods.LinearRemap(thirst, 0, maxThirst, 0.0f, 10.0f);
        ParticleSystem.EmissionModule pSweatEmission = sweatParticles.emission;
        pSweatEmission.rateOverTime = newEmissionRate;

        float fillAmount = ExtensionMethods.LinearRemap(thirst, 0, maxThirst, 1.0f, 0.0f);
        thirstBar.fillAmount = fillAmount;

        if (thirst >= midThirst)
        {
            moveSpeed = 3.0f;
            footprintCounterInterval = 20;

            // breathless particles
            breathParticlesLess.gameObject.SetActive(false);
            breathParticlesMore.gameObject.SetActive(true);

            // slower animation
            animator.speed = 0.5f;

            // flash thirstbar
            flashCounter = (flashCounter + 1) % flashCounterInterval;
            if (flashCounter == flashCounterInterval - 1)
                thirstBar.enabled = true;
            else if (flashCounter == (flashCounterInterval / 2) - 1)
                thirstBar.enabled = false;

        }
        else if (thirst < midThirst)
        {
            moveSpeed = 6.0f;
            footprintCounterInterval = 20;

            breathParticlesLess.gameObject.SetActive(true);
            breathParticlesMore.gameObject.SetActive(false);

            animator.speed = 1.0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pool"))
        {
            foreach (Transform child in inventory.slots[selectedSlot].transform)
            {
                if (child.CompareTag("Bottle"))
                {
                    bCouldRefill = true;
                    refillText.SetActive(true);
                }
            }
        }
        else if (other.CompareTag("Pickup"))
        {
            pickupText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pool"))
        {
            bCouldRefill = false;
            refillText.SetActive(false);
        }
        else if (other.CompareTag("Pickup"))
        {
            pickupText.SetActive(false);
        }
    }

    private void SelectInventorySlot(float delta)
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

            float x = Mathf.Lerp(-60.0f, 60.0f, (float) selectedSlot / (float) (inventory.slots.Length - 1));
            float y = selectedSlotHighlight.GetComponent<RectTransform>().anchoredPosition.y;
            selectedSlotHighlight.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
        }
    }

    private void UseInventory()
    {
        bPickingUpItem = Input.GetKey(KeyCode.E);

        if (inventory.isFull[selectedSlot])
        {
            if (Input.GetKey(KeyCode.E))
            {
                inventory.slots[selectedSlot].GetComponent<Slot>().UseItem();
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                inventory.slots[selectedSlot].GetComponent<Slot>().DropItem();
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                inventory.slots[selectedSlot].GetComponent<Slot>().StopUsingItem();
            }
        }
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
                SpawnDecal(footprint, FootprintSpawn, new Vector3(0.185f, 0, 0));
        }

    }

    void SpawnDecal(GameObject prefab, Transform spawn, Vector3 offset)
    {
        Instantiate(prefab, spawn.position + offset, Quaternion.Euler(0, animator.gameObject.transform.rotation.y, 0));
    }
}
