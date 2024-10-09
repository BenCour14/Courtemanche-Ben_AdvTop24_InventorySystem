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
    public GameObject inventoryMenu;
    public GameObject itemPanel;
    public GameObject itemPanelGrid;

    // Ref to the existing panels in the inventory so it can be updated if more slots are needed
    private List<ItemPanel> existingPanels = new List<ItemPanel>();

    [Space]
    // Variable to set the inventory size
    private int inventorySize = 24;

    // Getter and setter to protect inventory size from being altered
    public int InventorySize
    {
        get { return inventorySize; }
        set
        {
            // Inventory size is capped at 50
            if(value > 50)
            {
                inventorySize = 50;
                RefreshInventory();
            }
            else
            {
                inventorySize = value;
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
                // When in-game lock and remove the cursor
                Cursor.lockState = CursorLockMode.Locked;
            }

            else
            {
                // Open UI
                inventoryMenu.SetActive(true);
                // When the inventory screen is open, make visible and confine the cursor
                Cursor.lockState = CursorLockMode.Confined;
            }
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

        int index = 0;
        foreach (ItemSlotInfo i in items)
        {
            i.name = "" + (index + 1);
            if (i.item != null) i.name += ": " + i.item.GiveName();
            else i.name += ": --";

            ItemPanel panel = existingPanels[index];
            if (panel != null)
            {
                panel.name = i.name + " Panel";

                panel.inventory = this;
                panel.itemSlot = i;
                if (i.item != null)
                {
                    panel.itemImage.gameObject.SetActive(true);
                    panel.itemImage.sprite = i.item.GiveItemImage();
                    panel.stacksText.gameObject.SetActive(true);
                    panel.stacksText.text = "" + i.stacks;
                }

            }

        }

    }
}
