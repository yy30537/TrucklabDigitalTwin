using RosSharp.RosBridgeClient;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Manages the simulation menu for controlling and visualizing vehicle paths.
    /// </summary>
    public class SimulationMenu : BaseMenu
    {
        [Header("UI Components")]
        [SerializeField] private TMP_Dropdown createdVehicleDropdown;
        [SerializeField] private TMP_Dropdown pathDropdown;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI timeRemainingText;
        [SerializeField] private TextMeshProUGUI startPositionText;
        [SerializeField] private TextMeshProUGUI endPositionText;
        [SerializeField] private TextMeshProUGUI inputVelocityText;
        [SerializeField] private TextMeshProUGUI inputSteeringText;

        [Header("Dependencies")]
        [SerializeField] private PathDrawer pathDrawer;
        [SerializeField] private SetSimulationServiceProvider simulationServiceProvider;
        [SerializeField] private PathManager pathManager;
        [SerializeField] private VehicleFactory vehicleFactory;

        private Path selectedPath;
        private VehicleProduct selectedVehicle;

        private void Start()
        {
            base.Init();
            MenuName = "Simulation";
        }

        /// <summary>
        /// Populates the dropdown with created vehicles.
        /// </summary>
        public void PopulateCreatedVehicleDropdown()
        {
            if (createdVehicleDropdown == null || vehicleFactory == null)
            {
                Debug.LogError("Created vehicle dropdown or vehicle factory is not assigned.");
                return;
            }

            pathDrawer.Toggle(false);
            createdVehicleDropdown.ClearOptions();
            List<string> createdVehicleNames = new List<string>();

            foreach (var vehicle in vehicleFactory.ProductLookupTable.Values)
            {
                createdVehicleNames.Add(vehicle.ProductName);
            }

            createdVehicleDropdown.AddOptions(createdVehicleNames);

            if (createdVehicleNames.Count > 0)
            {
                SetSelectedVehicleFromDropdown();
            }
            else
            {
                selectedVehicle = null;
            }
        }

        /// <summary>
        /// Populates the dropdown with available paths.
        /// </summary>
        public void PopulatePathDropdown()
        {
            if (pathDropdown == null || pathManager == null)
            {
                Debug.LogError("Path dropdown or path manager is not assigned.");
                return;
            }

            pathDropdown.ClearOptions();
            List<string> pathNames = new List<string>();

            foreach (var path in pathManager.paths)
            {
                pathNames.Add(path.pathName);
            }

            pathDropdown.AddOptions(pathNames);
        }

        /// <summary>
        /// Handles the selection of a path.
        /// </summary>
        public void OnSelectPath()
        {
            if (pathDrawer == null || pathDropdown == null || pathManager == null || selectedVehicle == null || simulationServiceProvider == null)
            {
                Debug.LogError("One or more required components are not assigned.");
                return;
            }

            pathDrawer.Toggle(true);
            SetSelectedVehicleFromDropdown();
            var selectedPathIndex = pathDropdown.value;
            selectedPath = pathManager.paths[selectedPathIndex];
            selectedVehicle.Kinematics.InitPathVisualization(selectedPath);

            simulationServiceProvider.SetSimulationDetail(selectedVehicle.Config.VehicleId, selectedPath.pathID);

            UpdateUIWithPathInfo();
        }

        /// <summary>
        /// Handles the selection of a path from the dropdown.
        /// </summary>
        public void OnPathSelectedFromDropdown()
        {
            if (pathDropdown == null || pathManager == null || pathDrawer == null)
            {
                Debug.LogError("Path dropdown, path manager, or path drawer is not assigned.");
                return;
            }

            var selectedIndex = pathDropdown.value;
            selectedPath = pathManager.paths[selectedIndex];
            pathDrawer.DrawPath(selectedPath);
        }

        /// <summary>
        /// Initiates the visualization of the selected path.
        /// </summary>
        public void OnVisualizePath()
        {
            if (selectedVehicle == null || selectedPath == null)
            {
                Debug.LogError("Selected vehicle or path is null.");
                return;
            }

            selectedVehicle.Kinematics.VisualizePath(selectedPath);
            StartCoroutine(UpdateSimulationUI());
        }

        /// <summary>
        /// Sets the selected vehicle based on the dropdown selection.
        /// </summary>
        private void SetSelectedVehicleFromDropdown()
        {
            if (createdVehicleDropdown == null || vehicleFactory == null)
            {
                Debug.LogError("Created vehicle dropdown or vehicle factory is not assigned.");
                return;
            }

            if (createdVehicleDropdown.options.Count > 0)
            {
                var selectedVehicleName = createdVehicleDropdown.options[createdVehicleDropdown.value].text;
                foreach (var vehicle in vehicleFactory.ProductLookupTable.Values)
                {
                    if (vehicle.ProductName == selectedVehicleName)
                    {
                        selectedVehicle = vehicle;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the UI with information about the selected path.
        /// </summary>
        private void UpdateUIWithPathInfo()
        {
            if (statusText == null || startPositionText == null || endPositionText == null || selectedPath == null)
            {
                Debug.LogError("One or more UI components or selected path is null.");
                return;
            }

            statusText.text = $"Path: {selectedPath.pathName}";
            startPositionText.text = $"Start Position: ({selectedPath.frontaxle[0].x:F2}, {selectedPath.frontaxle[0].y:F2})";
            endPositionText.text = $"End Position: ({selectedPath.frontaxle[^1].x:F2}, {selectedPath.frontaxle[^1].y:F2})";
        }

        /// <summary>
        /// Updates the UI during path visualization.
        /// </summary>
        private IEnumerator UpdateSimulationUI()
        {
            if (statusText == null || timeRemainingText == null || selectedPath == null)
            {
                Debug.LogError("One or more UI components or selected path is null.");
                yield break;
            }

            statusText.text = $"Visualizing Path {selectedPath.pathName}";
            float simulationTime = selectedPath.time[^1];
            float elapsedTime = 0f;

            while (elapsedTime < simulationTime)
            {
                elapsedTime += Time.deltaTime;
                timeRemainingText.text = $"Time Remaining: {Mathf.Max(0, simulationTime - elapsedTime):0.00}s";
                yield return null;
            }

            statusText.text = "Completed";
            timeRemainingText.text = "Time Remaining: 0.00s";
        }

        /// <summary>
        /// Starts recording a new path.
        /// </summary>
        public void StartRecording()
        {
            if (selectedVehicle == null || pathManager == null || statusText == null)
            {
                Debug.LogError("Selected vehicle, path manager, or status text is null.");
                return;
            }

            pathManager.StartRecording(selectedVehicle);
            statusText.text = "Recording Path...";
        }

        /// <summary>
        /// Stops recording the current path.
        /// </summary>
        public void StopRecording()
        {
            if (pathManager == null || statusText == null)
            {
                Debug.LogError("Path manager or status text is null.");
                return;
            }

            pathManager.StopRecording();
            statusText.text = "Path Recording Stopped.";
            PopulatePathDropdown();
        }
    }
}
