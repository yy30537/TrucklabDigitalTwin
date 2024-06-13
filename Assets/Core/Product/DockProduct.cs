using UnityEngine;

namespace Core
{
    /// <summary>
    /// Represents a dock product in the simulation.
    /// </summary>
    public class DockProduct : Product
    {
        public DockConfig dockConfig { get; private set; }
        private DockDashboardObserver dashboardObserver;

        /// <summary>
        /// Initializes the dock product with the provided configuration and dependencies.
        /// </summary>
        /// <param name="config">Dock configuration.</param>
        /// <param name="instance">GameObject instance of the dock.</param>
        /// <param name="cam">Main camera.</param>
        /// <param name="systemLog">System log for logging events.</param>
        /// <param name="getClickedObject">Service for detecting clicked objects.</param>
        public void Init(DockConfig config, GameObject instance, Camera cam, SystemLog systemLog, GetClickedObject getClickedObject)
        {
            base.Init(config.dockStationID, config.dockStationName, instance, cam, systemLog, getClickedObject);
            dockConfig = config;
            InitComponents();
        }

        /// <summary>
        /// Initializes the components of the dock product.
        /// </summary>
        public override void InitComponents()
        {
            productInstance.transform.position = dockConfig.dockBuildingPosition;
            productInstance.transform.rotation = Quaternion.Euler(0, dockConfig.dockBuildingRotation, 0);
        }

        /// <summary>
        /// Registers an observer for the dock product.
        /// </summary>
        /// <param name="observer">Observer to register.</param>
        public void RegisterObserver(DockDashboardObserver observer)
        {
            dashboardObserver = observer;
        }

        /// <summary>
        /// Removes an observer from the dock product.
        /// </summary>
        /// <param name="observer">Observer to remove.</param>
        public void RemoveObserver(DockDashboardObserver observer)
        {
            dashboardObserver = null;
        }

        /// <summary>
        /// Notifies UI observers of changes.
        /// </summary>
        private void NotifyUIObservers()
        {
            dashboardObserver.UpdateUI();
        }
    }
}
