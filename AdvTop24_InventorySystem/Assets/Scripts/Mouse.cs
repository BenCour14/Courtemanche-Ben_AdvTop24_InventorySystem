using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Mouse : MonoBehaviour
{
    public GameObject mouseItemUI;
    public Image mouseCursor;
    public ItemSlotInfo itemSlot;
    public Image itemImage;
    public TextMeshProUGUI stacksText;

    // Update is called once per frame
    void Update()
    {
        // Set this mouse cursor to the current mouse position 
        transform.position = Input.mousePosition;

        // Check if mouse is locked for when inventory is closed
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            // Hide cursor and mouse item slot panel
            mouseCursor.enabled = false;
            mouseItemUI.SetActive(false);
        }
        // If mouse is not locked
        else
        {
            // Show the cursor
            mouseCursor.enabled = true;

            // Checking if current slot has an item
            if (itemSlot.item != null)
            {
                mouseItemUI.SetActive(true); // Make the item panel visible
            }
            else
            {
                mouseItemUI.SetActive(false); // If no item, hide the item panel
            }
        }
    }

    // Helper method that updates the UI with the selected item 
    public void SetUI()
    {
        stacksText.text = "" + itemSlot.stacks;
        itemImage.sprite = itemSlot.item.GiveItemImage();
    }

    // Helper method to empty a slot
    public void EmptySlot()
    {
        itemSlot = new ItemSlotInfo(null, 0);
    }
}
