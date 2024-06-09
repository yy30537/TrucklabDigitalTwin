using UnityEngine;

namespace Core
{
    // This script handles dragging vehiclesConfig
    public class DragVehicle : MonoBehaviour
    {
        public GetClickedObject getClickedObject;
        public bool isDraggingEnabled = false;
        public bool isDragging = false;
        
        [Header("Controlled Vehicles Reference")] 
        public GameObject clickedVehicle;
        public GameObject clickedTrailerObject;
        public GameObject clickedTractorObject;

        public Camera mainCamera;
        
        [Header("Scrolling Properties")] 
        public float scrollInput; 
        public float scrollSensitivity = 0.5f; 
        
        
        // Common Methods
        void Awake()
        {
            clickedVehicle = null;
            clickedTrailerObject = null;
            clickedTractorObject = null;
            isDraggingEnabled = false;
        }
        void Update()
        {
            if (isDraggingEnabled)
            {
                HandleDragAndDrop();
            }
        }
        private void HandleDragAndDrop()
        {
            if (Input.GetMouseButtonDown(0)) // inside this condition checks if tractorTransform/trailerTransform object is clicked 
            { 
                GameObject hitObject = getClickedObject.ReturnClickedObject();
                 
                if (hitObject != null)
                {
                    if (hitObject.CompareTag("Tractor"))
                    {
                        clickedTractorObject = hitObject;
                        clickedVehicle = hitObject.transform.parent.gameObject;
                        isDragging = true;
                    } else if (hitObject.CompareTag("Trailer"))
                    {
                        clickedTrailerObject = hitObject;
                        clickedVehicle = hitObject.transform.parent.gameObject;
                        isDragging = true;
                    }
                    else
                    {
                        clickedTractorObject = null;
                        clickedTrailerObject = null;
                        isDragging = false;
                    }
                }
            }
            if (isDragging && Input.GetMouseButton(0))
            {
                VehicleProduct vehicleProduct = clickedVehicle.GetComponent<VehicleProduct>();
                MoveSelectedVehicle(vehicleProduct);
            }
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                clickedVehicle = null;
                clickedTrailerObject = null;
                clickedTractorObject = null;
            }
        }
        private void MoveSelectedVehicle(VehicleProduct vehicleProduct)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 targetPoint = hit.point;
                targetPoint.y = clickedVehicle.transform.position.y; // Keep the currentTractorPosY position constant

                if (vehicleProduct != null)
                {
                    var scale = vehicleProduct.vehicleConfig.scale;

                    // Calculate parameters based on targetPoint
                    var x = targetPoint.x / scale; // Adjust as needed for scale
                    var y = targetPoint.z / scale; // Assuming Z-coordinate represents y in your scenario
                    var tractorAngle = Mathf.Rad2Deg * vehicleProduct.vehicleKinematics.psi1;
                    var trailerAngle = Mathf.Rad2Deg * vehicleProduct.vehicleKinematics.psi2;

                    // Update vehicleTransform position using SetVehiclePosition
                    vehicleProduct.vehicleKinematics.SetVehiclePosition(x, y, tractorAngle, trailerAngle);
                    HandleScrollInput(vehicleProduct); // during mouse down also handle scroll
                }
              
            }
        }
        private void HandleScrollInput(VehicleProduct vehicleProduct)
        {
            scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollInput) > 0.001) // Check if there is significant scroll input
            {
                AdjustVehicleAngles(vehicleProduct);
            }
        }
        private void AdjustVehicleAngles(VehicleProduct vehicleProduct)
        {
            if (vehicleProduct != null) 
            {
                if (clickedTractorObject != null)
                {
                    vehicleProduct.vehicleKinematics.SetTractorAngle(scrollInput, scrollSensitivity);
                } 
                else if (clickedTrailerObject != null)
                {
                    vehicleProduct.vehicleKinematics.SetTrailerAngle(scrollInput, scrollSensitivity);    
                }
                if (clickedTractorObject != null)
                {
                    vehicleProduct.vehicleKinematics.SetTractorAngle(scrollInput, scrollSensitivity);
                }
                else if (clickedTractorObject != null)
                {
                    vehicleProduct.vehicleKinematics.SetTrailerAngle(scrollInput, scrollSensitivity);    
                }
            }
        }
        public void ToggleControl()
        {
            isDraggingEnabled = !isDraggingEnabled;
        }
        
    }
}