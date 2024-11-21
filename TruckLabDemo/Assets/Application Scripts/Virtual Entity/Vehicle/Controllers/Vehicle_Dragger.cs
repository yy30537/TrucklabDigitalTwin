using Application_Scripts.Manager;
using Application_Scripts.UI_Controller.Application_UI;
using Application_Scripts.Virtual_Entity.Vehicle.Controllers.Collision_Control;
using UnityEngine;
using UnityEngine.UI;

namespace Application_Scripts.Virtual_Entity.Vehicle.Controllers
{
    /// <summary>
    /// Handles the drag-and-drop operations for vehicle instances in the simulation.
    /// Provides functionality for selecting, moving, and adjusting vehicle position and orientation
    /// based on user interaction with the mouse and scroll wheel.
    /// </summary>
    public class Vehicle_Dragger : MonoBehaviour
    {
        /// <summary>
        /// Indicates whether the drag-and-drop functionality is enabled.
        /// Controlled via the UI.
        /// </summary>
        [Header("Status")]
        public bool IsDraggingEnabled;

        /// <summary>
        /// Tracks whether a vehicle is currently being dragged.
        /// </summary>
        public bool IsDragging;

        /// <summary>
        /// Captures the scroll wheel input from the user.
        /// </summary>
        [Header("Scrolling Properties")]
        public float ScrollInput;

        /// <summary>
        /// Sensitivity multiplier for adjusting vehicle angles during scrolling.
        /// </summary>
        public float ScrollSensitivity = 0.2f;

        /// <summary>
        /// Reference to the system log UI controller for logging drag-and-drop events.
        /// </summary>
        public UI_Controller_SystemLog UiControllerSystemLog;

        /// <summary>
        /// The currently selected vehicle for drag-and-drop operations.
        /// </summary>
        [Header("Controlled Vehicles Reference")]
        public GameObject ClickedVehicle;

        /// <summary>
        /// Reference to the clicked trailer object, if applicable.
        /// </summary>
        public GameObject ClickedTrailerObject;

        /// <summary>
        /// Reference to the clicked tractor object, if applicable.
        /// </summary>
        public GameObject ClickedTractorObject;

        /// <summary>
        /// Reference to the click detection utility for identifying clicked objects.
        /// </summary>
        [Header("Application Dependencies")]
        public VE_OnClick_Getter VeOnClickGetterGetter;

        /// <summary>
        /// Reference to the camera manager for raycasting operations.
        /// </summary>
        public Camera_Manager CameraManager;

        /// <summary>
        /// UI toggle element controlling whether dragging is enabled.
        /// </summary>
        [Header("UI Objects")]
        public Toggle Toggle;

        /// <summary>
        /// Initializes the state of the dragger by clearing any selected vehicles
        /// and disabling drag-and-drop functionality by default.
        /// </summary>
        void Awake()
        {
            ClickedVehicle = null;
            ClickedTrailerObject = null;
            ClickedTractorObject = null;
            IsDraggingEnabled = false;
        }

        /// <summary>
        /// Checks if dragging functionality is enabled and processes drag-and-drop operations.
        /// </summary>
        void Update()
        {
            if (IsDraggingEnabled)
            {
                HandleDragAndDrop();
            }
        }

        /// <summary>
        /// Toggles the drag-and-drop functionality based on the UI toggle state.
        /// Logs the updated status to the system log.
        /// </summary>
        public void OnMenuToggle()
        {
            IsDraggingEnabled = Toggle.isOn;
            UiControllerSystemLog.LogEvent($"Vehicle Drag & Drop: {IsDraggingEnabled}");
        }

        /// <summary>
        /// Handles the drag-and-drop logic for vehicles, including selection,
        /// movement, and release of the vehicle.
        /// </summary>
        private void HandleDragAndDrop()
        {
            if (Input.GetMouseButtonDown(0)) // Detect mouse click
            {
                GameObject hitObject = VeOnClickGetterGetter.ReturnClickedObject();

                if (hitObject != null)
                {
                    if (hitObject.CompareTag("Tractor"))
                    {
                        ClickedTractorObject = hitObject;
                        ClickedVehicle = hitObject.transform.parent.gameObject;
                        ClickedVehicle.GetComponent<Vehicle_Collision_Controller>().SensorSphereCollider.enabled = false;
                        IsDragging = true;
                    }
                    else if (hitObject.CompareTag("Trailer"))
                    {
                        ClickedTrailerObject = hitObject;
                        ClickedVehicle = hitObject.transform.parent.gameObject;
                        ClickedVehicle.GetComponent<Vehicle_Collision_Controller>().SensorSphereCollider.enabled = false;
                        IsDragging = true;
                    }
                    else
                    {
                        ResetSelections();
                    }
                }
            }

            if (IsDragging && Input.GetMouseButton(0)) // While mouse button is held
            {
                VE_Vehicle veVehicle = ClickedVehicle.GetComponent<VE_Vehicle>();
                MoveSelectedVehicle(veVehicle);
            }

            if (Input.GetMouseButtonUp(0)) // When mouse button is released
            {
                if (ClickedVehicle != null)
                {
                    ClickedVehicle.GetComponent<Vehicle_Collision_Controller>().SensorSphereCollider.enabled = true;
                }
                ResetSelections();
            }
        }

        /// <summary>
        /// Updates the position of the selected vehicle based on mouse position
        /// and adjusts the vehicle angles using scroll input.
        /// </summary>
        /// <param name="veVehicle">The vehicle to move and adjust.</param>
        private void MoveSelectedVehicle(VE_Vehicle veVehicle)
        {
            Ray ray = CameraManager.ActiveCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 targetPoint = hit.point;
                targetPoint.y = ClickedVehicle.transform.position.y; // Maintain current Y position

                if (veVehicle != null)
                {
                    var scale = veVehicle.Config.Scale;

                    // Calculate parameters based on targetPoint
                    var x = targetPoint.x / scale;
                    var y = targetPoint.z / scale;

                    var tractorAngle = Mathf.Rad2Deg * veVehicle.KinematicsController.VehicleData.Psi1;
                    var trailerAngle = Mathf.Rad2Deg * veVehicle.KinematicsController.VehicleData.Psi2;

                    // Update vehicleTransform position using SetVehiclePosition
                    veVehicle.KinematicsController.SetVehiclePosition(x * scale, y * scale, tractorAngle, trailerAngle);
                    HandleScrollInput(veVehicle); // Handle scroll input during dragging
                }
            }
        }

        /// <summary>
        /// Processes the scroll wheel input and adjusts the angles of the selected vehicle.
        /// </summary>
        /// <param name="veVehicle">The vehicle to adjust.</param>
        private void HandleScrollInput(VE_Vehicle veVehicle)
        {
            ScrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(ScrollInput) > 0.001f)
            {
                AdjustVehicleAngles(veVehicle);
            }
        }

        /// <summary>
        /// Adjusts the tractor or trailer angles of the selected vehicle
        /// based on the scroll input and sensitivity.
        /// </summary>
        /// <param name="veVehicle">The vehicle to adjust.</param>
        private void AdjustVehicleAngles(VE_Vehicle veVehicle)
        {
            if (veVehicle != null)
            {
                if (ClickedTractorObject != null)
                {
                    veVehicle.KinematicsController.SetTractorAngle(ScrollInput, ScrollSensitivity);
                }
                else if (ClickedTrailerObject != null)
                {
                    veVehicle.KinematicsController.SetTrailerAngle(ScrollInput, ScrollSensitivity);
                }
            }
        }

        /// <summary>
        /// Resets the selected vehicle, trailer, and tractor references, and stops dragging.
        /// </summary>
        private void ResetSelections()
        {
            ClickedVehicle = null;
            ClickedTrailerObject = null;
            ClickedTractorObject = null;
            IsDragging = false;
        }
    }
}
