using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class DockFactory : Factory<DockProduct, DockConfig>
    {
        private GetClickedObject getClickedObject;

        public void Initialize(SystemLog systemLog, GetClickedObject getClickedObject)
        {
            this.getClickedObject = getClickedObject;
            base.Initialize(systemLog);
        }

        protected override GameObject CreateProductInstance(DockConfig config)
        {
            var instance = Instantiate(config.dockBuildingPrefab, instanceParent);
            instance.name = config.dockStationName;
            return instance;
        }

        protected override DockProduct InitializeProductComponent(GameObject instance, DockConfig config)
        {
            var product = instance.AddComponent<DockProduct>();
            product.Init(config, instance, mainCamera, systemLog, getClickedObject);
            InitializeDockUIObserver(product, config);
            return product;
        }

        private void InitializeDockUIObserver(DockProduct product, DockConfig config)
        {
            var dashboardInstance = Instantiate(config.dockBuildingDashboard, dashboardParentTransform);
            var observer = dashboardInstance.AddComponent<DockDashboardObserver>();
            observer.Initialize(product, dashboardParentTransform);
        }

        protected override void RegisterProduct(DockConfig config, DockProduct product)
        {
            productLookupTable.Add(config.dockStationID, product);
        }
    }
}
