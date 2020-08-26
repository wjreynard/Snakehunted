using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [Range(.1f, 1f)]
    public float delay;

    public GameObject[] objects;
    public bool bPlayerInScene;
    private GameObject player;

    public AudioManager audioManager_Effects;
    private void Awake()
    {
        DisableObjectsFast();

        if (bPlayerInScene)
        {
            player = GameObject.FindWithTag("Player");
            player.GetComponent<Player>().bCanMove = false;
        }
    }

    private void Start()
    {
        EnableObjects();
    }

    private IEnumerator IEnableObjects()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(true);
            audioManager_Effects.Play("Misc_Pop");
            yield return new WaitForSeconds(delay);
        }

        if (bPlayerInScene)
            player.GetComponent<Player>().bCanMove = true;
    }

    private IEnumerator IDisableObjects()
    {
        if (bPlayerInScene)
            player.GetComponent<Player>().bCanMove = false;

        for (int i = objects.Length - 1; i >= 0; i--)
        {
            objects[i].SetActive(false);
            yield return new WaitForSeconds(delay);
        }
    }

    public void EnableObjects()
    {
        StartCoroutine(IEnableObjects());
    }

    public void DisableObjects()
    {
        StartCoroutine(IDisableObjects());
    }

    public void DisableObjectsFast()
    {
        for (int i = objects.Length - 1; i >= 0; i--)
        {
            objects[i].SetActive(false);
        }
    }
}
