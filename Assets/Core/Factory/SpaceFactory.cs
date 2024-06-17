using UnityEngine;

namespace Core
{
    /// <summary>
    /// Factory class for creating and managing SpaceProduct instances.
    /// </summary>
    public class SpaceFactory : Factory<SpaceProduct, SpaceConfig>
    {
        /// <summary>
        /// Initializes the SpaceProduct component of the created instance.
        /// </summary>
        /// <param name="config">Configuration for the space product.</param>
        /// <returns>Initialized SpaceProduct component.</returns>
        public override IProduct GetProduct(SpaceConfig config)
        {
            var instance = new GameObject(config.spaceName);
            instance.transform.SetParent(instanceParent);
            instance.tag = "SpaceProduct";
            var product = instance.AddComponent<SpaceProduct>();
            
            product.productID = config.spaceID;
            product.productName = config.spaceName;
            product.productInstance = instance;
            product.mainCamera = mainCamera;
            product.systemLog = FindObjectOfType<SystemLog>();
            product.getClickedObject = FindObjectOfType<GetClickedObject>();
            product.spaceConfig = config;
            
            var dashboardInstance = Instantiate(config.spaceDashboard, dashboardParentTransform);
            var observer = dashboardInstance.AddComponent<SpaceDashboardObserver>();
            observer.Initialize(product, dashboardParentTransform);

            product.dashboardObserver = observer;
            product.vehicleFactory = vehicleFactory;
            
            product.Initialize();
            return product;
        }
        
        /// <summary>
        /// Registers the space product in the lookup table.
        /// </summary>
        /// <param name="config">Configuration for the space product.</param>
        /// <param name="product">Space product instance.</param>
        protected override void RegisterProduct(SpaceConfig config, SpaceProduct product)
        {
            productLookupTable.Add(config.spaceID, product);
        }
    }
}
