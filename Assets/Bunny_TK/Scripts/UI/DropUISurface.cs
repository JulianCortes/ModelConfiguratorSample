using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DropUISurface : MonoBehaviour, IDropHandler {

    public virtual void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        dropped.transform.position = transform.position;
    }
}
