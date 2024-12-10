using ApplicationScripts.Manager;
using ApplicationScripts.UIController.ApplicationUI;
using ApplicationScripts.VirtualEntity.Building.Controllers;
using ApplicationScripts.VirtualEntity.Vehicle;
using UnityEngine;

namespace ApplicationScripts.VirtualEntity.Building
{
    /// <summary>
    /// Represents A building entity in the simulation, encapsulating its data, controllers, and dependencies.
    /// </summary>
    public class VE_Building : VE
    {
        /// <summary>
        /// Configuration data for the building, including initialization and properties.
        /// </summary>
        [Header("Building Data")]
        public Building_Config Config;

        /// <summary>
        /// Runtime data associated with the building.
        /// </summary>
        public Building_Data BuildingData;

        /// <summary>
        /// Core controller for managing the building's behavior and interactions.
        /// </summary>
        [Header("Building Controllers")]
        public Building_Controller BuildingController;

        /// <summary>
        /// UI dashboard controller for the building, providing visualization and interaction tools.
        /// </summary>
        public Building_Dashboard_Controller BuildingDashboardController;

        /// <summary>
        /// Camera manager responsible for camera-related operations involving the building.
        /// </summary>
        [Header("Application Dependencies")]
        public Camera_Manager CameraManager;

        /// <summary>
        /// System log UI controller for recording and displaying building-related events.
        /// </summary>
        public UI_Controller_SystemLog SystemLogUiController;

        /// <summary>
        /// Reference to the vehicle creator responsible for managing vehicles associated with the building.
        /// </summary>
        public Vehicle_Creator VehicleCreator;

        /// <summary>
        /// Utility for detecting objects via mouse clicks within the building context.
        /// </summary>
        public VE_OnClick_Getter VeOnClickGetter;

        /// <summary>
        /// Parent transform for UI elements associated with the building.
        /// </summary>
        public Transform VeUiInstanceParentTransform;

        /// <summary>
        /// Parent transform for the building instance in the scene.
        /// </summary>
        public Transform VeInstanceParentTransform;

        /// <summary>
        /// Sets the dependencies required for the building entity.
        /// </summary>
        /// <param name="ve_instance">The building GameObject instance in the scene.</param>
        /// <param name="building_config">The configuration object for the building.</param>
        /// <param name="vehicle__creator">The vehicle creator managing vehicles related to this building.</param>
        /// <param name="ve_instance_parent_transform">The parent transform for the building instance.</param>
        /// <param name="ve_ui_parent_transform">The parent transform for UI elements associated with the building.</param>
        /// <param name="camera_manager">The camera manager handling camera operations.</param>
        /// <param name="systemLog_ui_controller">The system log UI controller for recording events.</param>
        /// <param name="ve_onclick_getter">Utility for detecting and handling mouse click interactions.</param>
        public void SetDependencies(
            GameObject ve_instance,
            Building_Config building_config,
            Vehicle_Creator vehicle__creator,
            Transform ve_instance_parent_transform,
            Transform ve_ui_parent_transform,
            Camera_Manager camera_manager,
            UI_Controller_SystemLog systemLog_ui_controller,
            VE_OnClick_Getter ve_onclick_getter)
        {
            // Assign dependencies to corresponding fields
            Config = building_config;
            Instance = ve_instance;
            CameraManager = camera_manager;
            VehicleCreator = vehicle__creator;
            SystemLogUiController = systemLog_ui_controller;
            VeOnClickGetter = ve_onclick_getter;
            VeInstanceParentTransform = ve_instance_parent_transform;
            VeUiInstanceParentTransform = ve_ui_parent_transform;
        }

        /// <summary>
        /// Initializes the building entity, including data, controllers, and its dashboard.
        /// </summary>
        public override void Init()
        {
            // Common initialization of the building
            Id = Config.Id;
            Name = Config.Name;
            Instance.name = Name;
            Instance.tag = "Building";

            // Initialize runtime data for the building
            BuildingData = Instance.AddComponent<Building_Data>();
            BuildingData.Init(Config);

            // Initialize the building's controller
            BuildingController = Instance.AddComponent<Building_Controller>();
            BuildingController.Init(BuildingData, VehicleCreator);

            // Initialize the building's dashboard UI
            var ui_instance = Instantiate(Config.UiTemplate, VeUiInstanceParentTransform);
            BuildingDashboardController = ui_instance.AddComponent<Building_Dashboard_Controller>();
            BuildingDashboardController.SetDependencies(
                Config,
                ui_instance,
                VeUiInstanceParentTransform,
                SystemLogUiController
            );
            BuildingDashboardController.Init();

            // Set the building's position and orientation in the scene
            SetBuildingPosition();

            // Log the initialization event
            SystemLogUiController.LogEvent($"Initialized Building: {Name}");
        }

        /// <summary>
        /// Sets the position and orientation of the building in the scene based on its configuration.
        /// </summary>
        private void SetBuildingPosition()
        {
            Instance.transform.position = Config.BuildingPosition;
            Instance.transform.rotation = Quaternion.Euler(0, Config.BuildingOrientation, 0);
        }
    }
}
