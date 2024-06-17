using UnityEngine;

namespace Core
{
    /// <summary>
    /// Factory class for creating and managing VehicleProduct instances.
    /// </summary>
    public class VehicleFactory : Factory<VehicleProduct, VehicleConfig>
    {
        public override IProduct GetProduct(VehicleConfig config)
        {
            var instance = Instantiate(config.vehiclePrototypePrefab, instanceParent);
            instance.name = config.vehicleName;
            systemLog.LogEvent("Created Vehicle IProduct Instance: " + instance.name);
            var product = instance.AddComponent<VehicleProduct>();
            
            product.productID = config.vehicleID;
            product.productName = config.vehicleName;
            product.productInstance = instance;
            product.mainCamera = mainCamera;
            product.systemLog = FindObjectOfType<SystemLog>();
            product.getClickedObject = FindObjectOfType<GetClickedObject>();
            product.vehicleConfig = config;
            product.vehicleDashboardParent = dashboardParentTransform;
            

            
            product.Initialize();
            systemLog.LogEvent("Initialized Vehicle IProduct Components: " + product.productName);
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
            systemLog.LogEvent("Registered Vehicle IProduct in Factory: " + product.productName);
        }
    }
}
