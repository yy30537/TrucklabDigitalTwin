using UnityEngine;

namespace Core
{
    /// <summary>
    /// Observer for Region products.
    /// </summary>
    public class RegionObserver : BaseObserver<RegionProduct, RegionUI>
    {
        /// <summary>
        /// Determines if the region observation should be toggled.
        /// Always returns false as regions are always observed.
        /// </summary>
        /// <returns>Always returns false.</returns>
        protected override bool ShouldToggleObservation()
        {
            return false; // Regions are always observed
        }

        /// <summary>
        /// Updates the region UI.
        /// </summary>
        protected override void UpdateObservation()
        {
            UI.UpdateUI();
        }

        /// <summary>
        /// Starts observing the region and ensures the dashboard is always visible.
        /// </summary>
        public override void StartObserving()
        {
            base.StartObserving();
            UI.ShowDashboard(); // Ensure dashboard is always visible for regions
            Product.SystemLogWindow.LogEvent($"Started observing Region: {Product.ProductName}");
        }

        protected override void OnStopObserving()
        {
            // Regions should not stop observing, but we'll log it just in case
            Product.SystemLogWindow.LogEvent($"Warning: Attempted to stop observing Region: {Product.ProductName}");
        }
    }
}