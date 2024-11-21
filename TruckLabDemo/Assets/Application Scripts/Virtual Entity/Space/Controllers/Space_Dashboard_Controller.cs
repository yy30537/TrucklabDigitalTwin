using System.Linq;
using Application_Scripts.UI_Controller;
using Application_Scripts.UI_Controller.Application_UI;
using TMPro;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Space.Controllers
{
    /// <summary>
    /// UI component for displaying and managing information about a space.
    /// </summary>
    public class Space_Dashboard_Controller : Base_UI_Controller
    {
        /// <summary>
        /// UI element displaying the title of the space dashboard.
        /// </summary>
        [Header("UI References")]
        public TextMeshProUGUI Title;

        /// <summary>
        /// UI element displaying content about the vehicles in the space.
        /// </summary>
        public TextMeshProUGUI Content;

        /// <summary>
        /// Parent transform for UI elements associated with the space.
        /// </summary>
        public Transform VeUiInstanceParentTransform;

        /// <summary>
        /// Reference to the VE_Space this dashboard represents.
        /// </summary>
        [Header("VE References")]
        public VE_Space VeSpace;

        /// <summary>
        /// Configuration settings for the space.
        /// </summary>
        public Space_Config SpaceConfig;

        /// <summary>
        /// Color used to indicate a vacant space.
        /// </summary>
        private Color green;

        /// <summary>
        /// Color used to indicate an occupied space.
        /// </summary>
        private Color yellow;

        /// <summary>
        /// Unity's Update method to refresh the dashboard state if active.
        /// </summary>
        void Update()
        {
            if (IsActive)
            {
                UpdateDashboard();
            }
        }

        /// <summary>
        /// Sets the required dependencies for the space dashboard.
        /// </summary>
        /// <param name="ve_space">The space entity represented by this dashboard.</param>
        /// <param name="config">Configuration data for the space.</param>
        /// <param name="ui_instance">The UI GameObject instance for this dashboard.</param>
        /// <param name="ve_ui_instance_parent_transform">The parent transform for UI elements.</param>
        /// <param name="systemLog_ui_controller">Controller for logging system events.</param>
        public void SetDependencies(
            VE_Space ve_space,
            Space_Config config,
            GameObject ui_instance,
            Transform ve_ui_instance_parent_transform,
            UI_Controller_SystemLog systemLog_ui_controller
        )
        {
            VeSpace = ve_space;
            SpaceConfig = config;
            VeUiInstanceParentTransform = ve_ui_instance_parent_transform;
            UiInstance = ui_instance;
            UiName = config.Name;
            UiInstance.name = UiName;
            SystemLogUiController = systemLog_ui_controller;
            UiNavigationEventChannel = config.UiNavigationEventChannel;
        }

        /// <summary>
        /// Initializes the space dashboard by setting up UI components and colors.
        /// </summary>
        public override void Init()
        {
            base.Init();

            // Find the title and content UI elements in the hierarchy
            if (Title == null) Title = UiInstance.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
            if (Content == null) Content = UiInstance.transform.Find("Content")?.GetComponent<TextMeshProUGUI>();

            // Log an error if the necessary UI components are missing
            if (Title == null || Content == null)
            {
                SystemLogUiController.LogEvent($"Failed to find UI components for {GetType().Name}");
            }

            // Setup colors
            ColorUtility.TryParseHtmlString("#7BED9F", out green); // Green for vacant
            ColorUtility.TryParseHtmlString("#ECCC68", out yellow); // Yellow for occupied

            SystemLogUiController.LogEvent($"Initialized Space UI for: {UiName}");

            IsActive = true;
            UiInstance.SetActive(IsActive);
        }

        /// <summary>
        /// Updates the dashboard UI with the current state of the space and vehicles.
        /// </summary>
        public void UpdateDashboard()
        {
            if (VeSpace == null)
            {
                SystemLogUiController.LogEvent("Attempted to update UI with null VeSpace");
                return;
            }

            // Update the title and content to reflect the space's state
            Title.text = $"{VeSpace.Name}\n";
            Content.text = GetVehicleListText();
        }

        /// <summary>
        /// Generates text representing the list of vehicles in the space.
        /// </summary>
        /// <returns>A string containing information about vehicles present in the space.</returns>
        private string GetVehicleListText()
        {
            // Handle vacant state
            if (VeSpace.SpaceData.VehiclesPresentInSpace.Count == 0)
            {
                Content.color = green;
                return "Vacant";
            }

            // Handle occupied state
            Content.color = yellow;
            return "Occupied By:\n" + string.Join("\n",
                VeSpace.VehicleCreator.LookupTable
                    .Where(kvp => VeSpace.SpaceData.VehiclesPresentInSpace.ContainsKey(kvp.Key))
                    .Select(kvp => kvp.Value.Name));
        }
    }
}
