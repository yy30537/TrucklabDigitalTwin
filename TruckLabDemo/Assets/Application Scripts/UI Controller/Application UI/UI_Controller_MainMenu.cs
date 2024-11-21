using Application_Scripts.Manager;
using Application_Scripts.UI_Controller.Virtual_Entity_UI;
using Application_Scripts.Virtual_Entity.Vehicle.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Application_Scripts.UI_Controller.Application_UI
{
    /// <summary>
    /// Manages the main menu UI and its interaction with submenus, toggles, and application dependencies.
    /// Handles the toggling of various dashboard panels and updates their states.
    /// </summary>
    public class UI_Controller_MainMenu : Base_UI_Controller
    {
        /// <summary>
        /// Reference to the camera manager, used for controlling camera settings.
        /// </summary>
        [Header("===Children: UI_Controller_MainMenu===")]
        public Camera_Manager CameraManager;

        /// <summary>
        /// Reference to the vehicle dragger for enabling drag-and-drop functionality.
        /// </summary>
        public Vehicle_Dragger VehicleDragger;

        /// <summary>
        /// References to sub-menu UI controllers for managing different panels in the UI.
        /// </summary>
        [Header("Sub Menu UI Controllers")]
        public UI_Controller_ServiceMenu UiControllerServiceMenu;
        public UI_Controller_SystemLog UiControllerSystemLog;
        public UI_Controller_SpaceDashboards UiControllerSpaceDashboards;
        public UI_Controller_BuildingDashboards UiControllerBuildingDashboards;
        public UI_Controller_VehicleDashboards UiControllerVehicleDashboards;

        /// <summary>
        /// Toggle UI elements for interacting with various submenus and dashboard panels.
        /// </summary>
        [Header("UI Objects")]
        public Toggle ServiceMenuToggle;
        public Toggle SystemLogToggle;
        public Toggle SpaceDashboardPanelToggle;
        public Toggle BuildingDashboardPanelToggle;
        public Toggle VehicleDashboardPanelToggle;
        public Toggle FreeLookMainMenuToggle;
        public Toggle DragVehicleToggle;

        /// <summary>
        /// Images associated with the toggles, used for visual feedback.
        /// </summary>
        public Image ServiceMenuToggleImage;
        public Image SystemLogToggleImage;
        public Image SpaceDashboardPanelToggleImage;
        public Image BuildingDashboardPanelToggleImage;
        public Image VehicleDashboardPanelToggleImage;
        public Image FreeLookMainMenuToggleImage;
        public Image DragVehicleMainMenuImage;

        /// <summary>
        /// Colors used for toggles to indicate active (green) or inactive (red) states.
        /// </summary>
        [Header("Colors")]
        private Color green;
        private Color red;

        /// <summary>
        /// Initializes the main menu controller and its dependencies.
        /// Sets up colors for toggle states.
        /// </summary>
        void Start()
        {
            Init();

            // Setup colors for active and inactive states
            ColorUtility.TryParseHtmlString("#7BED9F", out green); // Light green for active state
            ColorUtility.TryParseHtmlString("#FF6B81", out red);   // Light red for inactive state
        }

        /// <summary>
        /// Updates the color of a toggle based on its active state.
        /// </summary>
        /// <param name="toggle">The toggle being updated.</param>
        /// <param name="toggleImage">The image associated with the toggle.</param>
        /// <param name="is_visible">Indicates whether the toggle is active or not.</param>
        private void SetToggleColor(Toggle toggle, Image toggleImage, bool is_visible)
        {
            toggleImage.color = Color.clear; // Reset to transparent first
            toggleImage.color = is_visible ? green : red; // Set to green or red based on the state
        }

        /// <summary>
        /// Updates the toggle state and color for the service menu.
        /// </summary>
        public void OnServiceMenuToggle()
        {
            SetToggleColor(ServiceMenuToggle, ServiceMenuToggleImage, UiControllerServiceMenu.IsActive);
        }

        /// <summary>
        /// Updates the toggle state and color for the system log.
        /// </summary>
        public void OnSystemLogToggle()
        {
            SetToggleColor(SystemLogToggle, SystemLogToggleImage, UiControllerSystemLog.IsActive);
        }

        /// <summary>
        /// Updates the toggle state and color for the space dashboards.
        /// </summary>
        public void OnSpaceDashboardToggle()
        {
            SetToggleColor(SpaceDashboardPanelToggle, SpaceDashboardPanelToggleImage, UiControllerSpaceDashboards.IsActive);
        }

        /// <summary>
        /// Updates the toggle state and color for the building dashboards.
        /// </summary>
        public void OnBuildingDashboardToggle()
        {
            SetToggleColor(BuildingDashboardPanelToggle, BuildingDashboardPanelToggleImage, UiControllerBuildingDashboards.IsActive);
        }

        /// <summary>
        /// Updates the toggle state and color for the vehicle dashboards.
        /// </summary>
        public void OnVehicleDashboardToggle()
        {
            SetToggleColor(VehicleDashboardPanelToggle, VehicleDashboardPanelToggleImage, UiControllerVehicleDashboards.IsActive);
        }

        /// <summary>
        /// Updates the toggle state and color for the free look camera control.
        /// </summary>
        public void OnFreeLookToggle()
        {
            SetToggleColor(FreeLookMainMenuToggle, FreeLookMainMenuToggleImage, CameraManager.IsControlActive);
        }

        /// <summary>
        /// Updates the toggle state and color for the drag vehicle functionality.
        /// </summary>
        public void OnDragVehicleToggle()
        {
            SetToggleColor(DragVehicleToggle, DragVehicleMainMenuImage, VehicleDragger.IsDraggingEnabled);
        }
    }
}
