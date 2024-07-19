using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    /// <summary>
    /// UI component for displaying and managing DC docks information.
    /// </summary>
    public class DockUI : BaseUI
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Button toggleButton;

        // TODO: once scheduling and path planning services are implemented from the backend, create more UI elements for updating the DC schedule 

        private DockProduct dockProduct;
        private GetClickedObject getClickedObject;

        public override void Initialize(Transform parent)
        {
            base.Initialize(parent);

            if (title == null) title = UiInstance.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
            SetupButtons();
            HideDashboard();
        }

        /// <summary>
        /// Sets the dock data for this UI component.
        /// </summary>
        /// <param name="product">The DockProduct to associate with this UI.</param>
        public void SetDockData(DockProduct product)
        {
            dockProduct = product ?? throw new System.ArgumentNullException(nameof(product));
            getClickedObject = FindObjectOfType<GetClickedObject>();
        }

        /// <summary>
        /// Sets up the buttons for the dock UI.
        /// </summary>
        private void SetupButtons()
        {
            if (toggleButton == null)
            {
                var canvas = FindObjectOfType<Canvas>();
                toggleButton = canvas?.transform.Find("Explorer/Toggle Schedule")?.GetComponent<Button>();
            }
            toggleButton?.onClick.AddListener(ToggleDashboard);

            if (toggleButton == null)
            {
                SystemLogWindow.LogEvent($"Toggle Schedule button not found for {dockProduct?.ProductName ?? "Unknown Dock"}");
            }
        }

        public override void UpdateUI()
        {
            if (!IsDashboardVisible() || dockProduct == null) return;

            //if (dockProduct.VehicleFactory.ProductLookupTable.Any())
            //{
            //    title.text = "DC Hosting: \n";
            //    foreach (var vehicleProduct in dockProduct.VehicleFactory.ProductLookupTable)
            //    {
            //        title.text += $"[{vehicleProduct.Value.ProductName}] \n";
            //    }
            //}
        }

        /// <summary>
        /// Detects if the dock has been clicked.
        /// </summary>
        /// <returns>True if the dock was clicked, false otherwise.</returns>
        public bool DetectDockClick()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    GameObject hitObject = getClickedObject.ReturnClickedObject();
            //    if (hitObject != null && hitObject.CompareTag("DC"))
            //    {
            //        DockProduct clickedDock = hitObject.GetComponentInParent<DockProduct>();
            //        return clickedDock != null && clickedDock.ProductID == dockProduct.ProductID;
            //    }
            //}
            return false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            toggleButton?.onClick.RemoveListener(ToggleDashboard);
        }
    }
}
