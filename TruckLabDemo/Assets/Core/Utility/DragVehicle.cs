using Unity.VisualScripting;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Handles dragging and dropping of VehicleConfigs.
    /// </summary>
    public class DragVehicle : MonoBehaviour
    {
        public GetClickedObject getClickedObject;
        public bool isDraggingEnabled;
        public bool isDragging = false;

        [Header("Controlled Vehicles Reference")]
        public GameObject clickedVehicle;
        public GameObject clickedTrailerObject;
        public GameObject clickedTractorObject;

        public Camera mainCamera;

        [Header("Scrolling Properties")]
        public float scrollInput;
        public float scrollSensitivity = 0.2f;
        public SystemLogWindow SystemLogWindow;

        // Initialize variables
        void Awake()
        {
            clickedVehicle = null;
            clickedTrailerObject = null;
            clickedTractorObject = null;
            isDraggingEnabled = true;
        }

        // Handle drag and drop each frame
        void Update()
        {
            if (isDraggingEnabled)
            {
                HandleDragAndDrop();
            }
        }

        /// <summary>
        /// Handles the drag and drop logic for vehicles.
        /// </summary>
        private void HandleDragAndDrop()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject hitObject = getClickedObject.ReturnClickedObject();

                if (hitObject != null)
                {
                    if (hitObject.CompareTag("Tractor"))
                    {
                        clickedTractorObject = hitObject;
                        clickedVehicle = hitObject.transform.parent.gameObject;
                        isDragging = true;
                    }
                    else if (hitObject.CompareTag("Trailer"))
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

        /// <summary>
        /// Moves the selected vehicle based on mouse position.
        /// </summary>
        /// <param name="vehicleProduct">The vehicle product to move.</param>
        private void MoveSelectedVehicle(VehicleProduct vehicleProduct)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 targetPoint = hit.point;
                targetPoint.y = clickedVehicle.transform.position.y; // Keep the current Y position constant

                if (vehicleProduct != null)
                {
                    var scale = vehicleProduct.Config.Scale;

                    // Calculate parameters based on targetPoint
                    var x = targetPoint.x / scale;
                    var y = targetPoint.z / scale;

                    var tractorAngle = Mathf.Rad2Deg * vehicleProduct.Kinematics.psi1;
                    var trailerAngle = Mathf.Rad2Deg * vehicleProduct.Kinematics.psi2;

                    // Update vehicleTransform position using SetVehiclePosition
                    vehicleProduct.Kinematics.SetVehiclePosition(x, y, tractorAngle, trailerAngle);
                    HandleScrollInput(vehicleProduct); // during mouse down also handle scroll
                }
            }
        }

        /// <summary>
        /// Handles scroll input to adjust vehicle angles.
        /// </summary>
        /// <param name="vehicleProduct">The vehicle product to adjust.</param>
        private void HandleScrollInput(VehicleProduct vehicleProduct)
        {
            scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollInput) > 0.001)
            {
                AdjustVehicleAngles(vehicleProduct);
            }
        }

        /// <summary>
        /// Adjusts the angles of the tractor or trailer based on scroll input.
        /// </summary>
        /// <param name="vehicleProduct">The vehicle product to adjust.</param>
        private void AdjustVehicleAngles(VehicleProduct vehicleProduct)
        {
            if (vehicleProduct != null)
            {
                if (clickedTractorObject != null)
                {
                    vehicleProduct.Kinematics.SetTractorAngle(scrollInput, scrollSensitivity);
                }
                else if (clickedTrailerObject != null)
                {
                    vehicleProduct.Kinematics.SetTrailerAngle(scrollInput, scrollSensitivity);
                }
            }
        }

        /// <summary>
        /// Toggles the drag and drop control on or off.
        /// </summary>
        public void ToggleControl()
        {
            isDraggingEnabled = !isDraggingEnabled;
            SystemLogWindow.LogEvent($"Vehicle Drag & Drop: {isDraggingEnabled}");
        }
    }
}
