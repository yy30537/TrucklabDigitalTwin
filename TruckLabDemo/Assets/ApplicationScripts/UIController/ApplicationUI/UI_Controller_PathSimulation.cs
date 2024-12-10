using System.Collections;
using System.Collections.Generic;
using ApplicationScripts.Manager.PathManager;
using ApplicationScripts.VirtualEntity.Vehicle;
using TMPro;
using UnityEngine;

namespace ApplicationScripts.UIController.ApplicationUI
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
        /// Dropdown to select a referencePath for simulation.
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
        /// Text UI element to display the start position of the selected referencePath.
        /// </summary>
        public TextMeshProUGUI StartPositionText;

        /// <summary>
        /// Text UI element to display the end position of the selected referencePath.
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
        [Header("Reference_Path Simulation")]
        public Path_Manager PathManager;

        /// <summary>
        /// Reference to the PathPreviewer for visualizing paths.
        /// </summary>
        public Path_Previewer PathPreviewer;

        /// <summary>
        /// Currently selected referencePath for simulation.
        /// </summary>
        public Reference_Path SelectedReferencePath;

        /// <summary>
        /// Currently selected VeVehicle for referencePath simulation.
        /// </summary>
        public VE_Vehicle SelectedVeVehicle;

        /// <summary>
        /// Initializes the referencePath simulation controller and its dependencies.
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

            PathPreviewer.Toggle(false); // Hide referencePath previewer initially
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
                Debug.LogError("Reference_Path dropdown or referencePath manager is not assigned.");
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
                        Debug.Log($"Selected {vehicle.Name} for path simulation.");
                        break;
                    }
                }
            }

            
        }

        /// <summary>
        /// Updates the currently selected referencePath based on the dropdown selection.
        /// </summary>
        public void OnPathSelectedFromDropdown()
        {
            if (PathDropdown == null || PathManager == null || PathPreviewer == null)
            {
                Debug.LogError("Reference_Path dropdown, referencePath manager, or referencePath drawer is not assigned.");
                return;
            }

            var selectedIndex = PathDropdown.value;
            SelectedReferencePath = PathManager.Paths[selectedIndex];
            PathPreviewer.DrawPath(SelectedReferencePath);
            Debug.Log($"Selected {SelectedReferencePath.PathName} for path simulation.");
            UpdateUiWithPathInfo();
        }

        /// <summary>
        /// Selects and prepares a referencePath for simulation.
        /// </summary>
        public void OnSelectPath()
        {
            if (PathPreviewer == null || PathDropdown == null || PathManager == null || SelectedVeVehicle == null)
            {
                Debug.LogError("One or more required components are not assigned.");
                return;
            }

            PathPreviewer.Toggle(true); // Show referencePath preview
            OnVehicleSelectedFromDropdown();
            var selectedPathIndex = PathDropdown.value;
            SelectedReferencePath = PathManager.Paths[selectedPathIndex];
            SelectedVeVehicle.KinematicsController.InitPathReplay(SelectedReferencePath);

            UpdateUiWithPathInfo();
        }


        /*
         * Path Replay
         */
        /// <summary>
        /// Starts referencePath replaying for the selected vehicle and referencePath.
        /// </summary>
        public void OnStartPathReplaying()
        {
            if (SelectedVeVehicle == null || SelectedReferencePath == null)
            {
                Debug.LogError("Selected VeVehicle or referencePath is null.");
                return;
            }

            SelectedVeVehicle.KinematicsController.StartPathReplaying(SelectedReferencePath);
            StartCoroutine(UpdateSimulationUi());
        }

        /// <summary>
        /// Stops referencePath replaying for the selected vehicle.
        /// </summary>
        public void OnStopPathReplaying()
        {
            if (SelectedVeVehicle == null || SelectedReferencePath == null)
            {
                Debug.LogError("Selected VeVehicle or referencePath is null.");
                return;
            }

            SelectedVeVehicle.KinematicsController.StopPathReplaying();
            StopCoroutine(UpdateSimulationUi());
        }

        /*
         * Path Record
         */
        /// <summary>
        /// Starts recording a new referencePath for the selected vehicle.
        /// </summary>
        public void OnStartPathRecording()
        {
            if (SelectedVeVehicle == null || PathManager == null || StatusText == null)
            {
                Debug.LogError("Selected VeVehicle, referencePath manager, or status text is null.");
                return;
            }

            PathManager.StartPathRecording(SelectedVeVehicle);
            StatusText.text = "Recording Reference_Path...";
        }

        /// <summary>
        /// Stops the current referencePath recording and updates the referencePath list.
        /// </summary>
        public void OnStopPathRecording()
        {
            if (PathManager == null || StatusText == null)
            {
                Debug.LogError("Reference_Path manager or status text is null.");
                return;
            }

            PathManager.StopPathRecording();
            StatusText.text = "Reference_Path Recording Stopped.";
            PopulatePathDropdown();
        }

        /*
         * Path Visualization
         */
        /// <summary>
        /// Starts referencePath replaying for the selected vehicle and referencePath.
        /// </summary>
        public void OnStartPathVisualization()
        {
            if (SelectedVeVehicle == null || SelectedReferencePath == null)
            {
                Debug.LogError("Selected VeVehicle or referencePath is null.");
                return;
            }

            SelectedVeVehicle.KinematicsController.StartPathVisualization(SelectedReferencePath);
            StartCoroutine(UpdateSimulationUi());
        }

        /// <summary>
        /// Stops referencePath replaying for the selected vehicle.
        /// </summary>
        public void OnStopPathVisualization()
        {
            if (SelectedVeVehicle == null || SelectedReferencePath == null)
            {
                Debug.LogError("Selected VeVehicle or referencePath is null.");
                return;
            }

            SelectedVeVehicle.KinematicsController.StopPathVisualization(SelectedReferencePath);
            StopCoroutine(UpdateSimulationUi());
        }

        /// <summary>
        /// Updates the UI with information about the selected referencePath.
        /// </summary>
        private void UpdateUiWithPathInfo()
        {
            if (StatusText == null || StartPositionText == null || EndPositionText == null || SelectedReferencePath == null)
            {
                Debug.LogError("One or more UI components or selected referencePath is null.");
                return;
            }

            StatusText.text = $"Reference_Path: {SelectedReferencePath.PathName}";
            StartPositionText.text = $"Start Position: ({SelectedReferencePath.FrontAxle[0].x:F2}, {SelectedReferencePath.FrontAxle[0].y:F2})";
            EndPositionText.text = $"End Position: ({SelectedReferencePath.FrontAxle[^1].x:F2}, {SelectedReferencePath.FrontAxle[^1].y:F2})";
        }

        /// <summary>
        /// Updates the simulation UI dynamically during referencePath replay.
        /// </summary>
        private IEnumerator UpdateSimulationUi()
        {
            if (StatusText == null || TimeRemainingText == null || SelectedReferencePath == null)
            {
                Debug.LogError("One or more UI components or selected referencePath is null.");
                yield break;
            }

            StatusText.text = $"Visualizing Reference_Path {SelectedReferencePath.PathName}";
            float simulationTime = SelectedReferencePath.Time[^1];
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
