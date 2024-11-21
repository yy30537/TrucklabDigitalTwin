using Application_Scripts.Virtual_Entity.Vehicle;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Space
{
    /// <summary>
    /// Factory for creating and managing region products (spaces).
    /// </summary>
    public class Space_Creator : VE_Creator<VE_Space, Space_Config>
    {
        /// <summary>
        /// Reference to the site layout GameObject for visualizing the region.
        /// </summary>
        [Header("===Children: Building_Creator===")]
        [SerializeField] protected GameObject SiteLayout;

        /// <summary>
        /// Reference to the vehicle creator for managing vehicles within the region.
        /// </summary>
        [Header("Application Dependencies")]
        [SerializeField] protected Vehicle_Creator VehicleCreator;

        /// <summary>
        /// Utility for detecting mouse click interactions.
        /// </summary>
        [SerializeField] protected VE_OnClick_Getter VeOnClickGetter;

        /// <summary>
        /// Creates A new space entity based on the provided configuration.
        /// </summary>
        /// <param name="space_config">The configuration for the space.</param>
        /// <returns>The created space entity.</returns>
        public override VE_Space Create_VE(Space_Config space_config)
        {
            // Instantiate A new GameObject for the space
            var ve_instance = new GameObject(space_config.Name);

            // Add the VeSpace component to the GameObject
            var ve_space = ve_instance.AddComponent<VE_Space>();

            // Set dependencies for the space
            ve_space.SetDependencies(
                ve_instance,
                space_config,
                VehicleCreator,
                VeInstanceParentTransform,
                VeUiParentTransform,
                CameraManager,
                SystemLogUiController,
                VeOnClickGetter);

            // Initialize the space
            ve_space.Init();

            // Register the space in the lookup table
            Register_VE(ve_space, space_config);

            // Log the creation event
            SystemLogUiController.LogEvent($"Created Space: {ve_space.Name}");

            // Optionally enable the layout visualization
            SetLayout(true);

            return ve_space;
        }

        /// <summary>
        /// Deletes an existing space entity based on its ID.
        /// </summary>
        /// <param name="ve_id">The ID of the space to delete.</param>
        public override void Delete_VE(int ve_id)
        {
            if (LookupTable.TryGetValue(ve_id, out var product))
            {
                // Log the deletion event
                SystemLogUiController.LogEvent($"Deleting Space: ID={ve_id}");

                // Destroy the UI instance and the GameObject
                Destroy(product.SpaceDashboardController.UiInstance);
                Destroy(product.gameObject);

                // Remove the space from the lookup table
                LookupTable.Remove(ve_id);
            }
            else
            {
                // Log if the space was not found
                SystemLogUiController.LogEvent($"Space not found: ID={ve_id}");
            }
        }

        /// <summary>
        /// Enables or disables the site layout visualization.
        /// </summary>
        /// <param name="status">Whether to enable or disable the layout.</param>
        public void SetLayout(bool status)
        {
            SiteLayout.SetActive(status);
        }
    }
}
