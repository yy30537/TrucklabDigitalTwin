using System;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Factory for creating and managing dock products.
    /// </summary>
    public class DockFactory : BaseFactory<DockProduct, DockConfig>
    {
        /// <summary>
        /// Creates a new dock product based on the provided configuration.
        /// </summary>
        /// <param name="config">The configuration for the dock.</param>
        /// <returns>The created dock product.</returns>
        public override DockProduct GetProduct(DockConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            var instance = Instantiate(config.DockBuildingPrefab, InstanceParent);
            instance.name = config.DockStationName;
            instance.tag = "DC";

            var product = instance.AddComponent<DockProduct>();
            product.ProductID = config.DockStationId;
            product.ProductName = config.DockStationName;
            product.ProductInstance = instance;
            product.Config = config;

            InitializeProduct(product, config);
            product.Initialize();
            RegisterProduct(config, product);

            SystemLogWindow.LogEvent($"Created Dock: {product.ProductName}");
            return product;
        }
    }
}