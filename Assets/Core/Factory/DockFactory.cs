using UnityEngine;

namespace Core
{
    public class DockFactory : Factory<DockProduct, DockConfig>
    {
        protected override void InitializeFactory()
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
            product.Init(config, instance, mainCamera, productUIObserverParent, dashboardParentTransform);
            return product;
        }

        protected override void RegisterProduct(DockConfig config, DockProduct product)
        {
            productLookupTable.Add(config.dockStationID, product);
        }
    }
}