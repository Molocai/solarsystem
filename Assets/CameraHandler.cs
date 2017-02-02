using UnityEngine;

namespace UbiSolarSystem
{
    public class CameraHandler : MonoBehaviour
    {
        public enum CLICK_STATUS
        {
            HOVERING,
            CLICKING,
            DRAGGING,
            RELEASING
        }

        #region Events
        public delegate void ClickAction();
        public static event ClickAction OnClickEvent;

        public delegate void ReleaseAction();
        public static event ReleaseAction OnReleaseEvent;

        public delegate void DragAction();
        public static event DragAction OnDragEvent;
        #endregion

        public CLICK_STATUS ClickStatus;
        public Planet SelectedPlanet;

        private void OnClickAction()
        {
            ClickStatus = CLICK_STATUS.CLICKING;
            SelectedPlanet = GetPlanetAtPosition(Input.mousePosition);
        }

        private void OnReleaseAction()
        {
            ClickStatus = CLICK_STATUS.RELEASING;
            SelectedPlanet = null;
        }

        private void OnDragAction()
        {
            ClickStatus = CLICK_STATUS.DRAGGING;
            if (SelectedPlanet == null)
            {
                return;
            }

            SelectedPlanet.gameObject.transform.position = GetMousePositionInWorld();
        }

        void Start()
        {
            ClickStatus = CLICK_STATUS.HOVERING;

            // Register all the events
            OnClickEvent += OnClickAction;
            OnReleaseEvent += OnReleaseAction;
            OnDragEvent += OnDragAction;
        }

        void Update()
        {
            // Left click pressed
            if (Input.GetMouseButtonDown(0) && ClickStatus == CLICK_STATUS.HOVERING)
            {
                if (OnClickEvent != null)
                {
                    OnClickEvent();
                }
            }

            // Left click holded
            if (Input.GetMouseButton(0) && (ClickStatus == CLICK_STATUS.DRAGGING || ClickStatus == CLICK_STATUS.CLICKING))
            {
                if (OnDragEvent != null)
                {
                    OnDragEvent();
                }
            }

            // Left click released
            if (Input.GetMouseButtonUp(0) && (ClickStatus == CLICK_STATUS.DRAGGING || ClickStatus == CLICK_STATUS.CLICKING))
            {
                if (OnReleaseEvent != null)
                {
                    OnReleaseEvent();
                }
            }

            // Not doing anything
            if (!Input.GetMouseButton(0) && ClickStatus == CLICK_STATUS.RELEASING)
            {
                ClickStatus = CLICK_STATUS.HOVERING;
            }
        }

        private Planet GetPlanetAtPosition(Vector3 position)
        {
            RaycastHit hitInfo;
            Ray rayhit = Camera.main.ScreenPointToRay(position);

            Physics.Raycast(rayhit, out hitInfo, 1000, 1 << LayerMask.NameToLayer("Planet"), QueryTriggerInteraction.Ignore);

            if (hitInfo.collider != null)
            {
                return hitInfo.collider.GetComponent<Planet>();
            }

            return null;
        }

        private Vector3 GetMousePositionInWorld()
        {
            RaycastHit hitInfo;
            Ray rayhit = Camera.main.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(rayhit, out hitInfo, 1000, 1 << LayerMask.NameToLayer("Floor"), QueryTriggerInteraction.Ignore);

            if (hitInfo.collider != null)
            {
                return hitInfo.point;
            }

            return Vector3.zero;
        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;

        //    RaycastHit hitInfo;
        //    Ray rayhit = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    Physics.Raycast(rayhit, out hitInfo, 1000, 1 << LayerMask.NameToLayer("Floor"), QueryTriggerInteraction.Ignore);

        //    if (hitInfo.collider != null)
        //    {
        //        Gizmos.DrawWireSphere(hitInfo.point, 1f);
        //    }
        //}
    }
}