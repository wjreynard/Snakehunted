using UnityEngine;

public class Pickup : MonoBehaviour
{
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
            if (other.GetComponent<Player>().bPickingUpItem)
            {
                PickupItem();
                other.GetComponent<Player>().pickupText.SetActive(false);
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
