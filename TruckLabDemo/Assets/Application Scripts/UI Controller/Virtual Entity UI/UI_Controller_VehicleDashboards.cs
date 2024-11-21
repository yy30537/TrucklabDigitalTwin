using Application_Scripts.Virtual_Entity;
using Application_Scripts.Virtual_Entity.Vehicle;
using UnityEngine;

namespace Application_Scripts.UI_Controller.Virtual_Entity_UI
{
    /// <summary>
    /// Controls the visibility and interaction with vehicle dashboards.
    /// Extends the functionality of the base UI controller.
    /// </summary>
    public class UI_Controller_VehicleDashboards : Base_UI_Controller
    {
        /// <summary>
        /// Utility for detecting clicked objects in the scene.
        /// </summary>
        [Header("===Children: UI_Controller_VehicleDashboards===")]
        public VE_OnClick_Getter VeOnClickGetterGetter;

        /// <summary>
        /// Stores the currently clicked vehicle in the scene.
        /// </summary>
        public VE_Vehicle ClickedVehicle;

        /// <summary>
        /// Initializes the vehicle dashboard UI controller.
        /// Called when the script instance is loaded.
        /// </summary>
        void Start()
        {
            // Initialize the base UI controller.
            Init();
        }

        /// <summary>
        /// Handles updates for the vehicle dashboards during each frame.
        /// If the dashboard is active, detects clicks on vehicles and shows their dashboards.
        /// </summary>
        void Update()
        {
            if (IsActive)
            {
                // Detect if a vehicle has been clicked.
                ClickedVehicle = DetectVehicleClick();

                // If a valid vehicle is clicked, show its dashboard UI.
                if (ClickedVehicle != null)
                {
                    ClickedVehicle.VehicleDashboardController.ShowUi();
                }
            }
        }

        /// <summary>
        /// Detects if a vehicle object has been clicked in the scene.
        /// Returns the vehicle instance if a valid vehicle object is clicked.
        /// </summary>
        /// <returns>The clicked <see cref="VE_Vehicle"/> instance, or null if no vehicle is clicked.</returns>
        public VE_Vehicle DetectVehicleClick()
        {
            // Check if the left mouse button is pressed and if the click getter is available.
            if (Input.GetMouseButtonDown(0) && VeOnClickGetterGetter != null)
            {
                // Get the object under the mouse click.
                GameObject hitObject = VeOnClickGetterGetter.ReturnClickedObject();

                // Check if the object is tagged as a "Tractor" or "Trailer".
                if (hitObject != null && (hitObject.CompareTag("Tractor") || hitObject.CompareTag("Trailer")))
                {
                    // Retrieve and return the parent vehicle component.
                    VE_Vehicle clickedVeVehicle = hitObject.GetComponentInParent<VE_Vehicle>();
                    return clickedVeVehicle;
                }
            }

            // Return null if no valid vehicle is detected.
            return null;
        }
    }
}
