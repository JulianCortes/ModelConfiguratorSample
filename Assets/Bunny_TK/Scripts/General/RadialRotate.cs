using UnityEngine;
using UnityEngine.EventSystems;

namespace Bunny_TK.Utils
{
    public class RadialRotate : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler

    {
        //TODO
        //Selezione dell'angolo su cui ruotare

        public float snappingAngle = 1f;

        [SerializeField]
        private int currentIndex = 0;

        public Camera myCam;

        [Header("EDITOR ONLY")]
        [SerializeField]
        private bool updateRotationFromCurrentIndex = false;

        [SerializeField]
        private int targetIndex = 0;

        public event System.EventHandler Drag;

        public event System.EventHandler EndDrag;

        /// <summary>
        /// Value is updated during OnDrag & OnEndDrag
        /// </summary>
        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                currentIndex = value;

                Vector3 tmp = transform.localRotation.eulerAngles;
                tmp.z = snappingAngle * currentIndex;
                transform.localRotation = Quaternion.Euler(tmp.x, tmp.y, tmp.z);

                //TODO Event when current Index is updated
                //This is a work around for now
                if (Drag != null) Drag(this, System.EventArgs.Empty);
                if (EndDrag != null) EndDrag(this, System.EventArgs.Empty);
            }
        }

        private Vector3 screenPos;
        private float angleOffset;

        #region UNITY CALLBACK

        private void Start()
        {
            if (myCam == null) myCam = Camera.main;

            var vec = transform.localRotation.eulerAngles;
            vec.z = Mathf.Round(vec.z / snappingAngle) * snappingAngle;
            vec.z = vec.z == 360 ? 0 : vec.z;

            transform.localRotation = Quaternion.Euler(vec.x, vec.y, vec.z);
            currentIndex = Mathf.RoundToInt(vec.z / snappingAngle);
        }

        private void Update()
        {
            if (updateRotationFromCurrentIndex)
            {
                CurrentIndex = targetIndex;
                updateRotationFromCurrentIndex = false;
            }
        }

        #endregion UNITY CALLBACK

        #region DRAG INTERFACE

        public void OnBeginDrag(PointerEventData eventData)
        {
            screenPos = myCam.WorldToScreenPoint(transform.position);
            Vector3 v3 = Input.mousePosition - screenPos;
            angleOffset = (Mathf.Atan2(transform.right.y, transform.right.x) - Mathf.Atan2(v3.y, v3.x)) * Mathf.Rad2Deg;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 v3 = Input.mousePosition - screenPos;
            float angle = Mathf.Atan2(v3.y, v3.x) * Mathf.Rad2Deg;
            float targetAngle = angle + angleOffset;
            targetAngle = targetAngle == 360f ? 0 : targetAngle;
            transform.localRotation = Quaternion.Euler(0, 0, targetAngle);

            float temp = transform.localRotation.eulerAngles.z;
            temp = Mathf.Round(temp / snappingAngle) * snappingAngle;
            temp = temp == 360f ? 0f : temp;
            currentIndex = Mathf.RoundToInt(temp / snappingAngle);

            if (Drag != null)
                Drag(this, System.EventArgs.Empty);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var vec = transform.localRotation.eulerAngles;
            vec.z = Mathf.Round(vec.z / snappingAngle) * snappingAngle;
            vec.z = vec.z == 360 ? 0 : vec.z;                            //Work around

            transform.localRotation = Quaternion.Euler(vec.x, vec.y, vec.z);

            currentIndex = Mathf.RoundToInt(vec.z / snappingAngle);      //Per ora considero solo la z

            if (EndDrag != null)
                EndDrag(this, System.EventArgs.Empty);
            return;
        }

        #endregion DRAG INTERFACE

        #region WRAPPER

        //Wrapper per i bottone di Unity
        public void SetIndex(int index)
        {
            CurrentIndex = index;
        }

        public void SetCurrentIndexNoEvents(int index)
        {
            currentIndex = index;
            Vector3 tmp = transform.localRotation.eulerAngles;
            tmp.z = snappingAngle * currentIndex;
            transform.localRotation = Quaternion.Euler(tmp.x, tmp.y, tmp.z);
        }

        #endregion WRAPPER
    }
}