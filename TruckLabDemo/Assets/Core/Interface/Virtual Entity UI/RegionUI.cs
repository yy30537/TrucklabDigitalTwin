using TMPro;
using UnityEngine;
using System.Linq;

namespace Core
{
    /// <summary>
    /// UI component for displaying and managing region information.
    /// </summary>
    public class RegionUI : BaseUI
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI content;

        private RegionProduct regionProduct;

        public override void Initialize(Transform parent)
        {
            base.Initialize(parent);

            if (title == null) title = UiInstance.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
            if (content == null) content = UiInstance.transform.Find("Content")?.GetComponent<TextMeshProUGUI>();

            if (title == null || content == null)
            {
                SystemLogWindow.LogEvent($"Failed to find UI components for {GetType().Name}");
            }

            HideDashboard();
        }

        /// <summary>
        /// Sets the region data for this UI component.
        /// </summary>
        /// <param name="product">The RegionProduct to associate with this UI.</param>
        public void SetRegionData(RegionProduct product)
        {
            regionProduct = product ?? throw new System.ArgumentNullException(nameof(product));
        }

        public override void UpdateUI()
        {
            if (regionProduct == null)
            {
                SystemLogWindow.LogEvent("Attempted to update UI with null RegionProduct");
                return;
            }

            title.text = $"{regionProduct.ProductName}\n";
            content.text = GetVehicleListText();
        }

        /// <summary>
        /// Generates a text representation of the vehicles in the region.
        /// </summary>
        /// <returns>A string containing the list of vehicles in the region.</returns>
        private string GetVehicleListText()
        {
            if (regionProduct.VehiclesInRegion.Count == 0)
            {
                content.color = Color.black;
                return "No vehicles detected";
            }
            content.color = Color.red;
            return "Detecting:\n" + string.Join("\n",
                regionProduct.VehicleFactory.ProductLookupTable
                    .Where(kvp => regionProduct.VehiclesInRegion.ContainsKey(kvp.Key))
                    .Select(kvp => kvp.Value.ProductName));
        }
    }
}