using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bunny_TK.Utils
{
    //Tutto questo si risolve utilizzando un unico collider!
    public class CompoundColliderTrigger : MonoBehaviour
    {
        [Header("Behaviour Set-Up")]
        public bool removeDuplicates = true;

        /// <summary>
        /// Se vero, l'evento TriggerExit sarà lanciato solo quando tutti i ColliderTrigger (_colliders) sono usciti tutti
        /// </summary>
        public bool exitTriggerAll = false;

        /// <summary>
        /// Se vero, non manda eventi nel caso in cui il collider è all'interno di _colliders
        /// </summary>
        public bool ignoreSelfColliders = true;

        public bool autoAddColliderTriggers = false;

        [Header("ReadOnly")]
        [SerializeField]
        private List<ColliderTrigger> _colliders;

        [SerializeField]
        private List<Collider> _currentStayingColliders;

        public event ColliderTrigger.ColliderTriggerHandler
            TriggerEnter,
            TriggerStay,
            TriggerExit;

        public List<ColliderTrigger> ColliderTriggers
        {
            get { return new List<ColliderTrigger>(_colliders); }
        }

        public List<Collider> Colliders
        {
            get
            {
                //List<Collider> temp = new List<Collider>();
                //_colliders.ForEach(ct => temp.Add(ct.Collider));
                return _colliders.Select(ct => ct.Collider).ToList();
            }
        }

        public List<Collider> CurrentStayingColliders
        {
            get { return new List<Collider>(_currentStayingColliders); }
        }

        private void Start()
        {
            if (autoAddColliderTriggers)
                Setup();

            if (removeDuplicates)
                _colliders = _colliders.Distinct().ToList();

            foreach (ColliderTrigger ct in _colliders)
            {
                ct.TriggerEnter += ColliderTrigger_TriggerEnter;
                ct.TriggerStay += ColliderTrigger_TriggerStay;
                ct.TriggerExit += ColliderTrigger_TriggerExit;
            }
        }

        #region ColliderTrigger Handlers

        private void ColliderTrigger_TriggerEnter(object sender, ColliderTriggerEvent e)
        {
            ColliderTrigger s = sender as ColliderTrigger;

            if (ignoreSelfColliders)
                if (_colliders.Exists(c => c.Collider == e.collider))
                    return;

            if (_currentStayingColliders.IndexOf(e.collider) < 0)
            {
                _currentStayingColliders.Add(e.collider);
                OnTEnter(e.collider, s.Collider);
            }
        }

        private void ColliderTrigger_TriggerStay(object sender, ColliderTriggerEvent e)
        {
            ColliderTrigger s = sender as ColliderTrigger;

            if (ignoreSelfColliders)
                if (_colliders.Exists(c => c.Collider == e.collider))
                    return;

            foreach (Collider c in GetStayColliders())
                OnTStay(c, s.Collider);
        }

        private void ColliderTrigger_TriggerExit(object sender, ColliderTriggerEvent e)
        {
            ColliderTrigger s = sender as ColliderTrigger;
            if (ignoreSelfColliders)
                if (_colliders.Exists(c => c.Collider == e.collider))
                    return;

            if (!exitTriggerAll)
            {
                if (_currentStayingColliders.IndexOf(e.collider) >= 0)
                {
                    _currentStayingColliders.Remove(e.collider);
                    OnTExit(e.collider, s.Collider);
                    return;
                }
                return;
            }
            else
            {
                foreach (ColliderTrigger ct in _colliders)
                {
                    if (ct.CurrentStayingCollider.IndexOf(e.collider) >= 0)
                        return;
                }

                if (_currentStayingColliders.IndexOf(e.collider) >= 0)
                {
                    _currentStayingColliders.Remove(e.collider);
                    OnTExit(e.collider, s.Collider);
                    return;
                }
            }
        }

        #endregion ColliderTrigger Handlers

        #region Utility

        private List<Collider> GetStayColliders()
        {
            List<Collider> temp = new List<Collider>();
            foreach (ColliderTrigger ct in _colliders)
                temp.AddRange(ct.CurrentStayingCollider);

            return temp.Distinct().ToList();
        }

        public void AddColliderTrigger(ColliderTrigger newColliderTrigger)
        {
            if (_colliders.IndexOf(newColliderTrigger) < 0)
            {
                _colliders.Add(newColliderTrigger);
                newColliderTrigger.TriggerEnter += ColliderTrigger_TriggerEnter;
                newColliderTrigger.TriggerStay += ColliderTrigger_TriggerStay;
                newColliderTrigger.TriggerExit += ColliderTrigger_TriggerExit;
            }
            else
            {
                Debug.LogWarning("Adding a ColliderTrigger that is already in the list");
            }
        }

        public void RemoveColliderTrigger(ColliderTrigger colliderTrigger)
        {
            if (_colliders.IndexOf(colliderTrigger) >= 0)
            {
                _colliders.RemoveAll(c => c == colliderTrigger);
                colliderTrigger.TriggerEnter -= ColliderTrigger_TriggerEnter;
                colliderTrigger.TriggerStay -= ColliderTrigger_TriggerStay;
                colliderTrigger.TriggerExit -= ColliderTrigger_TriggerExit;
            }
            else
            {
                Debug.LogWarning("Removing a ColliderTrigger that isnt in the list");
            }
        }

        private void Setup()
        {
            _colliders.Clear();
            List<Collider> temp = GetComponentsInChildren<Collider>().ToList();
            for (int i = 0; i < temp.Count; i++)
            {
                ColliderTrigger ct = temp[i].GetComponent<ColliderTrigger>();
                if (ct == null) ct = temp[i].gameObject.AddComponent<ColliderTrigger>();
                _colliders.Add(ct);
            }
        }

        #endregion Utility

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
    }
}