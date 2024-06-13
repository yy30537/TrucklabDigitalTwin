using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Factory class for creating and managing SpaceProduct instances.
    /// </summary>
    public class SpaceFactory : Factory<SpaceProduct, SpaceConfig>
    {
        private VehicleFactory vehicleFactory;
        private GetClickedObject getClickedObject;

        /// <summary>
        /// Initializes the SpaceFactory with required dependencies.
        /// </summary>
        /// <param name="systemLog">System log for logging events.</param>
        /// <param name="vehicleFactory">Factory for creating and managing vehicle products.</param>
        /// <param name="getClickedObject">Service for detecting clicked objects.</param>
        public void Initialize(SystemLog systemLog, VehicleFactory vehicleFactory, GetClickedObject getClickedObject)
        {
            this.vehicleFactory = vehicleFactory;
            this.getClickedObject = getClickedObject;
            base.Initialize(systemLog);
        }

        /// <summary>
        /// Creates a new instance of the space product.
        /// </summary>
        /// <param name="config">Configuration for the space product.</param>
        /// <returns>Newly created GameObject instance.</returns>
        protected override GameObject CreateProductInstance(SpaceConfig config)
        {
            var instance = new GameObject(config.spaceName);
            instance.transform.SetParent(instanceParent);
            instance.tag = "SpaceProduct";
            return instance;
        }

        /// <summary>
        /// Initializes the SpaceProduct component of the created instance.
        /// </summary>
        /// <param name="instance">GameObject instance of the space product.</param>
        /// <param name="config">Configuration for the space product.</param>
        /// <returns>Initialized SpaceProduct component.</returns>
        protected override SpaceProduct InitializeProductComponent(GameObject instance, SpaceConfig config)
        {
            var product = instance.AddComponent<SpaceProduct>();
            product.Init(config, instance, mainCamera, systemLog, getClickedObject, vehicleFactory);
            InitializeSpaceUI(product, config);
            return product;
        }

        /// <summary>
        /// Initializes the UI observer for the space product.
        /// </summary>
        /// <param name="product">Space product instance.</param>
        /// <param name="config">Configuration for the space product.</param>
        private void InitializeSpaceUI(SpaceProduct product, SpaceConfig config)
        {
            var dashboardInstance = Instantiate(config.spaceDashboard, dashboardParentTransform);
            var observer = dashboardInstance.AddComponent<SpaceDashboardObserver>();
            observer.Initialize(product, dashboardParentTransform);
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
