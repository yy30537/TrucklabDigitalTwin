using UnityEngine;

namespace Application_Scripts.UI_Controller.Application_UI
{
    /// <summary>
    /// Manages the service menu UI, including visibility toggling and interaction
    /// with dependent child controllers for vehicle creation and path simulation.
    /// </summary>
    public class UI_Controller_ServiceMenu : Base_UI_Controller
    {
        /// <summary>
        /// Controller for managing the vehicle creation UI.
        /// </summary>
        [Header("===Children: UI_Controller_ServiceMenu===")]
        [Header("Service Managers")]
        public UI_Controller_VE_Creator UiControllerVeCreator;

        /// <summary>
        /// Controller for managing the path simulation UI.
        /// </summary>
        public UI_Controller_PathSimulation UiControllerPathSimulation;

        /// <summary>
        /// Initializes the service menu by calling the base initialization method.
        /// </summary>
        void Start()
        {
            Init();
        }

        /// <summary>
        /// Toggles the visibility of the service menu based on the provided UI name.
        /// Ensures that only one dependent UI (vehicle creation or path simulation) is visible at a Time.
        /// </summary>
        /// <param name="target_ui_name">The name of the UI to toggle visibility for.</param>
        public override void Toggle_UI_Visibility(string target_ui_name)
        {
            base.Toggle_UI_Visibility(target_ui_name); // Call base implementation for standard toggle behavior

            if (string.IsNullOrEmpty(UiName))
            {
                Debug.LogWarning($"UiName is not set for {gameObject.name}");
                return;
            }

            // Hide the path simulation UI if the vehicle creation UI is being toggled
            if (UiControllerVeCreator.UiName == target_ui_name)
            {
                UiControllerPathSimulation.HideUi();
            }

            // Hide the vehicle creation UI if the path simulation UI is being toggled
            if (UiControllerPathSimulation.UiName == target_ui_name)
            {
                UiControllerVeCreator.HideUi();
            }

            // Update the visibility status of this menu
            Set_UI_Visibility(IsActive);
        }
    }
}
