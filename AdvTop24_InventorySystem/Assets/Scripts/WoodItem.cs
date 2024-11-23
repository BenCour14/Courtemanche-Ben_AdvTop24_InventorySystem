using UnityEngine;

public class WoodItem : Item
{
    // Overidding the abstract item class and assigning the info for the wood item 
    public override string GiveName()
    {
        return "Wood";
    }

    public override int MaxStacks()
    {
        return 10;
    }

    public override Sprite GiveItemImage()
    {
        return Resources.Load<Sprite>("UI/Item Images/Wood Icon");
    }
}
