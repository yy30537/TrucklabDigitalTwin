using UnityEngine;

namespace Core
{
    public class SpaceFactory : Factory<SpaceProduct, SpaceConfig>
    {
        protected override void InitializeFactory()
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
            product.Init(config, instance, mainCamera, productUIObserverParent, dashboardParentTransform);
            return product;
        }

        protected override void RegisterProduct(SpaceConfig config, SpaceProduct product)
        {
            productLookupTable.Add(config.spaceID, product);
        }
    }
}