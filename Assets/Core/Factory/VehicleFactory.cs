using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /*
     * create and initialize VehicleProduct instances
     */
    public class VehicleFactory : Factory<VehicleProduct, VehicleConfig>
    {
        
        protected override GameObject CreateProductInstance(VehicleConfig config)
        {
            var instance = Instantiate(config.vehiclePrototypePrefab, instanceParent);
            instance.name = config.vehicleName;
            systemLog.LogEvent("Created Vehicle Product Instance: " + instance.name);
            return instance;
        }

        protected override VehicleProduct InitializeProductComponent(GameObject instance, VehicleConfig config)
        {
            var product = instance.AddComponent<VehicleProduct>();
            product.Init(config, instance, mainCamera, dashboardParentTransform);
            systemLog.LogEvent("Initialized Vehicle Product Components: " + product.productName);
            return product;
        }

        protected override void RegisterProduct(VehicleConfig config, VehicleProduct product)
        {
            productLookupTable.Add(config.vehicleID, product);
            systemLog.LogEvent("Registered Vehicle Product in Factory: " + product.productName);
        }
    }
}


