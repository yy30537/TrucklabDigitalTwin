using System.Collections.Generic;
using RosSharp.RosBridgeClient;
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
        public Dictionary<int, TProduct> productLookupTable = new Dictionary<int, TProduct>();
        [SerializeField] protected Transform instanceParent;
        [SerializeField] protected Camera mainCamera;
        [SerializeField] protected Transform dashboardParentTransform;
        [SerializeField] protected VehicleFactory vehicleFactory;
        [SerializeField] protected SystemLog systemLog;
        
        /// <summary>
        /// Manufactures a product based on the provided configuration.
        /// </summary>
        /// <param name="config">Configuration for the product.</param>
        public abstract IProduct GetProduct(TConfig config);

        /// <summary>
        /// Registers the product in the lookup table.
        /// </summary>
        /// <param name="config">Configuration for the product.</param>
        /// <param name="product">IProduct instance.</param>
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
                systemLog.LogEvent("IProduct deleted: " + product.name);
            }
            else
            {
                systemLog.LogEvent("IProduct not found: " + productID);
            }
        }

        /// <summary>
        /// Retrieves a product based on its ID.
        /// </summary>
        /// <param name="id">ID of the product to retrieve.</param>
        /// <returns>IProduct instance if found, null otherwise.</returns>
        public TProduct LookupProduct(int id)
        {
            productLookupTable.TryGetValue(id, out var product);
            return product;
        }
    }
}
