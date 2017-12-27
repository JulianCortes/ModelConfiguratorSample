using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bunny_TK.Utils
{
    public class MatryoshkaColliders : MonoBehaviour

    {
        [SerializeField]
        private List<ColliderTrigger> _colliders;

        public List<ColliderTrigger> Colliders
        {
            get { return new List<ColliderTrigger>(_colliders); }
        }

        public bool autoSearchColliderTriggers;
        public bool greaterOrEqual = false;

        public ColliderTrigger deepestColliderEnter;
        public ColliderTrigger deepestColliderExit;
        public ColliderTrigger deepestColliderStay;

        public event ColliderTrigger.ColliderTriggerHandler
                        TriggerEnter,
                        TriggerStay,
                        TriggerExit;

        private void Start()
        {
            if (autoSearchColliderTriggers)
                _colliders = GetComponentsInChildren<ColliderTrigger>().ToList();

            for (int i = 0; i < _colliders.Count; i++)
            {
                _colliders[i].TriggerEnter += MatryoshkaColliders_TriggerEnter;
                _colliders[i].TriggerExit += MatryoshkaColliders_TriggerExit;
                _colliders[i].TriggerStay += MatryoshkaColliders_TriggerStay;
            }
        }

        #region TriggerHandler

        private void MatryoshkaColliders_TriggerEnter(object sender, ColliderTriggerEvent e)
        {
            ColliderTrigger ct = sender as ColliderTrigger;
            if (deepestColliderEnter == null)
            {
                deepestColliderEnter = ct;
                OnTEnter(e.collider, ct.Collider);
            }
            else
            {
                if (greaterOrEqual)
                {
                    if (deepestColliderEnter.depth >= ct.depth)
                    {
                        deepestColliderEnter = ct;
                        OnTEnter(e.collider, ct.Collider);
                    }
                }
                else
                {
                    if (deepestColliderEnter.depth > ct.depth)
                    {
                        deepestColliderEnter = ct;
                        OnTEnter(e.collider, ct.Collider);
                    }
                }
            }
        }

        private void MatryoshkaColliders_TriggerStay(object sender, ColliderTriggerEvent e)
        {
            ColliderTrigger ct = sender as ColliderTrigger;
            if (deepestColliderStay == null)
            {
                deepestColliderStay = ct;
                OnTStay(e.collider, ct.Collider);
            }
            else
            {
                if (greaterOrEqual)
                {
                    if (deepestColliderStay.depth >= ct.depth)
                    {
                        deepestColliderStay = ct;
                        OnTStay(e.collider, ct.Collider);
                    }
                }
                else
                {
                    if (deepestColliderStay.depth > ct.depth)
                    {
                        deepestColliderStay = ct;
                        OnTStay(e.collider, ct.Collider);
                    }
                }
            }
        }

        private void MatryoshkaColliders_TriggerExit(object sender, ColliderTriggerEvent e)
        {
            ColliderTrigger ct = sender as ColliderTrigger;
            deepestColliderExit = ct;
            if (deepestColliderExit == deepestColliderEnter)
                deepestColliderEnter = null;

            if (deepestColliderExit == deepestColliderStay)
                deepestColliderStay = null;
            OnTExit(e.collider, ct.Collider);

            //if (deepestColliderExit == null)
            //{
            //    deepestColliderExit = ct;
            //    OnTExit(e.collider, ct.Collider);
            //}
            //else
            //{
            //    if (greaterOrEqual)
            //    {
            //        if (deepestColliderExit.depth <= ct.depth)
            //        {
            //            deepestColliderExit = ct;
            //            OnTExit(e.collider, ct.Collider);
            //        }
            //    }
            //    else
            //    {
            //        if (deepestColliderExit.depth < ct.depth)
            //        {
            //            deepestColliderExit = ct;
            //            OnTExit(e.collider, ct.Collider);
            //        }
            //    }

            //    if (deepestColliderExit == deepestColliderEnter)
            //        deepestColliderEnter = null;

            //    if (deepestColliderExit == deepestColliderStay)
            //        deepestColliderStay = null;
            //}
        }

        #endregion TriggerHandler

        #region Event Launchers

        private void OnTEnter(Collider collider, Collider triggeredCollider)
        {
            if (TriggerEnter != null)
                TriggerEnter(this, new ColliderTriggerEvent(collider, triggeredCollider));
        }

        private void OnTStay(Collider collider, Collider triggeredCollider)
        {
            if (TriggerStay != null)
                TriggerStay(this, new ColliderTriggerEvent(collider, triggeredCollider));
        }

        private void OnTExit(Collider collider, Collider triggeredCollider)
        {
            if (TriggerExit != null)
                TriggerExit(this, new ColliderTriggerEvent(collider, triggeredCollider));
        }

        #endregion Event Launchers

        public List<ColliderTrigger> GetOuterColliders(ColliderTrigger colliderTrigger, bool greaterOrEqual = false)
        {
            if (greaterOrEqual)
                return _colliders.FindAll(c => c.depth >= colliderTrigger.depth);
            else
                return _colliders.FindAll(c => c.depth > colliderTrigger.depth);
        }
    }
}