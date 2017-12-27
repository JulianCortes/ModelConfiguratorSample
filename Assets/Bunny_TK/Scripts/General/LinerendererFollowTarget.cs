using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinerendererFollowTarget : MonoBehaviour
{
    public LineRenderer targetLinerenderer;

    [Header("Target Transforms/Positions")]
    public bool useHeadTransform = true;
    public bool useTailTransform = true;
    public Transform headTransform;
    public Transform tailTransform;

    public Vector3 headPosition;
    public Vector3 tailPosition;

    [Header("Width Scaling Parameters")]
    public Transform scaleTransform;
    public bool scaleWithDistance = true;
    public float scaleFactor = 1000f;
    public bool scaleTransformIsMainCamera = true;

    public bool IsVisible
    {
        get
        { return targetLinerenderer.enabled; }

        set
        {
            targetLinerenderer.enabled = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        targetLinerenderer.SetPositions(new Vector3[] { this.transform.position, this.transform.position });
        if (scaleTransformIsMainCamera && scaleWithDistance) scaleTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (useHeadTransform) headPosition = headTransform.position;
        if (useTailTransform) tailPosition = tailTransform.position;

        targetLinerenderer.SetPosition(0, headPosition);
        targetLinerenderer.SetPosition(1, tailPosition);

        if(scaleWithDistance)
        {
            targetLinerenderer.startWidth = Vector3.Distance(headPosition, scaleTransform.position) / scaleFactor;
            targetLinerenderer.endWidth = Vector3.Distance(tailPosition, scaleTransform.position) / scaleFactor;
        }
    }
}
