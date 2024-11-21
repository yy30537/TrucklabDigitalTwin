using Application_Scripts.Event_Channel;
using Application_Scripts.Virtual_Entity.Space;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Vehicle
{
    /// <summary>
    /// Factory for creating and managing vehicle entities within the simulation.
    /// </summary>
    public class Vehicle_Creator : VE_Creator<VE_Vehicle, Vehicle_Config>
    {
        /// <summary>
        /// Event channel for notifying vehicle registration events.
        /// </summary>
        [Header("Event Channels")]
        public EventChannel_Void VeVehicleRegistrationEventChannel;

        /// <summary>
        /// Event channel for notifying vehicle deletion events.
        /// </summary>
        public EventChannel_Void VeVehicleDeletionEventChannel;

        /// <summary>
        /// Reference to the vehicle creator instance for managing dependencies.
        /// </summary>
        [Header("Application Dependencies")]
        [SerializeField] protected Vehicle_Creator VehicleCreator;

        /// <summary>
        /// Reference to the space creator instance for associating vehicles with spaces.
        /// </summary>
        [SerializeField] protected Space_Creator SpaceCreator;

        /// <summary>
        /// Utility for detecting objects via mouse clicks.
        /// </summary>
        [SerializeField] protected VE_OnClick_Getter VeOnClickGetter;

        /// <summary>
        /// Creates A new vehicle entity based on the provided configuration.
        /// </summary>
        /// <param name="vehicle_config">The configuration object for the vehicle.</param>
        /// <returns>The created vehicle entity.</returns>
        public override VE_Vehicle Create_VE(Vehicle_Config vehicle_config)
        {
            // Instantiate the vehicle prefab
            var ve_instance = Instantiate(vehicle_config.VehiclePrototypePrefab, VeInstanceParentTransform);

            // Add the VE_Vehicle component to the GameObject
            var ve_vehicle = ve_instance.AddComponent<VE_Vehicle>();

            // Set the dependencies for the vehicle
            ve_vehicle.SetDependencies(
                ve_instance,
                VeInstanceParentTransform,
                VeUiParentTransform,
                vehicle_config,
                CameraManager,
                VehicleCreator,
                SpaceCreator,
                SystemLogUiController,
                VeOnClickGetter);

            // Initialize the vehicle
            ve_vehicle.Init();

            // Register the vehicle in the lookup table
            Register_VE(ve_vehicle, vehicle_config);

            // Raise the registration event
            VeVehicleRegistrationEventChannel.RaiseEvent();

            // Log the creation event
            SystemLogUiController.LogEvent($"Created Vehicle: {ve_vehicle.Name}");

            return ve_vehicle;
        }

        /// <summary>
        /// Deletes an existing vehicle entity based on its ID.
        /// </summary>
        /// <param name="ve_id">The ID of the vehicle to delete.</param>
        public override void Delete_VE(int ve_id)
        {
            if (LookupTable.TryGetValue(ve_id, out var product))
            {
                // Log the deletion event
                SystemLogUiController.LogEvent($"Deleting Vehicle: ID={ve_id}");

                // Destroy the UI instance and the GameObject
                Destroy(product.VehicleDashboardController.UiInstance);
                Destroy(product.gameObject);

                // Remove the vehicle from the lookup table
                LookupTable.Remove(ve_id);
            }
            else
            {
                // Log if the vehicle was not found
                SystemLogUiController.LogEvent($"Vehicle not found: ID={ve_id}");
            }

            // Raise the deletion event
            VeVehicleDeletionEventChannel.RaiseEvent();
        }
    }
}
