using UnityEngine;

namespace Core
{
    /// <summary>
    /// Factory class for creating and managing DockProduct instances.
    /// </summary>
    public class DockFactory : Factory<DockProduct, DockConfig>
    {
        /// <summary>
        /// Initializes the DockProduct component of the created instance.
        /// </summary>
        /// <param name="config">Configuration for the dock product.</param>
        /// <returns>Initialized DockProduct component.</returns>
        public override IProduct GetProduct(DockConfig config)
        {
            var instance = Instantiate(config.dockBuildingPrefab, instanceParent);
            instance.name = config.dockStationName;
            var product = instance.AddComponent<DockProduct>();

            product.productID = config.dockStationID;
            product.productName = config.dockStationName;
            product.productInstance = instance;
            product.mainCamera = mainCamera;
            product.systemLog = FindObjectOfType<SystemLog>();
            product.getClickedObject = FindObjectOfType<GetClickedObject>();
            product.dockConfig = config;
            
            var dashboardInstance = Instantiate(config.dockBuildingDashboard, dashboardParentTransform);
            var observer = dashboardInstance.AddComponent<DockDashboardObserver>();
            observer.Initialize(product, dashboardParentTransform);

            product.dashboardObserver = observer;
            
            product.Initialize();
            return product;
        }
        
        /// <summary>
        /// Registers the dock product in the lookup table.
        /// </summary>
        /// <param name="config">Configuration for the dock product.</param>
        /// <param name="product">Dock product instance.</param>
        protected override void RegisterProduct(DockConfig config, DockProduct product)
        {
            productLookupTable.Add(config.dockStationID, product);
        }
    }
}
