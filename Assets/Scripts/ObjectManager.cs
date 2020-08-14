using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject[] objects;
    public bool bPlayerInScene;
    private GameObject player;

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
            yield return new WaitForSeconds(1.0f);
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
            yield return new WaitForSeconds(1.0f);
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
