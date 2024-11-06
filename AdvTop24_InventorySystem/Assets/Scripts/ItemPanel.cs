using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemPanel : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IDropHandler
{
    // References
    public Inventory inventory;
    private Mouse mouse;
    public ItemSlotInfo itemSlot;
    public Image itemImage;
    public TextMeshProUGUI stacksText;

    private bool click; // Bool to track if the click action has happened

    // Unity event that triggers when the cursor enters the panel
    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.pointerPress = this.gameObject;
    }
    // Unity event that triggers when the mouse button is pressed
    public void OnPointerDown(PointerEventData eventData)
    {
        click = true;
    }
    // Unity event that triggers when the mouse button is released
    public void OnPointerUp(PointerEventData eventData)
    {
        if (click)
        {
            OnClick();
            click = false;
        }
    }
    // Unity event that triggers when an item is released from a drag 
    public void OnDrop(PointerEventData eventData)
    {
        OnClick();
        click = false;
    }
    // Unity event that triggers when holding down and dragging the panel
    public void OnDrag(PointerEventData eventData)
    {
        if (click)
        {
            OnClick();
            click = false;
        }
    }
    
    // Transfers the item from the slot to the mouse so the player can visually see the item that they've selected
    public void PickupItem()
    {
        mouse.itemSlot = itemSlot; // Set clicked item slot to mouse item slot
        mouse.sourceItemPanel = this; // Reference for source slot

        // If shift key is down when picking up item & there's more than 1 item, split stack
        if (Input.GetKey(KeyCode.LeftShift) && itemSlot.stacks > 1) mouse.splitSize = itemSlot.stacks / 2;
        // Else set split size equal to stack count
        else mouse.splitSize = itemSlot.stacks;
        
        mouse.SetUI();
    }

    // Fade out the item image of the selected item
    public void FadeOut()
    {
        itemImage.CrossFadeAlpha(0.3f, 0.05f, true);
    }

   // Drop selected item into a slot 
    public void DropItem()
    {
        itemSlot.item = mouse.itemSlot.item; // Transfers item from mouse to slot

        // See if split size is less than mouse stack size  
        if (mouse.splitSize < mouse.itemSlot.stacks)
        {
            itemSlot.stacks = mouse.splitSize; // set target slot equal to split stack amount to be dropped
            mouse.itemSlot.stacks -= mouse.splitSize; // Decrease mouse slot stack size by split size amount 
            mouse.EmptySlot(); // CLear mouse slot
        }

        else
        {
            itemSlot.stacks = mouse.itemSlot.stacks; // Assigns stack count
            inventory.ClearSlot(mouse.itemSlot); // Clear the item from the mouse
        }
       
    }

    public void SwapItem(ItemSlotInfo slotA, ItemSlotInfo slotB)
    {
        // Hold item for transfer
        ItemSlotInfo tempItem = new ItemSlotInfo(slotA.item, slotA.stacks);

        // Perform the swap between the two slots
        slotA.item = slotB.item;
        slotA.stacks = slotB.stacks;

        slotB.item = tempItem.item;
        slotB.stacks = tempItem.stacks;

    }
    
    // Click events for the item panel
    public void OnClick()
    {
        if (inventory != null) // Make sure inventory ref is valid
        {
            mouse = inventory.mouse; // Get ref to mouse from the inventory class

            // Grab item if mouse slot is empty
            if (mouse.itemSlot.item == null)
            {
                if (itemSlot.item != null) // Checks if current slot has an item
                {
                    PickupItem();
                    FadeOut();
                }
            }
            else
            {
                // Clicked on orginial slot
                if (itemSlot == mouse.itemSlot)
                {
                    inventory.RefreshInventory();
                }
                // Clicked on empty slot
                else if (itemSlot.item == null)
                {
                    DropItem();
                    inventory.RefreshInventory();
                }
                //Clicked on occupied slot of a different type of item
                else if (itemSlot.item.GiveName() != mouse.itemSlot.item.GiveName())
                {
                    SwapItem(itemSlot, mouse.itemSlot); // Swap the different items between the mouse and slot
                    inventory.RefreshInventory();
                }
            }
        }
    }
}
