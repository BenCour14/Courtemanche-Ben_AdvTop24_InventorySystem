using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemPanel : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IDropHandler
{
    // References
    [SerializeField] private Inventory _inventory;
    public Inventory inventory => _inventory;

    

    private Mouse mouse;

    [SerializeField] private ItemSlotInfo _itemSlot;
    public ItemSlotInfo itemSlot => _itemSlot;


    //[SerializeField] private Image _itemImage;
    //public Image itemImage => _itemImage;

    public Image itemImage;


    [SerializeField] private TextMeshProUGUI _stacksText;
    public TextMeshProUGUI stacksText => _stacksText;

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
        mouse.SetItemSlot(itemSlot); // Set clicked item slot to mouse item slot using a setter method
        mouse.SetSourceItemPanel(this); // Reference for source slot using a setter method

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

    public void StackItem(ItemSlotInfo source, ItemSlotInfo destination, int amount)
    {
        // Figure out avalable space in the destination slot
        int slotsAvailable = destination.item.MaxStacks() - destination.stacks;
        if (slotsAvailable == 0) return; // Return if there's no available space

        // If the amount that you're trying to transfer is more than the available space, fill the destination slot to max
        if (amount > slotsAvailable)
        {
            source.stacks -= slotsAvailable; // Reduce amount transfered from the source slot
            destination.stacks = destination.item.MaxStacks();
        }

       // If the amount that you're trying to transfer fits within the available space, transfer all of it, else reduce the source stack by amount moved
        if (amount <= slotsAvailable)
        {
            destination.stacks += amount;
            if (source.stacks == amount) inventory.ClearSlot(source);
            else source.stacks -= amount;
        }
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
                // Cliked on occupied slot of same type
                else if (itemSlot.stacks < itemSlot.item.MaxStacks())
                {
                    StackItem(mouse.itemSlot, itemSlot, mouse.splitSize);
                    inventory.RefreshInventory();
                }
            }
        }
    }

    // Setter method to be able to safely set the inventory reference in the inventory script 
    public void SetInventory(Inventory updateInventory)
    {
        _inventory = updateInventory;
     }

    //Setter method so that the item slot can be safely updated
    public void SetItemSlot(ItemSlotInfo updateItemSlot)
    {
        _itemSlot = updateItemSlot;
    }
}
