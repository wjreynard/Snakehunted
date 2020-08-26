using UnityEngine;

public class Pickup : MonoBehaviour
{
    private Inventory inventory;
    public GameObject itemSprite;

    private AudioManager audioManager_Effects;

    private void Awake()
    {
        audioManager_Effects = GameObject.FindGameObjectWithTag("Audio_Effects").GetComponent<AudioManager>();
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
                // play sound
                if (itemSprite.CompareTag("Bottle"))
                {
                    audioManager_Effects.Play("Bottle_Pickup");
                }
                else if (itemSprite.CompareTag("Berry"))
                {
                    audioManager_Effects.Play("Berry_Pickup");
                }
                else if (itemSprite.CompareTag("Stone"))
                {
                    audioManager_Effects.Play("Stone_Pickup");
                }

                inventory.isFull[i] = true;
                Instantiate(itemSprite, inventory.slots[i].transform, false);
                Destroy(gameObject);    // remove item from screen
                break;
            }
        }
    }
}
