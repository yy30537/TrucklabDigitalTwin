using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SpaceFactory : Factory<SpaceProduct, SpaceConfig>
    {
        private VehicleFactory vehicleFactory;
        private GetClickedObject getClickedObject;

        public void Initialize(SystemLog systemLog, VehicleFactory vehicleFactory, GetClickedObject getClickedObject)
        {
            this.vehicleFactory = vehicleFactory;
            this.getClickedObject = getClickedObject;
            base.Initialize(systemLog);
        }

        protected override GameObject CreateProductInstance(SpaceConfig config)
        {
            var instance = new GameObject(config.spaceName);
            instance.transform.SetParent(instanceParent);
            instance.tag = "SpaceProduct";
            return instance;
        }

        protected override SpaceProduct InitializeProductComponent(GameObject instance, SpaceConfig config)
        {
            var product = instance.AddComponent<SpaceProduct>();
            product.Init(config, instance, mainCamera, systemLog, getClickedObject, vehicleFactory);

            InitializeSpaceUI(product, config);
            return product;
        }

        private void InitializeSpaceUI(SpaceProduct product, SpaceConfig config)
        {
            var dashboardInstance = Instantiate(config.spaceDashboard, dashboardParentTransform);
            var observer = dashboardInstance.AddComponent<SpaceDashboardObserver>();
            observer.Initialize(product, dashboardParentTransform);
        }

        protected override void RegisterProduct(SpaceConfig config, SpaceProduct product)
        {
            productLookupTable.Add(config.spaceID, product);
        }
    }
}
