using UnityEngine;

namespace Core
{
    /// <summary>
    /// Observer for Vehicle products.
    /// </summary>
    public class VehicleObserver : BaseObserver<VehicleProduct, VehicleUI>
    {
        private GetClickedObject clickObjectGetter;

        /// <summary>
        /// Initializes the vehicle observer with a product and its associated UI.
        /// </summary>
        /// <param name="product">The vehicle product to observe.</param>
        /// <param name="ui">The UI associated with the vehicle product.</param>
        public override void Initialize(VehicleProduct product, VehicleUI ui)
        {
            base.Initialize(product, ui);
            clickObjectGetter = Object.FindObjectOfType<GetClickedObject>();
            if (clickObjectGetter == null)
            {
                Product.SystemLogWindow.LogEvent("Error: GetClickedObject not found in the scene.");
            }
        }

        /// <summary>
        /// Determines if the vehicle observation should be toggled based on user interaction.
        /// </summary>
        /// <returns>True if the vehicle was clicked and its dashboard is not visible, false otherwise.</returns>
        protected override bool ShouldToggleObservation()
        {
            return !UI.IsDashboardVisible() && UI.DetectVehicleClick(clickObjectGetter);
        }

        /// <summary>
        /// Updates the vehicle UI.
        /// </summary>
        protected override void UpdateObservation()
        {
            UI.UpdateUI();
        }

        protected override void OnStartObserving()
        {
            base.OnStartObserving();
            Product.SystemLogWindow.LogEvent($"Started observing Vehicle: {Product.ProductName}");
        }

        protected override void OnStopObserving()
        {
            base.OnStopObserving();
            Product.SystemLogWindow.LogEvent($"Stopped observing Vehicle: {Product.ProductName}");
        }
    }
}