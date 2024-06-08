using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class DockFactory : Factory<DockProduct, DockConfig>
    {

        private void Awake()
        {
            productUIObserverParent = new GameObject("DockUIObservers");
            productUIObserverParent.transform.SetParent(instanceParent);
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
            product.Init(config, instance, mainCamera);
            InitializeDockUIObserver(product);
            return product;
        }

        private void InitializeDockUIObserver(DockProduct product)
        {
            var uiObserverInstance = new GameObject("DockDashboard");
            uiObserverInstance.transform.SetParent(uiObserverParent.transform);
            var uiObserver = uiObserverInstance.AddComponent<DockDashboard>();
            uiObserver.Initialize(product, dashboardParentTransform);
        }

        protected override void RegisterProduct(DockConfig config, DockProduct product)
        {
            productLookupTable.Add(config.dockStationID, product);
        }
    }
}
