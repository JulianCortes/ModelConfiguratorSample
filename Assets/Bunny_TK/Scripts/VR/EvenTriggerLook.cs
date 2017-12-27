using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;


public class EvenTriggerLook : EventTrigger
{
    [SerializeField]
    public float fillLenght = 2f;
    [SerializeField]
    public float fillTime = 0f;

    [SerializeField]
    bool pointerHovering = false;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        pointerHovering = true;
        if (SightLook.Instance != null)
            SightLook.Instance.SetCurrentLookingObject(this.gameObject, 1.5f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        pointerHovering = false;

        if (SightLook.Instance != null)
            SightLook.Instance.SetCurrentLookingObject(null);

    }

    void Update()
    {
        if (pointerHovering)
        {
            fillTime += Time.deltaTime;
            if (fillTime >= fillLenght)
            {
                this.OnPointerClick(new PointerEventData(EventSystem.current));
                pointerHovering = false;
            }

        }
        else
        {
            fillTime = 0f;
        }
    }

    public void OnPointerEnter()
    {
        Debug.Log("Enter");
    }

    public void OnPointerExit()
    {
        Debug.Log("Exit");
    }

    public void Test()
    {
        Debug.Log("Click");
    }


}
