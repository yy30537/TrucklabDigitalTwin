using UnityEngine;

namespace Core
{
    public class DockProduct : Product
    {
        public DockConfig dockConfig;
        private DockDashboard dashboard;

        public void Init(DockConfig config, GameObject instance, Camera cam, GameObject uiObserverParent, Transform dashboardParent)
        {
            base.Init(config.dockStationID, config.dockStationName, instance, cam, uiObserverParent, dashboardParent);
            dockConfig = config;
        }

        protected override void InitComponents(GameObject uiObserverParent, Transform dashboardParent)
        {
            productInstance.transform.position = dockConfig.dockBuildingPosition;
            productInstance.transform.rotation = Quaternion.Euler(0, dockConfig.dockBuildingRotation, 0);

            InitializeDockUIObserver(uiObserverParent, dashboardParent);
        }

        private void InitializeDockUIObserver(GameObject uiObserverParent, Transform dashboardParent)
        {
            var uiObserverInstance = new GameObject("DockDashboard");
            uiObserverInstance.transform.SetParent(uiObserverParent.transform);
            var uiObserver = uiObserverInstance.AddComponent<DockDashboard>();
            uiObserver.Initialize(this, dashboardParent);
            RegisterObserver(uiObserver);
        }

        public void RegisterObserver(DockDashboard observer)
        {
            dashboard = observer;
        }

        public void RemoveObserver(DockDashboard observer)
        {
            dashboard = null;
        }

        private void NotifyUIObservers()
        {
            dashboard?.UpdateUI();
        }
    }
}