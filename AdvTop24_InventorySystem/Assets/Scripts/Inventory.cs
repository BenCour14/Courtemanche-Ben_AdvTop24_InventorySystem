using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Space]
    // Variable to set the inventory size
    public int inventorySize = 24;

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
}
