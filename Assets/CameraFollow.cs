using UnityEngine;

namespace UbiSolarSystem
{
    public class CameraFollow : MonoBehaviour
    {
        [Range(0.1f, 5f)]
        public float FollowSpeed = 0.5f;

        private Vector3 Offset;

        // Use this for initialization
        void Start()
        {
            // Base the offset on the default scene camera position
            Offset = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = Vector3.Lerp(transform.position, InputHandler.GetMousePositionInWorld() + Offset, Time.deltaTime * FollowSpeed);
        }
    }
}