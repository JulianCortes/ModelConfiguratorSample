using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Componente per il dragging degli oggetti 3D, si basa sulle interfacce di dragging
/// </summary>
public class SimpleDrag :
    MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{

    /// <summary>
    /// (il tuo è un) Dragging Relativo
    /// Se falso l'oggetto si poszione al centro del mouse
    /// </summary>
    public bool relative = true;

    /// <summary>
    /// Se falso non sposterà l'oggetto
    /// Manderà comunque gli eventi e calcolerà comunque il target pos
    /// </summary>
    public bool isEnabled = true;

    /// <summary>
    /// La posizione in Z se useZDistance è true
    /// </summary>
    //public float zDistance = 1f;

    /// <summary>
    /// Se vero la Z che verrà applicata è zDistance, altrimenti la Z non verrà toccata
    /// </summary>
    //public bool useZDistance = false;

    /// <summary>
    /// Posizione di destinazione
    /// </summary>
    Vector3 targetPosition;
    /// <summary>
    /// Utilizzato per il dragging relativo
    /// </summary>
    Vector3 offset;
    Vector3 screenPosition;
    Vector3 camForward;
    public Vector3 TargetPosition
    {
        get
        {
            return targetPosition;
        }

    }

    /// <summary>
    /// Evento lanciato per ogni frame che questo oggetto è in fase di drag
    /// </summary>
    public event System.EventHandler Drag;

    /// <summary>
    /// Evento lanciato al BeginDrag (pointer down)
    /// </summary>
    public event System.EventHandler BeginDrag;

    /// <summary>
    /// Evento lanciato alla fine del drag (pointer up)
    /// </summary>
    public event System.EventHandler EndDrag;

    #region Drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));

        OnBeginDrag();
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Drag");
        if (relative)
        {
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));

            targetPosition = currentPos + offset ;

            if (isEnabled)
                transform.position = targetPosition;
        }
        else
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));

            targetPosition = pos;

            if (isEnabled)
                transform.position = targetPosition;
        }

        OnDrag();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDrag();
    }
    #endregion Drag

    #region Pointer
    public void OnPointerDown(PointerEventData eventData)
    {

    }
    public void OnPointerEnter(PointerEventData eventData)
    {

    }
    public void OnPointerExit(PointerEventData eventData)
    {

    }
    public void OnPointerUp(PointerEventData eventData)
    {

    }
    #endregion Pointer

    #region EventLaunchers
    void OnDrag()
    {
        if (Drag != null)
            Drag(this, System.EventArgs.Empty);
    }
    void OnBeginDrag()
    {
        if (BeginDrag != null)
            BeginDrag(this, System.EventArgs.Empty);
    }
    void OnEndDrag()
    {
        if (EndDrag != null)
            EndDrag(this, System.EventArgs.Empty);
    }
    #endregion EventLaunchers

}
