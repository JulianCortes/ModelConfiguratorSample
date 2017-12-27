using Bunny_TK.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public bool running = true;

    [SerializeField]
    private ColliderTrigger _enter_colliderTrigger;

    [SerializeField]
    private ColliderTrigger _blocking_colliderTrigger;

    [SerializeField]
    private MeshRenderer _portalMeshRenderer;

    /// <summary>
    /// This teleporter exit pos
    /// </summary>
    public Transform exitPosition;

    [SerializeField]
    private Teleporter _connectedTeleporter;

    public bool ignoreTags = false;
    public List<string> teleportableTags = new List<string>() { "Player" };

    #region UNITY CALLBACKS

    private void OnEnable()
    {
        if (_enter_colliderTrigger != null)
            _enter_colliderTrigger.TriggerEnter += Enter_ColliderTrigger_TriggerEnter;

        if (_blocking_colliderTrigger != null)
        {
            _blocking_colliderTrigger.TriggerEnter += Blocking_ColliderTrigger_TriggerEnter;
            _blocking_colliderTrigger.TriggerExit += Blocking_ColliderTrigger_TriggerExit;
            _blocking_colliderTrigger.TriggerStay += Blocking_ColliderTrigger_TriggerStay;
        }
    }

    private void OnDisable()
    {
        if (_enter_colliderTrigger != null)
            _enter_colliderTrigger.TriggerEnter -= Enter_ColliderTrigger_TriggerEnter;

        if (_blocking_colliderTrigger != null)
        {
            _blocking_colliderTrigger.TriggerEnter -= Blocking_ColliderTrigger_TriggerEnter;
            _blocking_colliderTrigger.TriggerExit -= Blocking_ColliderTrigger_TriggerExit;
            _blocking_colliderTrigger.TriggerStay -= Blocking_ColliderTrigger_TriggerStay;
        }
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    #endregion UNITY CALLBACKS

    #region BLOCKING COLLIDER TRIGGER

    private void Blocking_ColliderTrigger_TriggerEnter(object sender, ColliderTriggerEvent e)
    {
        //throw new System.NotImplementedException();
    }

    private void Blocking_ColliderTrigger_TriggerStay(object sender, ColliderTriggerEvent e)
    {
        //throw new NotImplementedException();
    }

    private void Blocking_ColliderTrigger_TriggerExit(object sender, ColliderTriggerEvent e)
    {
        //throw new NotImplementedException();
    }

    #endregion BLOCKING COLLIDER TRIGGER

    #region ENTER COLLIDER TRIGGER

    private void Enter_ColliderTrigger_TriggerEnter(object sender, ColliderTriggerEvent e)
    {
        print("Attempt");
        TeleportMe(e.collider.transform, ignoreTags);
    }

    #endregion ENTER COLLIDER TRIGGER

    public void TeleportMe(Transform t, bool ignoreTag = false)
    {
        if (_connectedTeleporter == null)
        {
            Debug.Log("Connected teleporter is NULL");
            return;
        }
        if (ignoreTag && teleportableTags.Count == 0)
        {
            Debug.Log("No tags!");
            return;
        }
        if (!running) return;
        if (!CheckIfTeleportable(t)) return;
        if (!_connectedTeleporter.CheckIfTeleportable(t)) return;
        if (!ignoreTag && !teleportableTags.Contains(t.tag)) return;

        _connectedTeleporter.running = false;
        _connectedTeleporter.SetVisible_Portal(false);

        Vector3 targetPos = transform.InverseTransformPoint(t.transform.position);
        targetPos = _connectedTeleporter.transform.TransformPoint(targetPos);
        t.transform.position = targetPos;
        //t.transform.rotation = _connectedTeleporter.exitPosition.rotation;
        t.transform.position += t.transform.forward * .3f;

        Invoke("EnableOtherTeleporter", .25f);
        Invoke("EnableOtherPortalMeshRenderer", .25f);
        Invoke("Flip", .25f);
        print("teleported");
    }

    public void SetVisible_Portal(bool isVisible)
    {
        _portalMeshRenderer.enabled = isVisible;
    }

    public void EnableOtherTeleporter()
    {
        _connectedTeleporter.running = true;
    }
    public void EnableOtherPortalMeshRenderer()
    {
        print("enable");
        _connectedTeleporter.SetVisible_Portal(true);
    }
    
    public void Flip()
    {
        this.transform.forward = -this.transform.forward;
        _connectedTeleporter.transform.forward = -_connectedTeleporter.transform.forward;
    }

    public bool CheckIfTeleportable(Transform t)
    {
        if (_blocking_colliderTrigger != null)
        {
            if (_blocking_colliderTrigger.CurrentStayingCollider.Exists(c => c.transform == t))
            {
                print("Blocked");
                return false;
            }
        }
        return true;
    }
}