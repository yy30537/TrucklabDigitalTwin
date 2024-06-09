using System.Collections.Generic;
using RosSharp.RosBridgeClient;
using UnityEngine;

namespace Core
{
    public class VehicleFactory : Factory<VehicleProduct, VehicleConfig>
    {
        private GetClickedObject getClickedObject;
        private SetSimulationServiceProvider setSimulationServiceProvider;

        public void Initialize(SystemLog systemLog, GetClickedObject getClickedObject, SetSimulationServiceProvider setSimulationServiceProvider)
        {
            this.getClickedObject = getClickedObject;
            this.setSimulationServiceProvider = setSimulationServiceProvider;
            base.Initialize(systemLog);
        }

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
            product.Init(config, instance, mainCamera, dashboardParentTransform, systemLog, getClickedObject, setSimulationServiceProvider);
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
