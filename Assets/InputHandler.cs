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

        [Range(0f, 5f)]
        public float PlanetDragSpeed = 3f;

        private CLICK_STATUS ClickStatus;
        private Planet SelectedPlanet;

        void Start()
        {
            ClickStatus = CLICK_STATUS.HOVERING;

            // Register all the events
            OnClickEvent += SelectPlanet;
            OnReleaseEvent += ReleasePlanet;
            OnDragEvent += DragPlanet;
        }

        void Update()
        {
            // Left click pressed
            if (Input.GetMouseButtonDown(0) && ClickStatus == CLICK_STATUS.HOVERING)
            {
                ClickStatus = CLICK_STATUS.CLICKING;

                if (OnClickEvent != null)
                {
                    OnClickEvent();
                }
            }

            // Left click holded
            if (Input.GetMouseButton(0) && (ClickStatus == CLICK_STATUS.DRAGGING || ClickStatus == CLICK_STATUS.CLICKING))
            {
                ClickStatus = CLICK_STATUS.DRAGGING;

                if (OnDragEvent != null)
                {
                    OnDragEvent();
                }
            }

            // Left click released
            if (Input.GetMouseButtonUp(0) && (ClickStatus == CLICK_STATUS.DRAGGING || ClickStatus == CLICK_STATUS.CLICKING))
            {
                ClickStatus = CLICK_STATUS.RELEASING;

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
        /// Tries to select a planet at mouse position
        /// </summary>
        private void SelectPlanet()
        {
            SelectedPlanet = GetPlanetAtPosition(Input.mousePosition);
        }

        /// <summary>
        /// Tries to drag the held planet
        /// </summary>
        private void DragPlanet()
        {
            if (SelectedPlanet != null)
            {
                SelectedPlanet.gameObject.transform.position = Vector3.Lerp(SelectedPlanet.gameObject.transform.position, GetMousePositionInWorld(), Time.deltaTime * PlanetDragSpeed);
                Cursor.visible = false;
            }
        }

        /// <summary>
        /// Tries to release the held planet
        /// </summary>
        private void ReleasePlanet()
        {
            Cursor.visible = true;
            SelectedPlanet = null;
        }

        #region Helper functions
        /// <summary>
        /// Returns the planet at a given position if it exists. Null otherwise.
        /// </summary>
        /// <param name="position">The position to look for a planet</param>
        /// <returns>The planet at given position</returns>
        public static Planet GetPlanetAtPosition(Vector3 position)
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
        #endregion
    }
}