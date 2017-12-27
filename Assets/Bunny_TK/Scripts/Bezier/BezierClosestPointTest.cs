using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierClosestPointTest : MonoBehaviour
{

    public BezierSpline spline;
    public LineRenderer lineRenderer;
    public float closestTime;
    // Use this for initialization
    void Start()
    {
        lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, this.transform.position);
        lineRenderer.SetPosition(1, spline.ClosestPointOnBezier(this.transform.position));
        closestTime = spline.ClosestTimeOnBezier(this.transform.position);
    }
}
