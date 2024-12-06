using UnityEngine;

// Will display the class in the editor
[System.Serializable]
public abstract class Item
{
    // Abstract tag means this method must be overridden in derived classes 
    public abstract string GiveName();

    // Virtual tags, if not overridden, will default to its code  
    public virtual int MaxStacks()
    {
        return 30;
    }

    // Using Resource.Load to retreive the UI sprites from the Resources folder. Because this is a virtual method if nothing is overridden then the No Item Image Icon will appear
    public virtual Sprite GiveItemImage()
    {
        return Resources.Load<Sprite>("UI/Item Images/No Item Image Icon");  
    }

    public virtual GameObject DropObject()
    {
        return Resources.Load<GameObject>("Pickup Items/Default Item");
    }

}
