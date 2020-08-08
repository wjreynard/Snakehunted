using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int index;

    [Space(10)]
    private Inventory inventory;
    public GameObject itemSprite;

    private void Awake()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<Player>().bIsPickingUp)
            {
                PickupItem();
            }
        }
    }

    private void PickupItem()
    {
        // find empty slot
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.isFull[i] == false)
            {
                inventory.isFull[i] = true;
                Instantiate(itemSprite, inventory.slots[i].transform, false);
                Destroy(gameObject);    // remove item from screen
                break;
            }
        }
    }
}
