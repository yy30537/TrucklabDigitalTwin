using System;
using Application_Scripts.Virtual_Entity.Vehicle;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Building
{
    /// <summary>
    /// Factory class responsible for creating and managing buildings within the application.
    /// </summary>
    public class Building_Creator : VE_Creator<VE_Building, Building_Config>
    {
        /// <summary>
        /// Reference to the vehicle creator for managing vehicles within the building.
        /// </summary>
        [Header("Application Dependencies")]
        [SerializeField] protected Vehicle_Creator VehicleCreator;

        /// <summary>
        /// Utility for detecting mouse click interactions.
        /// </summary>
        [SerializeField] protected VE_OnClick_Getter VeOnClickGetter;

        /// <summary>
        /// Creates A new building entity based on the provided configuration.
        /// </summary>
        /// <param name="building_config">The configuration for the building.</param>
        /// <returns>The created building entity.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided configuration is null.</exception>
        public override VE_Building Create_VE(Building_Config building_config)
        {
            if (building_config == null) throw new ArgumentNullException(nameof(building_config));

            // Instantiate the building prefab as A GameObject in the scene
            var ve_instance = Instantiate(building_config.BuildingPrefab, VeInstanceParentTransform);

            // Add the VE_Building component to the GameObject
            var ve_building = ve_instance.AddComponent<VE_Building>();

            // Set dependencies for the building
            ve_building.SetDependencies(
                ve_instance,
                building_config,
                VehicleCreator,
                VeInstanceParentTransform,
                VeUiParentTransform,
                CameraManager,
                SystemLogUiController,
                VeOnClickGetter);

            // Initialize the building
            ve_building.Init();

            // Register the building in the lookup table
            Register_VE(ve_building, building_config);

            // Log the creation event
            SystemLogUiController.LogEvent($"Created Building: {ve_building.Name}");

            return ve_building;
        }

        /// <summary>
        /// Deletes an existing building entity based on its ID.
        /// </summary>
        /// <param name="ve_id">The ID of the building to delete.</param>
        public override void Delete_VE(int ve_id)
        {
            if (LookupTable.TryGetValue(ve_id, out var product))
            {
                // Log the deletion event
                SystemLogUiController.LogEvent($"Deleting Building: ID={ve_id}");

                // Destroy the UI instance and the GameObject
                Destroy(product.BuildingDashboardController.UiInstance);
                Destroy(product.gameObject);

                // Remove the building from the lookup table
                LookupTable.Remove(ve_id);
            }
            else
            {
                // Log if the building was not found
                SystemLogUiController.LogEvent($"Building not found: ID={ve_id}");
            }
        }
    }
}
