using System.Collections.Generic;
using RosSharp.RosBridgeClient;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Factory class for creating and managing VehicleProduct instances.
    /// </summary>
    public class VehicleFactory : Factory<VehicleProduct, VehicleConfig>
    {
        private GetClickedObject getClickedObject;
        private SetSimulationServiceProvider setSimulationServiceProvider;

        /// <summary>
        /// Initializes the VehicleFactory with required dependencies.
        /// </summary>
        /// <param name="systemLog">System log for logging events.</param>
        /// <param name="getClickedObject">Service for detecting clicked objects.</param>
        /// <param name="setSimulationServiceProvider">Service provider for simulation settings.</param>
        public void Initialize(SystemLog systemLog, GetClickedObject getClickedObject, SetSimulationServiceProvider setSimulationServiceProvider)
        {
            this.getClickedObject = getClickedObject;
            this.setSimulationServiceProvider = setSimulationServiceProvider;
            base.Initialize(systemLog);
        }

        /// <summary>
        /// Creates a new instance of the vehicle product.
        /// </summary>
        /// <param name="config">Configuration for the vehicle product.</param>
        /// <returns>Newly created GameObject instance.</returns>
        protected override GameObject CreateProductInstance(VehicleConfig config)
        {
            var instance = Instantiate(config.vehiclePrototypePrefab, instanceParent);
            instance.name = config.vehicleName;
            systemLog.LogEvent("Created Vehicle Product Instance: " + instance.name);
            return instance;
        }

        /// <summary>
        /// Initializes the VehicleProduct component of the created instance.
        /// </summary>
        /// <param name="instance">GameObject instance of the vehicle product.</param>
        /// <param name="config">Configuration for the vehicle product.</param>
        /// <returns>Initialized VehicleProduct component.</returns>
        protected override VehicleProduct InitializeProductComponent(GameObject instance, VehicleConfig config)
        {
            var product = instance.AddComponent<VehicleProduct>();
            product.Init(config, instance, mainCamera, dashboardParentTransform, systemLog, getClickedObject, setSimulationServiceProvider);
            systemLog.LogEvent("Initialized Vehicle Product Components: " + product.productName);
            return product;
        }

        /// <summary>
        /// Registers the vehicle product in the lookup table.
        /// </summary>
        /// <param name="config">Configuration for the vehicle product.</param>
        /// <param name="product">Vehicle product instance.</param>
        protected override void RegisterProduct(VehicleConfig config, VehicleProduct product)
        {
            productLookupTable.Add(config.vehicleID, product);
            systemLog.LogEvent("Registered Vehicle Product in Factory: " + product.productName);
        }
    }
}
