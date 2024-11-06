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

    public ItemPanel sourceItemPanel;
    public int splitSize;

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

        // Splitting the stack if there's am item in the slot
        if (itemSlot.item != null)
        {
            // If scrolling up on mouse wheel increase split size, can't exceed stack size
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && splitSize < itemSlot.stacks)
            {
                splitSize++;
            }
            // If scrolling down on mouse wheel decrease split size, can't go below 1
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && splitSize > 1)
            {
                splitSize--;
            }

            // Update split size text with new split amount
            stacksText.text = "" + splitSize;

            // If player is moving entire stack, hide source panel's stack count
            if (splitSize == itemSlot.stacks) sourceItemPanel.stacksText.gameObject.SetActive(false);
            else
            {
                // Else show altered item count on the source panel and update the text
                sourceItemPanel.stacksText.gameObject.SetActive(true);
                sourceItemPanel.stacksText.text = "" + (itemSlot.stacks - splitSize);
            }
        }
    }

    // Helper method that updates the UI with the selected item 
    public void SetUI()
    {
        stacksText.text = "" + splitSize;
        itemImage.sprite = itemSlot.item.GiveItemImage();
    }

    // Helper method to empty a slot
    public void EmptySlot()
    {
        itemSlot = new ItemSlotInfo(null, 0);
    }
}
