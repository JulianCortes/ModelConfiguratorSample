using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DropUICheck : DropUISurface
{
    public GameObject match;
    public bool disableOnMatch = false;
    public bool matched = false;
    public event System.EventHandler MatchAttempt;
    public override void OnDrop(PointerEventData eventData)
    {
        if (Check(eventData))
        {
            base.OnDrop(eventData);

            //Crea una funzione per dragui che fa ste cose
            eventData.pointerDrag.GetComponent<DraggableUIElement>().enabled = false;
            eventData.pointerDrag.GetComponent<CanvasGroup>().blocksRaycasts = false;
            eventData.pointerDrag.GetComponent<Canvas>().overrideSorting = false;
            eventData.pointerDrag.GetComponent<Canvas>().sortingOrder = 0;
            matched = true;
        }
        else
        {
            matched = false;
            Debug.Log("Ritenta");
        }
        OnMatchAttempt();
    }

    public virtual bool Check(PointerEventData eventData)
    {
        if (match == null) return false;
        if (eventData.pointerDrag == null) return false;
        return eventData.pointerDrag == match;
    }

    private void OnMatchAttempt()
    {
        if (MatchAttempt != null)
            MatchAttempt(this, System.EventArgs.Empty);
    }
}
