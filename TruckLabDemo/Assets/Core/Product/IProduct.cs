using UnityEngine;

namespace Core
{
    /// <summary>
    /// Interface for virtual entity products in the simulation.
    /// 
    /// This interface defines the contract for any product in the simulation.
    /// It ensures that all products have an
    /// ID, a name, a GameObject instance, a main camera, a system log window, and an object click getter.
    /// It also requires an Initialize method for setting up the product.
    /// </summary>
    public interface IProduct
    {
        /// <summary>
        /// Gets or sets the unique identifier for the Product.
        /// </summary>
        int ProductID { get; set; }

        /// <summary>
        /// Gets or sets the name of the Product.
        /// </summary>
        string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the GameObject instance of the Product.
        /// </summary>
        GameObject ProductInstance { get; set; }

        /// <summary>
        /// Gets or sets the main camera used by the Product.
        /// </summary>
        Camera MainCamera { get; set; }

        /// <summary>
        /// Gets or sets the system log window for logging events related to the Product.
        /// </summary>
        SystemLogWindow SystemLogWindow { get; set; }

        /// <summary>
        /// Gets or sets the object click getter used for detecting user interactions with the Product.
        /// </summary>
        GetClickedObject ObjectClickGetter { get; set; }

        /// <summary>
        /// Initializes the Product with its configuration and dependencies.
        /// </summary>
        void Initialize();
    }
}