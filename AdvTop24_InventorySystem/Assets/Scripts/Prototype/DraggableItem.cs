using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    
    // ref to item's starting parent so that it can be reattached after drag
    public Transform parentAfterDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("BeginDrag");

        // store current parent of item
        parentAfterDrag = transform.parent;

        // set canvas as the parent of the object
        transform.SetParent(transform.root);

        // set item to be the last child object under the parent so that the item appears over everything
        transform.SetAsLastSibling();

        // disable raycast on image component so the item doesn't block the mouse raycasts
        image.raycastTarget = false;

 
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");

        // track current mouse position
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");

        // set item back to original parent after drag
        transform.SetParent(parentAfterDrag);

        // re-enable the raycast on the item's image component so that it can one again interact with the mouse
        image.raycastTarget = true;

  
    }

      

   

}
