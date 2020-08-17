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
    public GameObject pausePanel;
    public LightBob lightBob;
    public GameObject invertFilter;
    public GameObject HUD;
    private bool bPaused = false;

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
    private int footprintCounterInterval = 25;

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
    private int flashCounterInterval = 20;


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
        if (bAlreadyMoved)
        {
            pausePanel.SetActive(bPaused);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                bPaused = !bPaused;

                if (bPaused) PauseGame(true);
                else PauseGame(false);
            }
        }

        cinemachineBrain.enabled = bCanMove;

        if (!bPaused)
        {
            // triggers once when player movement is enabled
            if (bCanMove && !bAlreadyMoved)
            {
                StartCoroutine(ZoomFOV(cinemachineFreeLook, 4.0f, 45.0f));
                bAlreadyMoved = true;
            }

            if (bCanMove)
            {
                UpdateStats();
                SelectInventorySlot(-Input.mouseScrollDelta.y);
                UseInventory();
            }
        }
    }

    private void PauseGame(bool pause)
    {
        // disable all animations, particles, prompts
        animator.SetFloat("Velocity", 0.0f);
        animator.SetBool("Drinking", false);
        animator.SetBool("WaterEmpty", false);
        animator.SetBool("Refilling", false);

        refillText.SetActive(false);
        pickupText.SetActive(false);

        if (pause)
        {
            bCanMove = false;

            breathParticlesLess.Pause();
            breathParticlesMore.Pause();
            sweatParticles.Pause();
            snowParticles.Pause();

            lightBob.period = 0;
        }
        else
        {
            bCanMove = true;

            breathParticlesLess.Play();
            breathParticlesMore.Play();
            sweatParticles.Play();
            snowParticles.Play();

            lightBob.period = 1;
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

    public IEnumerator IShowInvertAndResetPanel()
    {
        yield return new WaitForSeconds(1.0f);
        invertFilter.SetActive(true);
        HUD.SetActive(false);
        
        yield return new WaitForSeconds(5.0f);
        resetPanel.SetActive(true);

        // unlock cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void PlayerDeath()
    {
        Debug.Log("player dead");

        StartCoroutine(ZoomFOV(cinemachineFreeLook, 2.0f, 35.0f));
        StartCoroutine(IShowInvertAndResetPanel());

        // freeze player
        bDead = true;
        bCanMove = false;

        // disable all animations, particles, prompts
        animator.SetFloat("Velocity", 0.0f);
        animator.SetBool("Drinking", false);
        animator.SetBool("WaterEmpty", false);
        animator.SetBool("Refilling", false);

        refillText.SetActive(false);
        pickupText.SetActive(false);

        breathParticlesLess.Pause();
        breathParticlesMore.Pause();
        sweatParticles.Pause();
        snowParticles.Pause();

        lightBob.period = 0;

        // change sprite
        animator.speed = 1.0f;
        animator.SetBool("Dead", true);

        // increase volume of breathing
        // decrease volume of everything else

    }

    //public void ResetGame(int index)
    //{
    //    StartCoroutine(IResetGame(index));
    //}

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

    public static IEnumerator ZoomFOV(CinemachineFreeLook cam, float duration, float targetFOV)
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
        if (fillAmount < 0) fillAmount = 0;
        else if (fillAmount > 1) fillAmount = 1;
        thirstBar.fillAmount = fillAmount;

        if (thirst >= midThirst)
        {
            // slower animation, speed, footprints
            animator.speed = 0.5f;
            moveSpeed = 3.0f;
            footprintCounterInterval = 50;

            // breathless particles
            breathParticlesLess.gameObject.SetActive(false);
            breathParticlesMore.gameObject.SetActive(true);

            // flash thirstbar
            flashCounter = (flashCounter + 1) % flashCounterInterval;
            if (flashCounter == flashCounterInterval - 1)
                thirstBar.enabled = true;
            else if (flashCounter == (flashCounterInterval / 2) - 1)
                thirstBar.enabled = false;

        }
        else if (thirst < midThirst)
        {
            thirstBar.enabled = true;
            animator.speed = 1.0f;
            moveSpeed = 6.0f;
            footprintCounterInterval = 25;

            breathParticlesLess.gameObject.SetActive(true);
            breathParticlesMore.gameObject.SetActive(false);

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
        // cancel any ongoing animations
        animator.SetBool("Drinking", false);
        animator.SetBool("WaterEmpty", false);
        animator.SetBool("Refilling", false);

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
