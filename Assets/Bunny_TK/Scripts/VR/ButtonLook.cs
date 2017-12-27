using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System;

[Serializable]
public class ButtonLook : Button
{
    //[SerializeField]
    public float fillLenght = 1.5f;
    [SerializeField]
    public float fillTime = 0f;
    [SerializeField]
    public bool pointerIsHovering = false;

    void Update()
    {
        //Metodo Stupido per simulare il click
        if (pointerIsHovering)
        {
            fillTime += Time.deltaTime;

            if(fillTime >= fillLenght)
            {
                OnPointerClick(new PointerEventData(EventSystem.current));
                pointerIsHovering = false;
            }
        }
        else
        {
            fillTime = 0f;
        }

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        pointerIsHovering = true;
        //Debug.Log("PointerEnter Button");
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        pointerIsHovering = false;
        //Debug.Log("PointerExit Button");
    }
}
