
// Make the class visible in the inspector
[System.Serializable]
public class ItemSlotInfo
{
    public Item item;
    public string name;
    public int stacks;

    // Initialize the item and stack size when an instance of this class is created
    public ItemSlotInfo(Item newItem, int newStacks)
    {
        item = newItem;
        stacks = newStacks;
    }
}
