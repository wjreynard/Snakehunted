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
    
    private IEnumerator IShakeBush()
    {

        yield return new WaitForSeconds(1.0f);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player shaking bush");

            // play animation, particles
            animator.SetBool("bShaking", true);
            leafParticles.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player not shaking bush anymore");

            // reset animation, particles
            animator.SetBool("bShaking", false);
            leafParticles.Stop();
        }

    }
}
