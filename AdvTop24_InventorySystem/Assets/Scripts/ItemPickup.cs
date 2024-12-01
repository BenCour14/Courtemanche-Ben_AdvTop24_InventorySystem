using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemToDrop;
    public int amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        // Check if player collides with item
        if (other.tag == "Player")
        {
            Debug.Log("Detected");

            //Find inventory component on player
            Inventory playerInventory = other.GetComponentInChildren<Inventory>();
            // If the inventory is found pick up the item
            if (playerInventory != null) PickUpItem(playerInventory);
        }
    }

    // Add item to inventory and destroy the pickup item
    public void PickUpItem(Inventory inventory)
    {
        amount = inventory.AddItem(itemToDrop, amount);

        if (amount < 1) Destroy(this.gameObject);
    }

}
