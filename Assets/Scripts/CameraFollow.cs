using UnityEngine;

namespace UbiSolarSystem
{
    public class CameraFollow : MonoBehaviour
    {
        [Range(0.1f, 5f)]
        public float FollowSpeed = 0.5f;

        private Vector3 Offset;
        Vector3 scrollOffset;

        // Use this for initialization
        void Start()
        {
            // Base the offset on the default scene camera position
            Offset = transform.position;
            scrollOffset = Offset;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                scrollOffset += Offset * -Input.mouseScrollDelta.y;
                scrollOffset *= FollowSpeed;
            }

            transform.position = Vector3.Lerp(transform.position, InputHandler.GetMousePositionInWorld() + scrollOffset, Time.deltaTime * FollowSpeed);
        }
    }
}