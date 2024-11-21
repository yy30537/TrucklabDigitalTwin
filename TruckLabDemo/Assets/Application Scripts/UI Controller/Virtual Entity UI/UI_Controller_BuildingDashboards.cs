using Application_Scripts.Virtual_Entity;
using Application_Scripts.Virtual_Entity.Building;
using UnityEngine;

namespace Application_Scripts.UI_Controller.Virtual_Entity_UI
{
    /// <summary>
    /// Controls the visibility and interaction with the building dashboards.
    /// Extends the functionality of the base UI controller.
    /// </summary>
    public class UI_Controller_BuildingDashboards : Base_UI_Controller
    {
        /// <summary>
        /// Utility for detecting clicked objects in the scene.
        /// </summary>
        [Header("===Children: UI_Controller_BuildingDashboards===")]
        public VE_OnClick_Getter VeOnClickGetterGetter;

        /// <summary>
        /// Stores the currently clicked building in the scene.
        /// </summary>
        public VE_Building ClickedBuilding;

        /// <summary>
        /// Initializes the building dashboard UI controller.
        /// Called when the script instance is loaded.
        /// </summary>
        void Start()
        {
            // Initialize the base UI controller.
            Init();
        }

        /// <summary>
        /// Handles updates for the building dashboards during each frame.
        /// If the dashboard is active, logic for detecting building clicks can be added here.
        /// </summary>
        void Update()
        {
            if (IsActive)
            {
                // Placeholder for detecting building clicks.
                // Uncomment and expand logic if needed.
                // ClickedBuilding = DetectBuildingClick();
                // if (ClickedBuilding != null)
                // {
                //     // Handle building-specific functionality here.
                // }
            }
        }

        /// <summary>
        /// Detects if a building object has been clicked in the scene.
        /// Returns the building instance if a valid building object is clicked.
        /// </summary>
        /// <returns>The clicked <see cref="VE_Building"/> instance, or null if no building is clicked.</returns>
        public VE_Building DetectBuildingClick()
        {
            // Check if the left mouse button is pressed and if the click getter is available.
            if (Input.GetMouseButtonDown(0) && VeOnClickGetterGetter != null)
            {
                // Get the object under the mouse click.
                GameObject hitObject = VeOnClickGetterGetter.ReturnClickedObject();

                // Check if the object is tagged as a "Building".
                if (hitObject != null && hitObject.CompareTag("Building"))
                {
                    // Retrieve and return the parent building component.
                    VE_Building clickedBuilding = hitObject.GetComponentInParent<VE_Building>();
                    return clickedBuilding;
                }
            }

            // Return null if no valid building is detected.
            return null;
        }
    }
}
