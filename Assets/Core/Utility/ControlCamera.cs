using UnityEngine;

namespace Core
{
    // This script manages the camera control
    public class ControlCamera : MonoBehaviour
    {
        public bool isActive = false;
        public Camera mainCamera;
        
        [Header("Camera Movement Control Properties")] 
        public float movementSpeed;
        public float cameraVerticalSpeed;
        public float cameraDragSpeed;
        public float mouseSensitivity;
        public float zoomSensitivity;
        public Vector3 lastMousePosition;
        public float cameraHeight = 0f;

        public float fov;
        private void Awake()
        {
            fov = mainCamera.fieldOfView;
            isActive = true;
        }
        void OnDestroy()
        {
        }

        // Specific Methods
        private void Update()
        {
            if (isActive)
            {
                // Mouse rotation
                if (Input.GetMouseButtonDown(1)) // Right mouse button
                {
                    lastMousePosition = Input.mousePosition;
                }
                
                if (Input.GetMouseButton(1)) // Right mouse button held down
                {
                    Vector3 delta = Input.mousePosition - lastMousePosition;
                    float rotationX = -delta.y * mouseSensitivity * Time.deltaTime; // Inverted currentTractorPosY
                    float rotationY = delta.x * mouseSensitivity * Time.deltaTime; // Inverted currentTractorPosX

                    mainCamera.transform.eulerAngles += new Vector3(rotationX, rotationY, 0);
                    lastMousePosition = Input.mousePosition;
                }
                
                // Camera movement using middle mouse button
                if (Input.GetMouseButton(2)) // Middle mouse button held down
                {
                    float middleMouseX = Input.GetAxis("Mouse X") * cameraDragSpeed * Time.deltaTime * (-1);
                    float middleMouseY = Input.GetAxis("Mouse Y") * cameraDragSpeed * Time.deltaTime * (-1);

                    mainCamera.transform.Translate(middleMouseX, 0, middleMouseY);
                }
                
                // WASD movement
                float x = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
                float z = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
                mainCamera.transform.Translate(x, 0, z);
                
                // Raise camera using spacebar
                if (Input.GetKey(KeyCode.Space))
                {
                    cameraHeight += cameraVerticalSpeed * Time.deltaTime;
                    mainCamera.transform.position += new Vector3(0, cameraHeight, 0);
                }

                // Camera zoom
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                mainCamera.fieldOfView -= scroll * zoomSensitivity;
            }
        }
        
        
        public void ToggleControl()
        {
            isActive = !isActive;
        }
        
    }
}

