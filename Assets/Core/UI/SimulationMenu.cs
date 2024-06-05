using System.Collections;
using System.Collections.Generic;
using RosSharp.RosBridgeClient;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class SimulationMenu : Menu
    {
        public ApplicationManager applicationManager;
        public TMP_Dropdown createdVehicleDropdown;
        public TMP_Dropdown pathDropdown;
        public PathDrawer pathDrawer;

        // UI elements
        public TextMeshProUGUI statusText;
        public TextMeshProUGUI timeRemainingText;
        public TextMeshProUGUI startPositionText;
        public TextMeshProUGUI endPositionText;
        public TextMeshProUGUI inputVelocityText;
        public TextMeshProUGUI inputSteeringText;

        public Path selectedPath;
        public VehicleProduct selectedVehicle;

        public SetSimulationServiceProvider simulationServiceProvider;
        private PathManager pathManager;

        void Start()
        {
            base.Init();
            simulationServiceProvider = FindObjectOfType<SetSimulationServiceProvider>();
            pathManager = FindObjectOfType<PathManager>();
        }

        public void PopulateCreatedVehicleDropdown()
        {
            pathDrawer.Toggle(false);
            createdVehicleDropdown.ClearOptions();
            List<string> createdVehicleNames = new List<string>();
            if (applicationManager.vehicleFactory.productLookupTable.Values.Count > 0)
            {
                foreach (var vehicle in applicationManager.vehicleFactory.productLookupTable.Values)
                {
                    createdVehicleNames.Add(vehicle.productName);
                }
                createdVehicleDropdown.AddOptions(createdVehicleNames);
            }
            SetSelectedVehicleFromDropdown();
        }

        public void PopulatePathDropdown()
        {
            pathDropdown.ClearOptions();
            List<string> pathNames = new List<string>();

            if (applicationManager.vehicleFactory.productLookupTable.Values.Count > 0)
            {
                foreach (var path in applicationManager.pathManager.paths)
                {
                    pathNames.Add(path.pathName);
                }
                pathDropdown.AddOptions(pathNames);
            }
        }

        public void OnSelectPath()
        {
            pathDrawer.Toggle(true);
            selectedVehicle.vehicleAnimation.trail.SetActive(false);
            SetSelectedVehicleFromDropdown();
            var selectedPathIndex = pathDropdown.value;
            selectedPath = applicationManager.pathManager.paths[selectedPathIndex];
            selectedVehicle.vehicleKinematics.InitPathSimulation(selectedPath);

            // Set simulation details via ROS service
            simulationServiceProvider.SetSimulationDetail(selectedVehicle.vehicleConfig.vehicleID, selectedPath.pathID);

            // Update UI with path information
            UpdateUIWithPathInfo();
        }

        public void OnPathSelectedFromDropdown()
        {
            var selectedIndex = pathDropdown.value;
            selectedPath = applicationManager.pathManager.paths[selectedIndex];
            // Visualize the selected path
            pathDrawer.DrawPath(selectedPath);
        }

        public void OnVisualizePath()
        {
            selectedVehicle.vehicleAnimation.trail.SetActive(true);
            selectedVehicle.vehicleKinematics.VisualizePath(selectedPath);
            // Update UI with simulation status
            StartCoroutine(UpdateSimulationUI());
        }

        public void OnStartPathSimulation()
        {
            selectedVehicle.vehicleAnimation.trail.SetActive(true);
            selectedVehicle.vehicleKinematics.SimulatePath(selectedPath);
            

            // TODO

            // Update UI with simulation status
            // StartCoroutine(UpdateSimulationUI());
        }

        public void SetSelectedVehicleFromDropdown()
        {
            if (applicationManager.vehicleFactory.productLookupTable.Values.Count > 0)
            {
                foreach (var vehicle in applicationManager.vehicleFactory.productLookupTable.Values)
                {
                    if (createdVehicleDropdown.options[createdVehicleDropdown.value].text == vehicle.productName)
                    {
                        selectedVehicle = vehicle;
                        break;
                    }
                }
            }
        }

        public void UpdateUIWithPathInfo()
        {
            statusText.text = $"Path: {selectedPath.pathName}";
            startPositionText.text = $"Start Position: ({selectedPath.frontaxle[0].x:F2}, {selectedPath.frontaxle[0].y:F2})";
            endPositionText.text = $"End Position: ({selectedPath.frontaxle[selectedPath.frontaxle.Count - 1].x:F2}, {selectedPath.frontaxle[selectedPath.frontaxle.Count - 1].y:F2})";
            //inputVelocityText.text = $"Input Velocity: {selectedPath.simInput.Vx:F2}";
        }

        public IEnumerator UpdateSimulationUI()
        {
            statusText.text = $"Visualizing Path {selectedPath.pathName}";
            float simulationTime = selectedPath.time[selectedPath.time.Count - 1];
            float elapsedTime = 0f;

            while (elapsedTime < simulationTime)
            {
                elapsedTime += Time.deltaTime;

                // Calculate the current index based on elapsed time
                int currentIndex = Mathf.FloorToInt((elapsedTime / simulationTime) * (selectedPath.time.Count - 1));

                // Update time remaining
                timeRemainingText.text = $"Time Remaining: {Mathf.Max(0, simulationTime - elapsedTime):0.00}s";

                // Update current steering input
                //inputSteeringText.text = $"Input Steering: {selectedVehicle.vehicleKinematics.inputSteerAngle:F2}";
                //inputVelocityText.text = $"Input Velocity: {selectedVehicle.vehicleKinematics.inputVelocity:F2}";
                yield return null;
            }

            // Simulation finished
            statusText.text = "Completed";
            timeRemainingText.text = "Time Remaining: 0.00s";
        }

        public void StartRecording()
        {
            if (selectedVehicle != null)
            {
                pathManager.StartRecording(selectedVehicle);
                statusText.text = "Recording Path...";
            }
        }

        public void StopRecording()
        {
            pathManager.StopRecording();
            statusText.text = "Path Recording Stopped.";
            PopulatePathDropdown();
        }
    }
}
