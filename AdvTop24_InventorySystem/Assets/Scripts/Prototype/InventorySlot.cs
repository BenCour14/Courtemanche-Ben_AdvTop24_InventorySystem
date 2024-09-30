using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    // part of the IDropHandler Unity system that is called when an item is dropped 
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0) // only allow one item per slot
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            draggableItem.parentAfterDrag = transform;
        }
        
    }

}
