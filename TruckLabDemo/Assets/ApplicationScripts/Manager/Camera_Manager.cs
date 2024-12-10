using System.Collections.Generic;
using System.Linq;
using ApplicationScripts.EventChannel;
using ApplicationScripts.UIController.ApplicationUI;
using ApplicationScripts.VirtualEntity.Vehicle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ApplicationScripts.Manager
{
    /// <summary>
    /// Manages the cameras in the scene, allowing users to switch between views and control the main camera.
    /// </summary>
    public class Camera_Manager : MonoBehaviour
    {
        /// <summary>
        /// The main camera used for free-look and primary operations.
        /// </summary>
        [Header("Target Cameras")]
        public Camera MainCamera;

        /// <summary>
        /// Overhead camera for viewing the environment from above.
        /// </summary>
        public Camera EnvOverheadCamera;

        /// <summary>
        /// Side camera for viewing the environment from a lateral perspective.
        /// </summary>
        public Camera EnvSideCamera;

        /// <summary>
        /// Camera positioned for viewing dock areas.
        /// </summary>
        public Camera EnvDockCamera;

        /// <summary>
        /// Overview camera for displaying the entire environment.
        /// </summary>
        public Camera EnvOverviewCamera;

        /// <summary>
        /// A dictionary containing all available cameras mapped to their names.
        /// </summary>
        private Dictionary<string, Camera> allCameras = new Dictionary<string, Camera>();

        /// <summary>
        /// The currently active camera.
        /// </summary>
        public Camera ActiveCamera;

        /// <summary>
        /// Dropdown UI element for selecting the active camera.
        /// </summary>
        [Header("UI Objects")]
        public TMP_Dropdown CameraDropdown;

        /// <summary>
        /// Toggle UI element for enabling or disabling free-look mode.
        /// </summary>
        public Toggle MenuToggle;

        /// <summary>
        /// Event triggered when a vehicle is registered, allowing its cameras to be added.
        /// </summary>
        [Header("Event Channels")]
        public EventChannel_Void VeRegistrationEvent;

        /// <summary>
        /// Event triggered when a vehicle is deleted, allowing its cameras to be removed.
        /// </summary>
        public EventChannel_Void VeDeletionEvent;

        /// <summary>
        /// Indicates whether free-look mode is active.
        /// </summary>
        [Header("Camera Control Properties")]
        public bool IsControlActive = false;

        /// <summary>
        /// Speed of camera movement during free-look.
        /// </summary>
        public float MovementSpeed = 10f;

        /// <summary>
        /// Speed of vertical camera movement.
        /// </summary>
        public float CameraVerticalSpeed = 5f;

        /// <summary>
        /// Speed of panning the camera with the middle mouse button.
        /// </summary>
        public float CameraDragSpeed = 50f;

        /// <summary>
        /// Sensitivity of camera rotation using the mouse.
        /// </summary>
        public float MouseSensitivity = 0.1f;

        /// <summary>
        /// Sensitivity of camera zoom with the scroll wheel.
        /// </summary>
        public float ZoomSensitivity = 10f;

        private Vector3 lastMousePosition;
        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private float targetFieldOfView;

        /// <summary>
        /// Reference to the vehicle creator for accessing and managing vehicle-related cameras.
        /// </summary>
        [Header("Application Dependencies")]
        public Vehicle_Creator VehicleCreator;

        /// <summary>
        /// Reference to the system log controller for logging camera-related events.
        /// </summary>
        public UI_Controller_SystemLog UiControllerSystemLog;

        private void Start()
        {
            // Ensure the VehicleCreator dependency is assigned
            if (VehicleCreator == null)
            {
                Debug.LogError("Vehicle_Creator not found in the scene.");
                return;
            }

            InitializeDropdown();

            // Set initial camera state
            targetPosition = MainCamera.transform.position;
            targetRotation = MainCamera.transform.rotation;
            targetFieldOfView = MainCamera.fieldOfView;

            // Subscribe to events for vehicle registration and deletion
            VeRegistrationEvent.OnEventRaised += On_VehicleVE_Registered;
            VeDeletionEvent.OnEventRaised += On_Vehicle_VE_Deleted;

            // Enable the main camera by default
            MainCamera.enabled = true;
            ActiveCamera = MainCamera;
        }

        private void Update()
        {
            // Process camera controls if free-look mode is active
            if (IsControlActive && MainCamera.enabled)
            {
                HandleCameraControl();
            }

            // Smoothly interpolate camera movement, rotation, and zoom
            MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, targetPosition, 0.1f);
            MainCamera.transform.rotation = Quaternion.Slerp(MainCamera.transform.rotation, targetRotation, 0.1f);
            MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, targetFieldOfView, 0.1f);
        }

        /// <summary>
        /// Handles camera movement, rotation, and zoom based on user input.
        /// </summary>
        private void HandleCameraControl()
        {
            if (ActiveCamera == MainCamera)
            {
                // Mouse rotation using right-click
                if (Input.GetMouseButtonDown(1))
                {
                    lastMousePosition = Input.mousePosition;
                }

                if (Input.GetMouseButton(1))
                {
                    Vector3 delta = Input.mousePosition - lastMousePosition;
                    float rotationX = -delta.y * MouseSensitivity;
                    float rotationY = delta.x * MouseSensitivity;
                    targetRotation = Quaternion.Euler(
                        targetRotation.eulerAngles.x + rotationX,
                        targetRotation.eulerAngles.y + rotationY,
                        targetRotation.eulerAngles.z
                    );
                    lastMousePosition = Input.mousePosition;
                }

                // Movement using WASD keys
                float x = Input.GetAxis("Horizontal") * MovementSpeed * Time.deltaTime;
                float z = Input.GetAxis("Vertical") * MovementSpeed * Time.deltaTime;
                targetPosition += MainCamera.transform.right * x + MainCamera.transform.forward * z;

                // Vertical movement using space and shift keys
                if (Input.GetKey(KeyCode.Space))
                {
                    targetPosition += Vector3.up * CameraVerticalSpeed * Time.deltaTime;
                }
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    targetPosition -= Vector3.up * CameraVerticalSpeed * Time.deltaTime;
                }

                // Zoom using the scroll wheel
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                targetFieldOfView = Mathf.Clamp(targetFieldOfView - scroll * ZoomSensitivity, 15f, 90f);

                // Panning using the middle mouse button
                if (Input.GetMouseButton(2))
                {
                    float middleMouseX = Input.GetAxis("Mouse X") * CameraDragSpeed * Time.deltaTime * (-1);
                    float middleMouseY = Input.GetAxis("Mouse Y") * CameraDragSpeed * Time.deltaTime * (-1);
                    targetPosition += MainCamera.transform.right * middleMouseX + MainCamera.transform.up * middleMouseY;
                }
            }
        }

        /// <summary>
        /// Initializes the camera dropdown with all available cameras.
        /// </summary>
        private void InitializeDropdown()
        {
            CameraDropdown.ClearOptions();
            CameraDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            // Populate camera dictionary
            allCameras.Clear();
            allCameras["Main Camera"] = MainCamera;

            if (EnvOverheadCamera != null) allCameras["DC Overhead View"] = EnvOverheadCamera;
            if (EnvSideCamera != null) allCameras["DC Side View"] = EnvSideCamera;
            if (EnvDockCamera != null) allCameras["DC Dock View"] = EnvDockCamera;
            if (EnvOverviewCamera != null) allCameras["DC Overview"] = EnvOverviewCamera;

            UpdateDropdownOptions();
            ActivateCamera(MainCamera);
        }

        /// <summary>
        /// Adds cameras associated with a given vehicle to the camera list and updates the dropdown options.
        /// </summary>
        /// <param name="veVehicle">The vehicle whose cameras are being added.</param>
        private void AddVehicleCameras(VE_Vehicle veVehicle)
        {
            foreach (Camera cam in veVehicle.VehicleCameras)
            {
                // Construct a unique camera name using the vehicle's name and the camera's name
                string cameraName = $"{veVehicle.Name} - {cam.name}";

                // Add the camera to the dictionary if it doesn't already exist
                if (!allCameras.ContainsKey(cameraName))
                {
                    allCameras[cameraName] = cam;
                    cam.enabled = false; // Ensure the camera is disabled initially
                }
            }

            // Refresh the camera dropdown to include the newly added cameras
            UpdateDropdownOptions();
        }

        /// <summary>
        /// Updates the camera dropdown options based on the current camera dictionary.
        /// Maintains the user's current selection if possible.
        /// </summary>
        private void UpdateDropdownOptions()
        {
            // Save the current selection
            int currentIndex = CameraDropdown.value;
            string currentSelection = CameraDropdown.options.Count > 0 ? CameraDropdown.options[currentIndex].text : "Main Camera";

            // Clear the existing options in the dropdown
            CameraDropdown.ClearOptions();

            // Populate the dropdown with camera names from the dictionary
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (string cameraName in allCameras.Keys)
            {
                options.Add(new TMP_Dropdown.OptionData(cameraName));
            }
            CameraDropdown.AddOptions(options);

            // Attempt to restore the user's previous selection
            int newIndex = options.FindIndex(option => option.text == currentSelection);
            if (newIndex >= 0)
            {
                CameraDropdown.value = newIndex;
            }
            else
            {
                // Default to the "Main Camera" if the previous selection is no longer valid
                CameraDropdown.value = options.FindIndex(option => option.text == "Main Camera");
            }
        }

        /// <summary>
        /// Handles the dropdown value change event to activate the selected camera.
        /// </summary>
        /// <param name="index">The index of the selected dropdown option.</param>
        public void OnDropdownValueChanged(int index)
        {
            // Retrieve the selected camera's name and activate it if it exists in the dictionary
            string selectedCameraName = CameraDropdown.options[index].text;
            if (allCameras.TryGetValue(selectedCameraName, out Camera selectedCamera))
            {
                ActivateCamera(selectedCamera);
                ActiveCamera = selectedCamera;
            }
        }

        /// <summary>
        /// Activates the specified camera and disables all others.
        /// </summary>
        /// <param name="cameraToActivate">The camera to activate.</param>
        public void ActivateCamera(Camera cameraToActivate)
        {
            foreach (Camera cam in allCameras.Values)
            {
                // Enable only the specified camera
                cam.enabled = (cam == cameraToActivate);
            }

            // Ensure the main camera is disabled if another camera is activated
            if (cameraToActivate != MainCamera)
            {
                MainCamera.enabled = false;
            }

            // Update the dropdown to reflect the active camera
            int index = CameraDropdown.options.FindIndex(option => option.text == cameraToActivate.name);
            if (index >= 0)
            {
                CameraDropdown.SetValueWithoutNotify(index);
            }
        }

        /// <summary>
        /// Handles the event when a new vehicle is registered by adding its cameras to the list.
        /// </summary>
        private void On_VehicleVE_Registered()
        {
            // Check if there are vehicles in the creator's lookup table
            if (VehicleCreator.LookupTable.Count > 0)
            {
                // Get the last registered vehicle and add its cameras
                var lastVehicleId = VehicleCreator.LookupTable.Keys.Max();
                var lastVehicle = VehicleCreator.LookupTable[lastVehicleId];
                AddVehicleCameras(lastVehicle);
            }
        }

        /// <summary>
        /// Handles the event when a vehicle is deleted by removing its cameras from the list.
        /// </summary>
        private void On_Vehicle_VE_Deleted()
        {
            // Identify cameras associated with deleted vehicles
            List<string> camerasToRemove = new List<string>();
            foreach (var cameraEntry in allCameras)
            {
                if (cameraEntry.Key.Contains("Truck"))
                {
                    // Extract the vehicle name from the camera name and check if the vehicle still exists
                    string vehicleName = cameraEntry.Key.Split('-')[0].Trim();
                    bool vehicleExists = VehicleCreator.LookupTable.Values.Any(v => v.Name == vehicleName);
                    if (!vehicleExists)
                    {
                        camerasToRemove.Add(cameraEntry.Key);
                    }
                }
            }

            // Remove the identified cameras from the dictionary
            foreach (string cameraName in camerasToRemove)
            {
                allCameras.Remove(cameraName);
            }

            // Update the dropdown to reflect the changes
            UpdateDropdownOptions();

            // Ensure the main camera is activated if the currently selected camera was removed
            if (camerasToRemove.Contains(CameraDropdown.options[CameraDropdown.value].text))
            {
                ActivateCamera(MainCamera);
            }
        }

        /// <summary>
        /// Toggles the free-look mode for the main camera and logs the event.
        /// </summary>
        public void OnMenuToggle()
        {
            // Update the free-look control state based on the toggle's value
            IsControlActive = MenuToggle.isOn;

            // Log the change in free-look mode
            UiControllerSystemLog.LogEvent($"Camera Free Look: {IsControlActive}");
        }

    }
}
