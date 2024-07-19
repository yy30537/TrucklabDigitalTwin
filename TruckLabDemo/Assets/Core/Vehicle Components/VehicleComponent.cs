using UnityEngine;

namespace Core
{
    /// <summary>
    /// Abstract base class for components that belong to a vehicle.
    /// Provides initialization functionality for vehicle-specific components.
    /// </summary>
    public abstract class VehicleComponent : MonoBehaviour
    {
        /// <summary>
        /// Gets the vehicle product associated with this component.
        /// </summary>
        protected VehicleProduct VehicleProduct { get; private set; }

        /// <summary>
        /// Gets the vehicle data associated with this component.
        /// </summary>
        protected VehicleData VehicleData { get; private set; }

        /// <summary>
        /// Initializes the vehicle component with the specified vehicle product and data.
        /// </summary>
        /// <param name="vehicleProduct">The vehicle product associated with this component.</param>
        /// <param name="vehicleData">The vehicle data associated with this component.</param>
        public virtual void Initialize(VehicleProduct vehicleProduct, VehicleData vehicleData)
        {
            this.VehicleProduct = vehicleProduct;
            VehicleData = vehicleData;
        }
    }
}