using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public abstract class Factory<TProduct, TConfig> : MonoBehaviour
        where TProduct : MonoBehaviour where TConfig : ScriptableObject
    {
        public Dictionary<int, TProduct> productLookupTable = new Dictionary<int, TProduct>();
        public Transform instanceParent;
        public Camera mainCamera;
        public Transform uiObserverParent;
        public GameObject productUIObserverParent;
        public Transform dashboardParentTransform;
        public SystemLog systemLog { get; private set; }

        private void Awake()
        {
            systemLog = FindObjectOfType<SystemLog>();
            InitializeFactory();
        }

        protected virtual void InitializeFactory() { }

        public virtual void ManufactureProduct(TConfig config)
        {
            var newInstance = CreateProductInstance(config);
            var product = InitializeProductComponent(newInstance, config);
            RegisterProduct(config, product);
        }

        protected abstract GameObject CreateProductInstance(TConfig config);
        protected abstract TProduct InitializeProductComponent(GameObject instance, TConfig config);
        protected abstract void RegisterProduct(TConfig config, TProduct product);

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

        public TProduct GetProduct(int id)
        {
            productLookupTable.TryGetValue(id, out var product);
            return product;
        }
    }
}