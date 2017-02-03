using UnityEngine;

namespace UbiSolarSystem
{
    public class InputHandler : MonoBehaviour
    {
        /// <summary>
        /// Describes the current click state
        /// </summary>
        public enum CLICK_STATUS
        {
            /// <summary>
            /// No button is pressed
            /// </summary>
            HOVERING,
            /// <summary>
            /// The button is currently begin pressed (scope: frame)
            /// </summary>
            CLICKING,
            /// <summary>
            /// The button has already been pressed and is being held down
            /// </summary>
            DRAGGING,
            /// <summary>
            /// The button is currently being released (scope: frame)
            /// </summary>
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

        private CLICK_STATUS ClickStatus;
        private Planet SelectedPlanet;

        private Vector3 PreviousMousePosition;

        private void OnClickAction()
        {
            ClickStatus = CLICK_STATUS.CLICKING;
            SelectedPlanet = GetPlanetAtPosition(Input.mousePosition);
        }

        private void OnReleaseAction()
        {
            ClickStatus = CLICK_STATUS.RELEASING;
            Vector3 direction = GetMousePositionInWorld() - PreviousMousePosition;
            Cursor.visible = true;

            if (SelectedPlanet)
            {
                SelectedPlanet = null;
            }
        }

        private void OnDragAction()
        {
            ClickStatus = CLICK_STATUS.DRAGGING;
            if (SelectedPlanet == null)
            {
                return;
            }

            SelectedPlanet.gameObject.transform.position = Vector3.Lerp(SelectedPlanet.gameObject.transform.position, GetMousePositionInWorld(), Time.deltaTime * 3f);
            Cursor.visible = false;
            PreviousMousePosition = GetMousePositionInWorld();
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

        /// <summary>
        /// Returns the planet at a given position if it exists. Null otherwise.
        /// </summary>
        /// <param name="position">The position to look for a planet</param>
        /// <returns>The planet at given position</returns>
        private Planet GetPlanetAtPosition(Vector3 position)
        {
            RaycastHit hitInfo;
            Ray rayhit = Camera.main.ScreenPointToRay(position);

            // Raycast only on PLANET layer and ignore triggers
            Physics.Raycast(rayhit, out hitInfo, 1000, 1 << LayerManager.ToInt(LAYER.PLANET), QueryTriggerInteraction.Ignore);

            if (hitInfo.collider != null)
            {
                return hitInfo.collider.GetComponent<Planet>();
            }

            return null;            
        }

        /// <summary>
        /// Returns the world position of the mouse along the (0,0,0) plane
        /// </summary>
        /// <returns>The mouse position in the world</returns>
        public static Vector3 GetMousePositionInWorld()
        {
            RaycastHit hitInfo;
            Ray rayhit = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast only on the FLOOR layer and ignore triggers
            Physics.Raycast(rayhit, out hitInfo, 1000, 1 << LayerManager.ToInt(LAYER.FLOOR), QueryTriggerInteraction.Ignore);

            if (hitInfo.collider != null)
            {
                return hitInfo.point;
            }

            return Vector3.zero;
        }
    }
}