using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Bunny_TK.Utils
{

    [RequireComponent(typeof(Collider))]
    public class ColliderTrigger : MonoBehaviour
    {
        public bool removeDisabledGameObjects = true;

        [Header("For MatryoshkaColliders")]
        public int depth = 0;

        [Header("Read Only")]
        [SerializeField]
        Collider _collider;

        [SerializeField]
        List<Collider> currentStayingColliders;

        public delegate void ColliderTriggerHandler(object sender, ColliderTriggerEvent e);
        public event ColliderTriggerHandler
            TriggerEnter,
            TriggerStay,
            TriggerExit;

        public Collider Collider { get { return _collider; } }
        public List<Collider> CurrentStayingCollider
        {
            get { return new List<Collider>(currentStayingColliders); }
        }
        void Start()
        {

            if (currentStayingColliders == null)
            {
                currentStayingColliders = new List<Collider>();
            }

            _collider = GetComponent<Collider>();
            if (_collider == null)
                Debug.LogWarning("Collider not found!");
            else
            {
                if (!_collider.isTrigger)
                    Debug.LogWarning("Collider is not trigger");
            }
        }

        //void Update()
        //{
        //    RemoveDisabledGameObject()
        //}

        #region Triggers
        public void OnTriggerEnter(Collider c)
        {
            if (currentStayingColliders.IndexOf(c) < 0)
                currentStayingColliders.Add(c);

            OnTEnter(c);
        }
        public void OnTriggerStay(Collider c)
        {
            //Unneccesary?
            if (currentStayingColliders.IndexOf(c) < 0)
            {
                currentStayingColliders.Add(c);
                Debug.LogWarning("Collider " + c.name + " is staying w/o triggering enter");
            }

            OnTStay(c);
        }
        public void OnTriggerExit(Collider c)
        {

            if (currentStayingColliders.IndexOf(c) >= 0)
                currentStayingColliders.Remove(c);

            OnTExit(c);
        }
        #endregion Triggers

        #region Event Launchers
        private void OnTEnter(Collider go)
        {
            if (TriggerEnter != null)
                TriggerEnter(this, new ColliderTriggerEvent(go));
        }
        private void OnTStay(Collider go)
        {
            if (TriggerStay != null)
                TriggerStay(this, new ColliderTriggerEvent(go));
        }
        private void OnTExit(Collider go)
        {
            if (TriggerExit != null)
                TriggerExit(this, new ColliderTriggerEvent(go));
        }
        #endregion Event Launchers

        #region Utility
        public void RemoveDisabledGameObject(bool removeColliderDisabled = false, bool sendExitTriggerEvent = false)
        {
            List<Collider> temp = new List<Collider>();

            if (removeColliderDisabled)
                temp = currentStayingColliders.FindAll(c => !c.gameObject.activeSelf || !c.enabled);
            else
                temp = currentStayingColliders.FindAll(c => !c.gameObject.activeSelf);

            foreach (Collider c in temp)
            {
                currentStayingColliders.Remove(c);
                if (sendExitTriggerEvent)
                    OnTExit(c);
            }
        }
        #endregion Utility
    }

    public class ColliderTriggerEvent : System.EventArgs
    {
        /// <summary>
        /// Collider that caused the event
        /// </summary>
        public Collider collider;
        public Collider triggeredCollider;

        public ColliderTriggerEvent() : base()
        {
            collider = null;
            triggeredCollider = null;
        }

        public ColliderTriggerEvent(Collider collider, Collider triggeredCollider = null)
        {
            this.collider = collider;
            this.triggeredCollider = triggeredCollider;
        }
    }
}
