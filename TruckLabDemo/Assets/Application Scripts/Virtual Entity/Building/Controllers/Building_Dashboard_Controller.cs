using Application_Scripts.Event_Channel;
using Application_Scripts.UI_Controller;
using Application_Scripts.UI_Controller.Application_UI;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Building.Controllers
{
    // TODO: Once scheduling and path planning services are implemented from the backend,
    //       create additional UI elements for updating the DC (Distribution Center) schedule.

    /// <summary>
    /// UI component for displaying and managing information related to a building (e.g., Distribution Center docks).
    /// </summary>
    public class Building_Dashboard_Controller : Base_UI_Controller
    {
        /// <summary>
        /// Parent transform under which UI elements for the building will be instantiated.
        /// </summary>
        [Header("UI References")]
        public Transform VeUiInstanceParentTransform;

        /// <summary>
        /// Configuration data for the building, including UI and behavior settings.
        /// </summary>
        [Header("VE References")]
        public Building_Config BuildingConfig;

        /// <summary>
        /// Event channel for managing UI navigation events specific to this building.
        /// </summary>
        public EventChannel_UI_Navigation UiNavigationEc;

        /// <summary>
        /// Periodically updates the dashboard's state and visuals.
        /// </summary>
        void Update()
        {
            // The update method can be used for dynamic dashboard updates.
            // Currently commented out as the feature is not active.
            //if (IsActive)
            //{
            //    UpdateDashboard();
            //}
        }

        /// <summary>
        /// Sets the required dependencies for the building's dashboard.
        /// </summary>
        /// <param name="config">The building configuration containing initialization data.</param>
        /// <param name="ui_instance">The UI GameObject instance for the building.</param>
        /// <param name="ve_ui_instance_parent_transform">The parent transform for the building's UI instance.</param>
        /// <param name="systemLog_ui_controller">The system log controller for event logging.</param>
        public void SetDependencies(
            Building_Config config,
            GameObject ui_instance,
            Transform ve_ui_instance_parent_transform,
            UI_Controller_SystemLog systemLog_ui_controller
        )
        {
            BuildingConfig = config;
            VeUiInstanceParentTransform = ve_ui_instance_parent_transform;
            UiInstance = ui_instance;
            UiName = config.Name; // Set the UI's name from the building config.
            UiInstance.name = UiName;
            SystemLogUiController = systemLog_ui_controller;
            UiNavigationEventChannel = BuildingConfig.UiNavigationEventChannel;
        }

        /// <summary>
        /// Initializes the building's dashboard by activating it and logging its initialization.
        /// </summary>
        public override void Init()
        {
            base.Init();
            SystemLogUiController.LogEvent($"Initialized Building UI for: {UiName}");
            IsActive = true;
            UiInstance.SetActive(true);
        }

        /// <summary>
        /// Updates the dashboard's UI to reflect the current state of the building.
        /// </summary>
        public void UpdateDashboard()
        {
            // Prevent updates if the dashboard is not visible or the UI instance is null.
            if (!IsDashboardVisible() || UiInstance == null) return;

            // Example of updating the dashboard with vehicle information.
            // Uncomment and adapt this logic as required.
            //if (VE_Building.Vehicle_Creator.LookupTable.Any())
            //{
            //    title.text = "DC Hosting: \n";
            //    foreach (var VE_Vehicle in VE_Building.Vehicle_Creator.LookupTable)
            //    {
            //        title.text += $"[{VE_Vehicle.Value.Name}] \n";
            //    }
            //}
        }

        /// <summary>
        /// Detects whether the building was clicked by the user.
        /// </summary>
        /// <returns>True if the building was clicked; otherwise, false.</returns>
        public bool DetectBuildingClick()
        {
            // Placeholder implementation for detecting building clicks.
            // Uncomment and implement as required.
            //if (Input.GetMouseButtonDown(0))
            //{
            //    GameObject hitObject = VeOnClickGetter.ReturnClickedObject();
            //    if (hitObject != null && hitObject.CompareTag("DC"))
            //    {
            //        VE_Building clickedDock = hitObject.GetComponentInParent<VE_Building>();
            //        return clickedDock != null && clickedDock.Id == VE_Building.Id;
            //    }
            //}
            return false;
        }
    }
}
