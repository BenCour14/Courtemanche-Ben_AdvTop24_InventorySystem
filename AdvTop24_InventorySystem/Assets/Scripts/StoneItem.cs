using UnityEngine;

public class StoneItem : Item
{
    // Overidding the abstract item class and assigning the info for the wood item 
    public override string GiveName()
    {
        return "Stone";
    }
    public override int MaxStacks()
    {
        return 5;
    }
    public override Sprite GiveItemImage()
    {
        return Resources.Load<Sprite>("UI/Item Images/Stone Icon");
    }
}
