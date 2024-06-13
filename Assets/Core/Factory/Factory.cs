using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Abstract base class for factories managing products.
    /// </summary>
    /// <typeparam name="TProduct">Type of product to be managed.</typeparam>
    /// <typeparam name="TConfig">Type of configuration for the product.</typeparam>
    public abstract class Factory<TProduct, TConfig> : MonoBehaviour
        where TProduct : MonoBehaviour where TConfig : ScriptableObject
    {
        /// <summary>
        /// Lookup table for products managed by the factory.
        /// </summary>
        public Dictionary<int, TProduct> productLookupTable = new Dictionary<int, TProduct>();

        /// <summary>
        /// Parent transform for instantiating products.
        /// </summary>
        [SerializeField] protected Transform instanceParent;

        /// <summary>
        /// Main camera for product initialization.
        /// </summary>
        [SerializeField] protected Camera mainCamera;

        /// <summary>
        /// Parent transform for dashboard UI.
        /// </summary>
        [SerializeField] protected Transform dashboardParentTransform;

        /// <summary>
        /// System log for logging events.
        /// </summary>
        public SystemLog systemLog { get; private set; }

        /// <summary>
        /// Initializes the factory with required dependencies.
        /// </summary>
        /// <param name="systemLog">System log for logging events.</param>
        public void Initialize(SystemLog systemLog)
        {
            this.systemLog = systemLog;
            InitializeFactory();
        }

        /// <summary>
        /// Virtual method to initialize the factory. Override in derived classes for custom initialization.
        /// </summary>
        protected virtual void InitializeFactory() { }

        /// <summary>
        /// Manufactures a product based on the provided configuration.
        /// </summary>
        /// <param name="config">Configuration for the product.</param>
        public virtual void ManufactureProduct(TConfig config)
        {
            var newInstance = CreateProductInstance(config);
            var product = InitializeProductComponent(newInstance, config);
            RegisterProduct(config, product);
        }

        /// <summary>
        /// Creates a new instance of the product.
        /// </summary>
        /// <param name="config">Configuration for the product.</param>
        /// <returns>Newly created GameObject instance.</returns>
        protected abstract GameObject CreateProductInstance(TConfig config);

        /// <summary>
        /// Initializes the product component of the created instance.
        /// </summary>
        /// <param name="instance">GameObject instance of the product.</param>
        /// <param name="config">Configuration for the product.</param>
        /// <returns>Initialized product component.</returns>
        protected abstract TProduct InitializeProductComponent(GameObject instance, TConfig config);

        /// <summary>
        /// Registers the product in the lookup table.
        /// </summary>
        /// <param name="config">Configuration for the product.</param>
        /// <param name="product">Product instance.</param>
        protected abstract void RegisterProduct(TConfig config, TProduct product);

        /// <summary>
        /// Deletes a product based on its ID.
        /// </summary>
        /// <param name="productID">ID of the product to delete.</param>
        public void DeleteProduct(int productID)
        {
            if (productLookupTable.TryGetValue(productID, out var product))
            {
                Destroy(product.gameObject);
                productLookupTable.Remove(productID);
                systemLog.LogEvent("Product deleted: " + product.name);
            }
            else
            {
                systemLog.LogEvent("Product not found: " + productID);
            }
        }

        /// <summary>
        /// Retrieves a product based on its ID.
        /// </summary>
        /// <param name="id">ID of the product to retrieve.</param>
        /// <returns>Product instance if found, null otherwise.</returns>
        public TProduct GetProduct(int id)
        {
            productLookupTable.TryGetValue(id, out var product);
            return product;
        }
    }
}
