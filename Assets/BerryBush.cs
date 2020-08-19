using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryBush : MonoBehaviour
{
    public int spawnCounter;
    public int spawnInterval;

    public Transform _spawn;
    public GameObject _item;

    private void Awake()
    {
        SpawnItem(_item, _spawn);
    }

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
}
