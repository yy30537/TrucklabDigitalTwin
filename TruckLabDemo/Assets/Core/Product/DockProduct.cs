using UnityEngine;

namespace Core
{
    /// <summary>
    /// Represents a dock product in the simulation.
    /// Manages the initialization and configuration of the dock.
    /// </summary>
    public class DockProduct : BaseProduct
    {
        /// <summary>
        /// Gets or sets the configuration for the dock.
        /// </summary>
        public DockConfig Config { get; set; }

        /// <summary>
        /// Gets or sets the vehicle factory associated with the dock.
        /// </summary>
        public VehicleFactory VehicleFactory { get; set; }

        /// <summary>
        /// Gets the UI component for the dock.
        /// </summary>
        public DockUI Ui { get; private set; }

        /// <summary>
        /// Gets the observer component for the dock.
        /// </summary>
        public DockObserver Observer { get; private set; }

        /// <summary>
        /// Initializes the dock product.
        /// </summary>
        public override void Initialize()
        {
            VehicleFactory = FindObjectOfType<VehicleFactory>();
            SetupDockPosition();
            base.Initialize();
            Observer.StartObserving();
            //SystemLogWindow.LogEvent($"Initialized Dock: {ProductName}");
        }

        /// <summary>
        /// Sets up the dock's position and rotation based on its configuration.
        /// </summary>
        private void SetupDockPosition()
        {
            ProductInstance.transform.position = Config.DockBuildingPosition;
            ProductInstance.transform.rotation = Quaternion.Euler(0, Config.DockBuildingRotation, 0);
        }

        /// <summary>
        /// Initializes the UI components for the dock.
        /// </summary>
        protected override void InitializeUI()
        {
            var uiInstance = Instantiate(Config.DockUiPrefab, UiParent);
            Ui = uiInstance.GetComponent<DockUI>();
            uiInstance.name = Config.DockStationName;
            if (Ui == null)
            {
                Ui = uiInstance.AddComponent<DockUI>();
            }
            Ui.Initialize(UiParent);
            Ui.SetDockData(this);
            //SystemLogWindow.LogEvent($"Initialized Dock UI for: {ProductName}");
        }

        /// <summary>
        /// Initializes the observer component for the dock.
        /// </summary>
        protected override void InitializeObserver()
        {
            Observer = gameObject.AddComponent<DockObserver>();
            Observer.Initialize(this, Ui);
        }

        /// <summary>
        /// Performs cleanup when the dock product is destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            //base.OnDestroy();
            //if (Observer != null)
            //{
            //    Observer.StopObserving();
            //    Destroy(Observer);
            //}
            //if (Ui != null)
            //{
            //    Destroy(Ui.gameObject);
            //}
            //SystemLogWindow.LogEvent($"Destroyed Dock: {ProductName}");
        }
    }
}
