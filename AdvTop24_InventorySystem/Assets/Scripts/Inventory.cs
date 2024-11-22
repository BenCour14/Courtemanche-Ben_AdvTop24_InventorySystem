using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    // List to store the inventory items
    [SerializeReference] public List<ItemSlotInfo> items = new List<ItemSlotInfo>();

    [Space]
    [Header("Inventory Menu Components")]

    // Refs to the different Inventory UI elements
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private GameObject itemPanel;
    [SerializeField] private GameObject itemPanelGrid;

    [SerializeField] public Mouse mouse;

    // Ref to the existing panels in the inventory so it can be updated if more slots are needed
    private List<ItemPanel> existingPanels = new List<ItemPanel>();

    // Dictionary that stores all available items using their name
    Dictionary<string, Item> allItemsDictionary = new Dictionary<string, Item>();

    [Space]
    // Variable to set the inventory size
    private int _inventorySize = 24;

    // Getter and setter to protect inventory size from being altered
    public int inventorySize
    {
        get {return _inventorySize;}
        set
        {
            // Inventory size is capped at 50
            if (value > 50)
            {
                _inventorySize = 50;
                RefreshInventory();
            }
            else
            {
                _inventorySize = value;
                RefreshInventory();
            }

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Inventory slots are initialized with no info
        for (int i = 0; i < inventorySize; i++)
        {
            // Adds no item and stack size to each empty slot
            items.Add(new ItemSlotInfo(null, 0));
        }

        // Convert all of the items that return as an array to a list
        List<Item> allItems = GetAllItems().ToList();
        string itemsInDictionary = "Items in Dictionary: ";
        foreach (Item i in allItems)
        {
            // Add items to dictionary if it's name is not already there
            if (!allItemsDictionary.ContainsKey(i.GiveName()))
            {
                allItemsDictionary.Add(i.GiveName(), i);
                itemsInDictionary += ", " + i.GiveName();
            }
            else
            {
                // Debug for if there's a duplicate name
                Debug.Log("" + i + " already exists in Dictionary - shares name with " + allItemsDictionary[i.GiveName()]);
            }
        }
        itemsInDictionary += ".";
        Debug.Log(itemsInDictionary);

        // Adding items for testing purposes
        AddItem("Wood", 40);
        AddItem("Stone", 20);
    }

    // Update is called once per frame
    void Update()
    {
        // Player can toggle the inventory on and off
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Check if UI is open
            if (inventoryMenu.activeSelf)
            {
                // Hide UI
                inventoryMenu.SetActive(false);
                // Clear item slot
                mouse.EmptySlot();
                // When in-game lock and remove the cursor
                Cursor.lockState = CursorLockMode.Locked;
            }

            else
            {
                // Open UI
                inventoryMenu.SetActive(true);
                // When the inventory screen is open, make visible and confine the cursor
                Cursor.lockState = CursorLockMode.Confined;

                RefreshInventory();
            }
        }

        // Drop whatever item is currently being held by the mouse
        if (Input.GetKeyDown(KeyCode.Mouse1) && mouse.itemSlot.item != null)
        {
            RefreshInventory();
        }

    }

    public void RefreshInventory()
    {
        // Store all existing panels in a list
        existingPanels = itemPanelGrid.GetComponentsInChildren<ItemPanel>().ToList();

        // Checking if the existing amount of item panels is less than inventory size to determine if more item panels need to be added
        if (existingPanels.Count < inventorySize)
        {
            // Calculate amount of panels to create
            int amountToCreate = inventorySize - existingPanels.Count;

            // Loop to instantiate the additional panels
            for (int i = 0; i < amountToCreate; i++)
            {
                GameObject newPanel = Instantiate(itemPanel, itemPanelGrid.transform);
                existingPanels.Add(newPanel.GetComponent<ItemPanel>());
            }
        }

        // Index to track the item slots
        int index = 0;

        // Will look at each item slot in the items list and update it
        foreach (ItemSlotInfo i in items)
        {
            i.name = "" + (index + 1);
            if (i.item != null) i.name += ": " + i.item.GiveName();
            else i.name += ": --";

            // Update the panels
            ItemPanel panel = existingPanels[index];
            if (panel != null)
            {
                panel.name = i.name + " Panel";

                panel.SetInventory(this); // Setting inventory reference in item panel script
                panel.SetItemSlot(i); // Set item slot so that its UI can be updated
                if (i.item != null)
                {
                    // If the item exists activate the item image and stacks text
                    panel.itemImage.gameObject.SetActive(true);
                    panel.itemImage.sprite = i.item.GiveItemImage();
                    panel.itemImage.CrossFadeAlpha(1, 0.05f, true);
                    panel.stacksText.gameObject.SetActive(true);
                    panel.stacksText.text = "" + i.stacks;
                }
                else
                {
                    // If the item deosn't exist disable the item image and stacks text
                    panel.itemImage.gameObject.SetActive(false);
                    panel.stacksText.gameObject.SetActive(false);
                }

            }
            // With every loop add 1 to the index
            index++;
        }
        mouse.EmptySlot();

    }

    public int AddItem(string itemName, int amount)
    {
        // Find Item to add
        Item item = null;
        allItemsDictionary.TryGetValue(itemName, out item);

        // Exit method if no item was found
        if (item == null)
        {
            Debug.Log("Could not find Item in Dictionary to add to Inventory");
            return amount;
        }

        // Checks if the item can get added to an existing stack
        foreach (ItemSlotInfo i in items)
        {
            // Check if the item slot is not empty 
            if (i.item != null)
            {
                // Check if the item being added is the same as the item in the slot
                if (i.item.GiveName() == item.GiveName())
                {
                    // Checking if the amount that the player is adding is greater than the space available in the stack
                    if (amount > i.item.MaxStacks() - i.stacks)
                    {
                        // If true, reduce the amount being added by the space available in the stack and fill the current stack
                        amount -= i.item.MaxStacks() - i.stacks;
                        i.stacks = i.item.MaxStacks();
                    }
                    // If the amount being added doesn't exceed max stack size, simply add the items to the stack
                    else
                    {
                        i.stacks += amount;
                        if (inventoryMenu.activeSelf) RefreshInventory();
                        return 0; // No items left to add
                    }
                }
            }
        }

        // Fill the empty slots with leftover items
        foreach (ItemSlotInfo i in items)
        {
            // Look for an empty slot
            if (i.item == null)
            {
                // Check if the amount trying to get added exceeds max stack size
                if (amount > item.MaxStacks())
                {
                    // If true, fill the slot to max and reduce the amount by max stack size to get what coudn't be added
                    i.item = item;
                    i.stacks = item.MaxStacks();
                    amount -= item.MaxStacks();
                }
                // If the remaining amount can fill a slot, add the remaining items to the slot
                else
                {
                    i.item = item;
                    i.stacks = amount;
                    if (inventoryMenu.activeSelf) RefreshInventory();
                    return 0; // No items left to add
                }
            }
        }
        // No space in Inventory, return remaining items
        Debug.Log("No space in Inventory for: " + item.GiveName());
        if (inventoryMenu.activeSelf) RefreshInventory();
        return amount;

    }

    // Helper method to clear a slot when needed
    public void ClearSlot(ItemSlotInfo slot)
    {
        slot.item = null;
        slot.stacks = 0;
    }

    // Go through entire project and find all the scripts that are a subtype of Item 
    IEnumerable<Item> GetAllItems()
    {
        return System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(typeof(Item)))
            .Select(type => System.Activator.CreateInstance(type) as Item);
    }
}

