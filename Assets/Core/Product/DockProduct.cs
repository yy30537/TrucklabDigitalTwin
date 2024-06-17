using UnityEngine;

namespace Core
{
    /// <summary>
    /// Represents a dock product in the simulation.
    /// </summary>
    public class DockProduct : MonoBehaviour, IProduct
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public GameObject productInstance { get; set; }
        public Camera mainCamera { get; set; }
        public SystemLog systemLog { get; set; }
        public GetClickedObject getClickedObject { get; set; }
        public DockConfig dockConfig { get; set; }
        
        public DockDashboardObserver dashboardObserver { get; set; }
        
        public void Initialize()
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
