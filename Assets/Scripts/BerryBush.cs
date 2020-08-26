using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryBush : MonoBehaviour
{
    public Animator animator;
    public ParticleSystem leafParticles;

    public int spawnCounter;
    public int spawnInterval;

    public Transform _spawn;
    public GameObject _item;

    public AudioManager audioManager_Effects;

    void Update()
    {
        spawnCounter = (spawnCounter + 1) % spawnInterval;
        if (spawnCounter == 0)
        {
            // check if berry already spawned
            if (transform.childCount < 1)
                SpawnItem(_item, _spawn);
        }
    }

    void SpawnItem(GameObject prefab, Transform spawn)
    {
        GameObject newItem = Instantiate(prefab, spawn.position, Quaternion.identity);
        newItem.transform.parent = _spawn;
    }
    
    private IEnumerator IShakeBush(float t)
    {
        // play sound
        audioManager_Effects.Play("Misc_Bush");

        // play animation, particles
        animator.SetBool("bShaking", true);
        leafParticles.Play();

        yield return new WaitForSeconds(t);

        // reset animation, particles
        animator.SetBool("bShaking", false);
        leafParticles.Stop();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player shaking bush");
            StartCoroutine(IShakeBush(1.0f));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player shaking bush");
            StartCoroutine(IShakeBush(0.5f));
        }
    }
}
