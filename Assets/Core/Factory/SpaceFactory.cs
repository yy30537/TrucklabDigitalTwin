using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SpaceFactory : Factory<SpaceProduct, SpaceConfig>
    {
        private void Awake()
        {
            productUIObserverParent = new GameObject("SpaceUIObservers");
            productUIObserverParent.transform.SetParent(uiObserverParent);
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
            product.Init(config, instance, mainCamera);
            InitializeSpaceUIObserver(product);
            return product;
        }

        private void InitializeSpaceUIObserver(SpaceProduct product)
        {
            var uiObserverInstance = new GameObject("SpaceDashboard");
            uiObserverInstance.transform.SetParent(productUIObserverParent.transform);
            var uiObserver = uiObserverInstance.AddComponent<SpaceDashboard>();
            uiObserver.Initialize(product, dashboardParentTransform);
        }

        protected override void RegisterProduct(SpaceConfig config, SpaceProduct product)
        {
            productLookupTable.Add(config.spaceID, product);
        }
    }
}

