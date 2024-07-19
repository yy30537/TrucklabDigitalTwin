using System;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Factory for creating and managing vehicle products.
    /// </summary>
    public class VehicleFactory : BaseFactory<VehicleProduct, VehicleConfig>
    {
        /// <summary>
        /// Creates a new vehicle product based on the provided configuration.
        /// </summary>
        /// <param name="config">The configuration for the vehicle.</param>
        /// <returns>The created vehicle product.</returns>
        public override VehicleProduct GetProduct(VehicleConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            var instance = Instantiate(config.VehiclePrototypePrefab, InstanceParent);
            instance.name = config.VehicleName;

            var product = instance.AddComponent<VehicleProduct>();
            product.ProductID = config.VehicleId;
            product.ProductName = config.VehicleName;
            product.ProductInstance = instance;
            product.Config = config;

            InitializeProduct(product, config);
            product.Initialize();
            RegisterProduct(config, product);

            SystemLogWindow.LogEvent($"Created Vehicle: {product.ProductName}");
            return product;
        }

        /// <summary>
        /// Initializes the vehicle product with additional logging.
        /// </summary>
        /// <param name="product">The vehicle product to initialize.</param>
        /// <param name="config">The configuration for the vehicle.</param>
        protected override void InitializeProduct(VehicleProduct product, VehicleConfig config)
        {
            base.InitializeProduct(product, config);
            //SystemLogWindow.LogEvent($"Initialized Vehicle Components: {product.ProductName}");
        }

        /// <summary>
        /// Registers the vehicle product with additional logging.
        /// </summary>
        /// <param name="config">The configuration for the vehicle.</param>
        /// <param name="product">The vehicle product to register.</param>
        protected override void RegisterProduct(VehicleConfig config, VehicleProduct product)
        {
            base.RegisterProduct(config, product);
            //SystemLogWindow.LogEvent($"Registered Vehicle in VehicleFactory: {product.ProductName}");
        }
    }
}