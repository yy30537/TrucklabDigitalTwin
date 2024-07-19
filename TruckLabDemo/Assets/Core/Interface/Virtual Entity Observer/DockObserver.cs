using UnityEngine;

namespace Core
{
    /// <summary>
    /// Observer for Dock products.
    /// </summary>
    public class DockObserver : BaseObserver<DockProduct, DockUI>
    {
        /// <summary>
        /// Determines if the dock observation should be toggled based on user interaction.
        /// </summary>
        /// <returns>True if the dock was clicked, false otherwise.</returns>
        protected override bool ShouldToggleObservation()
        {
            return UI.DetectDockClick();
        }

        /// <summary>
        /// Updates the dock UI.
        /// </summary>
        protected override void UpdateObservation()
        {
            UI.UpdateUI();
        }

        protected override void OnStartObserving()
        {
            base.OnStartObserving();
            Product.SystemLogWindow.LogEvent($"Started observing Dock: {Product.ProductName}");
        }

        protected override void OnStopObserving()
        {
            base.OnStopObserving();
            Product.SystemLogWindow.LogEvent($"Stopped observing Dock: {Product.ProductName}");
        }
    }
}