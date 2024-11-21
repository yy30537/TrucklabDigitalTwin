using System.Collections;
using System.Collections.Generic;
using Application_Scripts.Manager.Path_Manager;
using Application_Scripts.Virtual_Entity.Vehicle;
using TMPro;
using UnityEngine;

namespace Application_Scripts.UI_Controller.Application_UI
{
    /// <summary>
    /// Manages the simulation menu for controlling and visualizing VeVehicle paths.
    /// Provides functionality to select, preview, and replay paths for VeVehicles.
    /// </summary>
    public class UI_Controller_PathSimulation : Base_UI_Controller
    {
        /// <summary>
        /// Reference to the VehicleCreator for accessing created vehicles.
        /// </summary>
        [Header("===Children: UI_Controller_PathSimulation===")]
        public Vehicle_Creator VehicleCreator;

        /// <summary>
        /// Dropdown to select a created vehicle from the list.
        /// </summary>
        [Header("UI Objects")]
        public TMP_Dropdown CreatedVehicleDropdown;

        /// <summary>
        /// Dropdown to select a path for simulation.
        /// </summary>
        public TMP_Dropdown PathDropdown;

        /// <summary>
        /// Text UI element to display the simulation status.
        /// </summary>
        public TextMeshProUGUI StatusText;

        /// <summary>
        /// Text UI element to display the remaining Time for the simulation.
        /// </summary>
        public TextMeshProUGUI TimeRemainingText;

        /// <summary>
        /// Text UI element to display the start position of the selected path.
        /// </summary>
        public TextMeshProUGUI StartPositionText;

        /// <summary>
        /// Text UI element to display the end position of the selected path.
        /// </summary>
        public TextMeshProUGUI EndPositionText;

        /// <summary>
        /// Text UI element to display the input velocity.
        /// </summary>
        public TextMeshProUGUI InputVelocityText;

        /// <summary>
        /// Text UI element to display the input steering angle.
        /// </summary>
        public TextMeshProUGUI InputSteeringText;

        /// <summary>
        /// Reference to the PathManager for managing available paths.
        /// </summary>
        [Header("Path Simulation")]
        public Path_Manager PathManager;

        /// <summary>
        /// Reference to the PathPreviewer for visualizing paths.
        /// </summary>
        public Path_Previewer PathPreviewer;

        /// <summary>
        /// Currently selected path for simulation.
        /// </summary>
        public Path SelectedPath;

        /// <summary>
        /// Currently selected VeVehicle for path simulation.
        /// </summary>
        public VE_Vehicle SelectedVeVehicle;

        /// <summary>
        /// Initializes the path simulation controller and its dependencies.
        /// </summary>
        private void Start()
        {
            base.Init();
        }

        /// <summary>
        /// Populates the dropdown with the list of created vehicles.
        /// </summary>
        public void PopulateCreatedVehicleDropdown()
        {
            if (CreatedVehicleDropdown == null || VehicleCreator == null)
            {
                Debug.LogError("Created VeVehicle dropdown or VeVehicle factory is not assigned.");
                return;
            }

            PathPreviewer.Toggle(false); // Hide path previewer initially
            CreatedVehicleDropdown.ClearOptions();
            List<string> createdVehicleNames = new List<string>();

            foreach (var vehicle in VehicleCreator.LookupTable.Values)
            {
                createdVehicleNames.Add(vehicle.Name);
            }

            CreatedVehicleDropdown.AddOptions(createdVehicleNames);

            if (createdVehicleNames.Count > 0)
            {
                OnVehicleSelectedFromDropdown();
            }
            else
            {
                SelectedVeVehicle = null;
            }
        }

        /// <summary>
        /// Populates the dropdown with the list of available paths.
        /// </summary>
        public void PopulatePathDropdown()
        {
            if (PathDropdown == null || PathManager == null)
            {
                Debug.LogError("Path dropdown or path manager is not assigned.");
                return;
            }

            PathDropdown.ClearOptions();
            List<string> pathNames = new List<string>();

            foreach (var path in PathManager.Paths)
            {
                pathNames.Add(path.PathName);
            }

            PathDropdown.AddOptions(pathNames);
        }

        /// <summary>
        /// Updates the currently selected vehicle based on the dropdown selection.
        /// </summary>
        public void OnVehicleSelectedFromDropdown()
        {
            if (CreatedVehicleDropdown == null || VehicleCreator == null)
            {
                Debug.LogError("Created VeVehicle dropdown or VeVehicle factory is not assigned.");
                return;
            }

            if (CreatedVehicleDropdown.options.Count > 0)
            {
                var selectedVehicleName = CreatedVehicleDropdown.options[CreatedVehicleDropdown.value].text;
                foreach (var vehicle in VehicleCreator.LookupTable.Values)
                {
                    if (vehicle.Name == selectedVehicleName)
                    {
                        SelectedVeVehicle = vehicle;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the currently selected path based on the dropdown selection.
        /// </summary>
        public void OnPathSelectedFromDropdown()
        {
            if (PathDropdown == null || PathManager == null || PathPreviewer == null)
            {
                Debug.LogError("Path dropdown, path manager, or path drawer is not assigned.");
                return;
            }

            var selectedIndex = PathDropdown.value;
            SelectedPath = PathManager.Paths[selectedIndex];
            PathPreviewer.DrawPath(SelectedPath);
        }

        /// <summary>
        /// Selects and prepares a path for simulation.
        /// </summary>
        public void OnSelectPath()
        {
            if (PathPreviewer == null || PathDropdown == null || PathManager == null || SelectedVeVehicle == null)
            {
                Debug.LogError("One or more required components are not assigned.");
                return;
            }

            PathPreviewer.Toggle(true); // Show path preview
            OnVehicleSelectedFromDropdown();
            var selectedPathIndex = PathDropdown.value;
            SelectedPath = PathManager.Paths[selectedPathIndex];
            SelectedVeVehicle.KinematicsController.InitPathReplay(SelectedPath);

            UpdateUiWithPathInfo();
        }

        /// <summary>
        /// Starts path replaying for the selected vehicle and path.
        /// </summary>
        public void OnStartPathReplaying()
        {
            if (SelectedVeVehicle == null || SelectedPath == null)
            {
                Debug.LogError("Selected VeVehicle or path is null.");
                return;
            }

            SelectedVeVehicle.KinematicsController.StartPathReplaying(SelectedPath);
            StartCoroutine(UpdateSimulationUi());
        }

        /// <summary>
        /// Stops path replaying for the selected vehicle.
        /// </summary>
        public void OnStopPathReplaying()
        {
            if (SelectedVeVehicle == null || SelectedPath == null)
            {
                Debug.LogError("Selected VeVehicle or path is null.");
                return;
            }

            SelectedVeVehicle.KinematicsController.StopPathReplaying(SelectedPath);
            StopCoroutine(UpdateSimulationUi());
        }

        /// <summary>
        /// Starts recording a new path for the selected vehicle.
        /// </summary>
        public void OnStartPathRecording()
        {
            if (SelectedVeVehicle == null || PathManager == null || StatusText == null)
            {
                Debug.LogError("Selected VeVehicle, path manager, or status text is null.");
                return;
            }

            PathManager.StartPathRecording(SelectedVeVehicle);
            StatusText.text = "Recording Path...";
        }

        /// <summary>
        /// Stops the current path recording and updates the path list.
        /// </summary>
        public void OnStopPathRecording()
        {
            if (PathManager == null || StatusText == null)
            {
                Debug.LogError("Path manager or status text is null.");
                return;
            }

            PathManager.StopPathRecording();
            StatusText.text = "Path Recording Stopped.";
            PopulatePathDropdown();
        }

        /// <summary>
        /// Updates the UI with information about the selected path.
        /// </summary>
        private void UpdateUiWithPathInfo()
        {
            if (StatusText == null || StartPositionText == null || EndPositionText == null || SelectedPath == null)
            {
                Debug.LogError("One or more UI components or selected path is null.");
                return;
            }

            StatusText.text = $"Path: {SelectedPath.PathName}";
            StartPositionText.text = $"Start Position: ({SelectedPath.FrontAxle[0].x:F2}, {SelectedPath.FrontAxle[0].y:F2})";
            EndPositionText.text = $"End Position: ({SelectedPath.FrontAxle[^1].x:F2}, {SelectedPath.FrontAxle[^1].y:F2})";
        }

        /// <summary>
        /// Updates the simulation UI dynamically during path replay.
        /// </summary>
        private IEnumerator UpdateSimulationUi()
        {
            if (StatusText == null || TimeRemainingText == null || SelectedPath == null)
            {
                Debug.LogError("One or more UI components or selected path is null.");
                yield break;
            }

            StatusText.text = $"Visualizing Path {SelectedPath.PathName}";
            float simulationTime = SelectedPath.Time[^1];
            float elapsedTime = 0f;

            while (elapsedTime < simulationTime)
            {
                elapsedTime += Time.deltaTime;
                TimeRemainingText.text = $"Time Remaining: {Mathf.Max(0, simulationTime - elapsedTime):0.00}s";
                yield return null;
            }

            StatusText.text = "Completed";
            TimeRemainingText.text = "Time Remaining: 0.00s";
        }
    }
}
