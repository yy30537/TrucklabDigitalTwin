using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Base class for all factories in the application.
    /// This abstract class provides the basic structure and functionalities that all products in the simulation share.
    /// It implements the IProduct interface, ensuring that all products have the necessary properties and methods.
    /// The Initialize method is meant to set up the product's UI and observer, which are defined as abstract methods to be implemented by derived classes.
    /// The OnDestroy method is also provided for cleanup purposes. 
    /// </summary>
    /// <typeparam name="TProduct">The type of product this factory creates.</typeparam>
    /// <typeparam name="TConfig">The configuration type for the product.</typeparam>
    public abstract class BaseFactory<TProduct, TConfig> : MonoBehaviour
        where TProduct : BaseProduct
        where TConfig : ScriptableObject
    {
        /// <summary>
        /// Lookup table for all products created by this factory.
        /// </summary>
        public Dictionary<int, TProduct> ProductLookupTable { get; } = new Dictionary<int, TProduct>();

        [SerializeField] protected Transform InstanceParent;
        [SerializeField] protected Transform UiParent;
        [SerializeField] protected Camera MainCamera;
        [SerializeField] protected SystemLogWindow SystemLogWindow;
        [SerializeField] protected GetClickedObject ObjectClickGetter;
        [SerializeField] public VoidEventChannel ProductInitializedEvent;
        [SerializeField] public VoidEventChannel ProductDeletedEvent;

        /// <summary>
        /// Creates and returns a new product based on the provided configuration.
        /// </summary>
        /// <param name="config">The configuration for the product.</param>
        /// <returns>The created product.</returns>
        public abstract TProduct GetProduct(TConfig config);

        /// <summary>
        /// Initializes the common properties of a product.
        /// </summary>
        /// <param name="product">The product to initialize.</param>
        /// <param name="config">The configuration for the product.</param>
        protected virtual void InitializeProduct(TProduct product, TConfig config)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            product.MainCamera = MainCamera;
            product.SystemLogWindow = SystemLogWindow;
            product.ObjectClickGetter = ObjectClickGetter;
            product.UiParent = UiParent;
        }

        /// <summary>
        /// Registers a product in the lookup table and raises the initialization event.
        /// </summary>
        /// <param name="config">The configuration for the product.</param>
        /// <param name="product">The product to register.</param>
        protected virtual void RegisterProduct(TConfig config, TProduct product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            ProductLookupTable[product.ProductID] = product;
            //SystemLogWindow.LogEvent($"Registered {typeof(TProduct).Name}: {product.ProductName}");
            ProductInitializedEvent.RaiseEvent();
        }

        /// <summary>
        /// Deletes a product from the factory and raises the deletion event.
        /// </summary>
        /// <param name="productId">The ID of the product to delete.</param>
        public virtual void DeleteProduct(int productId)
        {
            if (ProductLookupTable.TryGetValue(productId, out var product))
            {
                Destroy(product.gameObject);
                ProductLookupTable.Remove(productId);
                SystemLogWindow.LogEvent($"Deleted {typeof(TProduct).Name}: {product.ProductName}");
                ProductDeletedEvent.RaiseEvent();
            }
            else
            {
                SystemLogWindow.LogEvent($"{typeof(TProduct).Name} not found: {productId}");
            }
        }

        /// <summary>
        /// Retrieves a product from the lookup table by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>The product if found, null otherwise.</returns>
        public TProduct LookupProduct(int id)
        {
            ProductLookupTable.TryGetValue(id, out var product);
            return product;
        }
    }
}