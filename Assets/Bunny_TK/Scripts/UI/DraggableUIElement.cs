using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

//[RequireComponent(typeof(CanvasGroup))]
//[RequireComponent(typeof(Canvas))]
public class DraggableUIElement : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler, IPointerUpHandler
{
    [SerializeField]
    protected CanvasGroup _cGroup;
    [SerializeField]
    protected Canvas _canvas;
    [SerializeField]
    public Canvas _parentCanvas;
    [SerializeField]
    protected bool _goesBackInPosition;
    [SerializeField]
    protected RectTransform _thisTransform;

    public bool detachFromParentOnSelect = false;
    public bool relativeDragging = true;

    [SerializeField]
    Vector2 offsetMousePosition;

    protected enum SortOrder : int
    {
        Back = 0,
        Front = 1
    }

    void Start()
    {
        _cGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponent<Canvas>();
        _canvas.overrideSorting = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        offsetMousePosition = (Vector2)gameObject.transform.position - eventData.position;

        if (!relativeDragging)
            transform.position = eventData.position;

        Selected();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_parentCanvas.renderMode == RenderMode.WorldSpace)
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(eventData.position);
        else
        {
            transform.position = (Vector2)eventData.position + (relativeDragging ? offsetMousePosition : Vector2.zero);
        }
    }

    private void Selected()
    {
        Canvas.ForceUpdateCanvases();
        if (detachFromParentOnSelect)
            transform.SetParent(_parentCanvas.transform);
        _canvas.overrideSorting = true;
        SetSortOrder(SortOrder.Front);
        _cGroup.blocksRaycasts = false;
    }

    private void Deselected()
    {
        _canvas.overrideSorting = true;
        SetSortOrder(SortOrder.Back);
        _cGroup.blocksRaycasts = true;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Deselected();
        //_canvas.overrideSorting = false;
        if (_goesBackInPosition)
            transform.localPosition = Vector3.zero;
    }


    protected void SetSortOrder(SortOrder order)
    {
        _canvas.sortingOrder = (int)order;
    }

    private float elapsedTime = 0;
    private float speed;

    private IEnumerator MoveToPosition_Coroutine(Vector2 position)
    {
        elapsedTime = 0;
        Vector2 startPos = _thisTransform.position;

        while ((Vector2)_thisTransform.position != position)
        {
            elapsedTime += Time.deltaTime;
            _thisTransform.position = Vector2.Lerp(startPos, position, elapsedTime * speed);
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Deselected();
    }
}
