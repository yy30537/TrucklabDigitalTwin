using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Manages the cameras in the scene, allowing switching between different views and controlling the main camera.
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        public Camera MainCamera;
        public TMP_Dropdown CameraDropdown;
        private Dictionary<string, Camera> allCameras = new Dictionary<string, Camera>();
        private VehicleFactory vehicleFactory;

        public VoidEventChannel ProductInitializedEvent;
        public VoidEventChannel ProductDeletedEvent;

        public SystemLogWindow SystemLogWindow;

        [Header("Preconfigured Cameras")]
        public Camera dcOverheadView;
        public Camera dcSideView;
        public Camera dcDockView;
        public Camera dcOverview;

        [Header("Camera Control Properties")]
        public bool isControlActive = true;
        public float movementSpeed = 50f;
        public float cameraVerticalSpeed = 10f;
        public float cameraDragSpeed = 100f;
        public float mouseSensitivity = 20f;
        public float zoomSensitivity = 20f;
        private Vector3 lastMousePosition;
        private float cameraHeight = 0f;

        /// <summary>
        /// Initializes the CameraManager.
        /// </summary>
        private void Start()
        {
            vehicleFactory = FindObjectOfType<VehicleFactory>();
            if (vehicleFactory == null)
            {
                Debug.LogError("VehicleFactory not found in the scene.");
                return;
            }

            InitializeDropdown();

            // Subscribe to event channels
            ProductInitializedEvent.onEventRaised += OnProductInitialized;
            ProductDeletedEvent.onEventRaised += OnProductDeleted;

            MainCamera.enabled = true;
        }

        /// <summary>
        /// Unsubscribes from event channels on destruction.
        /// </summary>
        private void OnDestroy()
        {
            ProductInitializedEvent.onEventRaised -= OnProductInitialized;
            ProductDeletedEvent.onEventRaised -= OnProductDeleted;
        }

        /// <summary>
        /// Handles new product initialization.
        /// </summary>
        private void OnProductInitialized()
        {
            // Find the last created vehicle and add its cameras
            if (vehicleFactory.ProductLookupTable.Count > 0)
            {
                var lastVehicleId = vehicleFactory.ProductLookupTable.Keys.Max();
                var lastVehicle = vehicleFactory.ProductLookupTable[lastVehicleId];
                AddVehicleCameras(lastVehicle);
            }
        }

        /// <summary>
        /// Handles product deletion.
        /// </summary>
        private void OnProductDeleted()
        {
            // Remove cameras of deleted vehicles
            List<string> camerasToRemove = new List<string>();
            foreach (var cameraEntry in allCameras)
            {
                if (!cameraEntry.Key.Contains("Main Camera"))
                {
                    string vehicleName = cameraEntry.Key.Split('-')[0].Trim();
                    bool vehicleExists = vehicleFactory.ProductLookupTable.Values.Any(v => v.ProductName == vehicleName);
                    if (!vehicleExists)
                    {
                        camerasToRemove.Add(cameraEntry.Key);
                    }
                }
            }

            foreach (string cameraName in camerasToRemove)
            {
                allCameras.Remove(cameraName);
            }

            UpdateDropdownOptions();

            // If the active camera was removed, switch to Main Camera
            if (camerasToRemove.Contains(CameraDropdown.options[CameraDropdown.value].text))
            {
                ActivateCamera(MainCamera);
            }
        }

        /// <summary>
        /// Handles camera control updates.
        /// </summary>
        private void Update()
        {
            if (isControlActive && MainCamera.enabled)
            {
                HandleCameraControl();
            }
        }

        /// <summary>
        /// Initializes the camera dropdown with available cameras.
        /// </summary>
        private void InitializeDropdown()
        {
            CameraDropdown.ClearOptions();
            CameraDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            allCameras.Clear();
            allCameras["Main Camera"] = MainCamera;

            // Add preconfigured cameras
            if (dcOverheadView != null) allCameras["DC Overhead View"] = dcOverheadView;
            if (dcSideView != null) allCameras["DC Side View"] = dcSideView;
            if (dcDockView != null) allCameras["DC Dock View"] = dcDockView;
            if (dcOverview != null) allCameras["DC Overview"] = dcOverview;

            UpdateDropdownOptions();

            // Ensure Main Camera is active
            ActivateCamera(MainCamera);
        }

        /// <summary>
        /// Adds cameras from a vehicle to the manager.
        /// </summary>
        /// <param name="vehicle">The vehicle whose cameras are to be added.</param>
        private void AddVehicleCameras(VehicleProduct vehicle)
        {
            foreach (Camera cam in vehicle.VehicleCameras)
            {
                string cameraName = $"{vehicle.ProductName} - {cam.name}";
                if (!allCameras.ContainsKey(cameraName))
                {
                    allCameras[cameraName] = cam;
                    cam.enabled = false; // Ensure the camera is disabled when added
                }
            }
            UpdateDropdownOptions();
        }

        /// <summary>
        /// Updates the dropdown options with the current list of cameras.
        /// </summary>
        private void UpdateDropdownOptions()
        {
            int currentIndex = CameraDropdown.value;
            string currentSelection = CameraDropdown.options.Count > 0 ? CameraDropdown.options[currentIndex].text : "Main Camera";

            CameraDropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (string cameraName in allCameras.Keys)
            {
                options.Add(new TMP_Dropdown.OptionData(cameraName));
            }
            CameraDropdown.AddOptions(options);

            // Restore the previous selection if it still exists
            int newIndex = options.FindIndex(option => option.text == currentSelection);
            if (newIndex >= 0)
            {
                CameraDropdown.value = newIndex;
            }
            else
            {
                // If the previous selection doesn't exist, default to Main Camera
                CameraDropdown.value = options.FindIndex(option => option.text == "Main Camera");
            }
        }

        /// <summary>
        /// Handles dropdown value changes to switch active camera.
        /// </summary>
        /// <param name="index">The index of the selected camera in the dropdown.</param>
        private void OnDropdownValueChanged(int index)
        {
            string selectedCameraName = CameraDropdown.options[index].text;
            if (allCameras.TryGetValue(selectedCameraName, out Camera selectedCamera))
            {
                ActivateCamera(selectedCamera);
            }
        }

        /// <summary>
        /// Activates the specified camera and deactivates others.
        /// </summary>
        /// <param name="cameraToActivate">The camera to activate.</param>
        private void ActivateCamera(Camera cameraToActivate)
        {
            foreach (Camera cam in allCameras.Values)
            {
                cam.enabled = (cam == cameraToActivate);
            }

            // If activating a vehicle camera or preconfigured camera, disable the main camera
            if (cameraToActivate != MainCamera)
            {
                MainCamera.enabled = false;
            }

            // Update dropdown to reflect the active camera
            int index = CameraDropdown.options.FindIndex(option => option.text == cameraToActivate.name);
            if (index >= 0)
            {
                CameraDropdown.SetValueWithoutNotify(index);
            }
        }

        /// <summary>
        /// Handles the main camera control for movement, rotation, and zoom.
        /// </summary>
        private void HandleCameraControl()
        {
            // Only apply camera control to the main camera
            if (MainCamera.enabled)
            {
                // Mouse rotation
                if (Input.GetMouseButtonDown(1)) // Right mouse button
                {
                    lastMousePosition = Input.mousePosition;
                }

                if (Input.GetMouseButton(1)) // Right mouse button held down
                {
                    Vector3 delta = Input.mousePosition - lastMousePosition;
                    float rotationX = -delta.y * mouseSensitivity * Time.deltaTime;
                    float rotationY = delta.x * mouseSensitivity * Time.deltaTime;
                    MainCamera.transform.eulerAngles += new Vector3(rotationX, rotationY, 0);
                    lastMousePosition = Input.mousePosition;
                }

                // Camera movement using middle mouse button
                if (Input.GetMouseButton(2)) // Middle mouse button held down
                {
                    float middleMouseX = Input.GetAxis("Mouse X") * cameraDragSpeed * Time.deltaTime * (-1);
                    float middleMouseY = Input.GetAxis("Mouse Y") * cameraDragSpeed * Time.deltaTime * (-1);
                    MainCamera.transform.Translate(middleMouseX, 0, middleMouseY);
                }

                // WASD movement
                float x = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
                float z = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
                MainCamera.transform.Translate(x, 0, z);

                // Raise camera using spacebar
                if (Input.GetKey(KeyCode.Space))
                {
                    cameraHeight += cameraVerticalSpeed * Time.deltaTime;
                    MainCamera.transform.position += new Vector3(0, cameraHeight, 0);
                }

                // Camera zoom
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                MainCamera.fieldOfView -= scroll * zoomSensitivity;
            }
        }

        /// <summary>
        /// Toggles the camera control on or off.
        /// </summary>
        public void ToggleControl()
        {
            isControlActive = !isControlActive;
            SystemLogWindow.LogEvent($"Camera Free Look: {isControlActive}");
        }
    }
}
