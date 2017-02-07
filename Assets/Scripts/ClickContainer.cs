namespace UbiSolarSystem
{
    public class ClickContainer
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

        public enum MOUSE_BUTTON
        {
            LEFT = 0,
            RIGHT = 1
        }

        #region Events
        public delegate void ClickAction();
        public event ClickAction OnClickEvent;

        public delegate void ReleaseAction();
        public event ReleaseAction OnReleaseEvent;

        public delegate void DragAction();
        public event DragAction OnDragEvent;
        #endregion

        public MOUSE_BUTTON MouseButton;
        public CLICK_STATUS ClickStatus;

        public ClickContainer(MOUSE_BUTTON mb)
        {
            MouseButton = mb;
            ClickStatus = CLICK_STATUS.HOVERING;
        }

        public void Click()
        {
            ClickStatus = CLICK_STATUS.CLICKING;
            if (OnClickEvent != null)
            {
                OnClickEvent();
            }
        }

        public void Drag()
        {
            ClickStatus = CLICK_STATUS.DRAGGING;
            if (OnDragEvent != null)
            {
                OnDragEvent();
            }
        }

        public void Release()
        {
            ClickStatus = CLICK_STATUS.RELEASING;
            if (OnReleaseEvent != null)
            {
                OnReleaseEvent();
            }
        }

        public void Hover()
        {
            ClickStatus = CLICK_STATUS.HOVERING;
        }
    }
}