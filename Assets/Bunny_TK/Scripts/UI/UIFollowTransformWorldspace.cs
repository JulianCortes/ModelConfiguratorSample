using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowTransformWorldspace : MonoBehaviour
{

    public RectTransform canvasRectT;
    public RectTransform _rectTransform;
    public Transform objectToFollow;

    private Vector3 camNormal;
    private Vector3 vectorFromCam;
    private float camNormDot = 0f;

    [SerializeField]
    bool isVisible = false;
    [SerializeField]
    float distanceFromCamera = 0f;

    public bool IsVisible
    {
        get
        {
            return isVisible;
        }
    }

    public float DistanceFromCamera
    {
        get
        {
            return distanceFromCamera;
        }
    }

    void Update()
    {
        if (objectToFollow == null) return;

        camNormal = Camera.main.transform.forward;
        vectorFromCam = objectToFollow.transform.position - Camera.main.transform.position;
        camNormDot = Vector3.Dot(camNormal, vectorFromCam);

        distanceFromCamera = Vector3.Distance(objectToFollow.transform.position, Camera.main.transform.position);

        if (camNormDot < 0) //Im behind ya
        {
            isVisible = false;
        }
        else
        {
            isVisible = true;

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, objectToFollow.position);
            _rectTransform.anchoredPosition = screenPoint - canvasRectT.sizeDelta / 2f;
        }
    }
}
