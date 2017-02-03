using UnityEngine;

namespace UbiSolarSystem
{
    public class CameraFollow : MonoBehaviour
    {
        private Vector3 Offset;

        // Use this for initialization
        void Start()
        {
            Offset = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = Vector3.Lerp(transform.position, InputHandler.GetMousePositionInWorld() + Offset, Time.deltaTime * 0.5f);
        }
    }
}