using UnityEngine;

namespace Core
{
    /// <summary>
    /// Base class for all observers in the application.
    /// </summary>
    /// <typeparam name="T">The type of product being observed, must implement IProduct.</typeparam>
    /// <typeparam name="U">The type of UI associated with the product, must inherit from BaseUI.</typeparam>
    public abstract class BaseObserver<T, U> : MonoBehaviour where T : IProduct where U : BaseUI
    {
        /// <summary>
        /// The product being observed.
        /// </summary>
        protected T Product { get; private set; }

        /// <summary>
        /// The UI component associated with the observed product.
        /// </summary>
        protected U UI { get; private set; }

        /// <summary>
        /// Gets or sets whether the observer is currently observing.
        /// </summary>
        public bool IsObserving { get; protected set; }

        /// <summary>
        /// Initializes the observer with the product and UI it will monitor.
        /// </summary>
        /// <param name="product">The product to observe.</param>
        /// <param name="ui">The UI component associated with the product.</param>
        public virtual void Initialize(T product, U ui)
        {
            Product = product ?? throw new System.ArgumentNullException(nameof(product));
            UI = ui ?? throw new System.ArgumentNullException(nameof(ui));
            IsObserving = false;
        }

        protected virtual void Update()
        {
            if (ShouldToggleObservation())
            {
                ToggleObservation();
            }
            if (IsObserving)
            {
                UpdateObservation();
            }
        }

        /// <summary>
        /// Determines whether the observation state should be toggled.
        /// </summary>
        /// <returns>True if observation should be toggled, false otherwise.</returns>
        protected abstract bool ShouldToggleObservation();

        /// <summary>
        /// Updates the observation, typically by updating the UI.
        /// </summary>
        protected abstract void UpdateObservation();

        /// <summary>
        /// Starts observing the product.
        /// </summary>
        public virtual void StartObserving()
        {
            IsObserving = true;
            UI.ShowDashboard();
            OnStartObserving();
        }

        /// <summary>
        /// Stops observing the product.
        /// </summary>
        public virtual void StopObserving()
        {
            IsObserving = false;
            UI.HideDashboard();
            OnStopObserving();
        }

        /// <summary>
        /// Called when observation starts. Can be overridden in derived classes.
        /// </summary>
        protected virtual void OnStartObserving() { }

        /// <summary>
        /// Called when observation stops. Can be overridden in derived classes.
        /// </summary>
        protected virtual void OnStopObserving() { }

        /// <summary>
        /// Toggles the observation state.
        /// </summary>
        protected void ToggleObservation()
        {
            if (IsObserving)
            {
                StopObserving();
            }
            else
            {
                StartObserving();
            }
        }
    }
}