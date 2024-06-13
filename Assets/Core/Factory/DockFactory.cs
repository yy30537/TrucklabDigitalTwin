using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Factory class for creating and managing DockProduct instances.
    /// </summary>
    public class DockFactory : Factory<DockProduct, DockConfig>
    {
        private GetClickedObject getClickedObject;

        /// <summary>
        /// Initializes the DockFactory with required dependencies.
        /// </summary>
        /// <param name="systemLog">System log for logging events.</param>
        /// <param name="getClickedObject">Service for detecting clicked objects.</param>
        public void Initialize(SystemLog systemLog, GetClickedObject getClickedObject)
        {
            this.getClickedObject = getClickedObject;
            base.Initialize(systemLog);
        }

        /// <summary>
        /// Creates a new instance of the dock product.
        /// </summary>
        /// <param name="config">Configuration for the dock product.</param>
        /// <returns>Newly created GameObject instance.</returns>
        protected override GameObject CreateProductInstance(DockConfig config)
        {
            var instance = Instantiate(config.dockBuildingPrefab, instanceParent);
            instance.name = config.dockStationName;
            return instance;
        }

        /// <summary>
        /// Initializes the DockProduct component of the created instance.
        /// </summary>
        /// <param name="instance">GameObject instance of the dock product.</param>
        /// <param name="config">Configuration for the dock product.</param>
        /// <returns>Initialized DockProduct component.</returns>
        protected override DockProduct InitializeProductComponent(GameObject instance, DockConfig config)
        {
            var product = instance.AddComponent<DockProduct>();
            product.Init(config, instance, mainCamera, systemLog, getClickedObject);
            InitializeDockUIObserver(product, config);
            return product;
        }

        /// <summary>
        /// Initializes the UI observer for the dock product.
        /// </summary>
        /// <param name="product">Dock product instance.</param>
        /// <param name="config">Configuration for the dock product.</param>
        private void InitializeDockUIObserver(DockProduct product, DockConfig config)
        {
            var dashboardInstance = Instantiate(config.dockBuildingDashboard, dashboardParentTransform);
            var observer = dashboardInstance.AddComponent<DockDashboardObserver>();
            observer.Initialize(product, dashboardParentTransform);
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
