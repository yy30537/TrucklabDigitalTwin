using UnityEngine;

namespace Core
{
    /// <summary>
    /// Abstract base class for all products in the simulation.
    /// Provides common properties and initialization logic.
    /// </summary>
    public abstract class BaseProduct : MonoBehaviour, IProduct
    {
        /// <summary>
        /// Gets or sets the unique identifier for the Product.
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// Gets or sets the name of the Product.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the GameObject instance of the Product.
        /// </summary>
        public GameObject ProductInstance { get; set; }

        /// <summary>
        /// Gets or sets the main camera used by the Product.
        /// </summary>
        public Camera MainCamera { get; set; }

        /// <summary>
        /// Gets or sets the system log window for logging events related to the Product.
        /// </summary>
        public SystemLogWindow SystemLogWindow { get; set; }

        /// <summary>
        /// Gets or sets the object click getter used for detecting user interactions with the Product.
        /// </summary>
        public GetClickedObject ObjectClickGetter { get; set; }

        /// <summary>
        /// Gets or sets the UI parent transform for this Product.
        /// </summary>
        public Transform UiParent { get; set; }

        /// <summary>
        /// Initializes the Product with its configuration and dependencies.
        /// </summary>
        public virtual void Initialize()
        {
            InitializeUI();
            InitializeObserver();
        }

        /// <summary>
        /// Initializes the UI components for the Product.
        /// </summary>
        protected abstract void InitializeUI();

        /// <summary>
        /// Initializes the observer for the Product.
        /// </summary>
        protected abstract void InitializeObserver();

        /// <summary>
        /// Performs cleanup when the Product is destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            // Common cleanup logic
        }
    }
}
