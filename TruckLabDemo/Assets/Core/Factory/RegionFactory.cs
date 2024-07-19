using System;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Factory for creating and managing region products.
    /// </summary>
    public class RegionFactory : BaseFactory<RegionProduct, RegionConfig>
    {
        /// <summary>
        /// Creates a new region product based on the provided configuration.
        /// </summary>
        /// <param name="config">The configuration for the region.</param>
        /// <returns>The created region product.</returns>
        public override RegionProduct GetProduct(RegionConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            var instance = new GameObject(config.RegionName);
            instance.transform.SetParent(InstanceParent);
            instance.tag = "RegionProduct";

            var product = instance.AddComponent<RegionProduct>();
            product.ProductID = config.RegionId;
            product.ProductName = config.RegionName;
            product.ProductInstance = instance;
            product.Config = config;

            InitializeProduct(product, config);
            product.Initialize();
            RegisterProduct(config, product);

            SystemLogWindow.LogEvent($"Created Region: {product.ProductName}");
            return product;
        }
    }
}