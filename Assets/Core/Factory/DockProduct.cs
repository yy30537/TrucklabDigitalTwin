using UnityEngine;

namespace Core
{
    public class DockProduct : Product
    {
        public DockConfig dockConfig;
        private DockDashboardObserver dashboardObserver;

        public void Init(DockConfig config, GameObject instance, Camera cam, SystemLog systemLog, GetClickedObject getClickedObject)
        {
            base.Init(config.dockStationID, config.dockStationName, instance, cam, systemLog, getClickedObject);
            dockConfig = config;
            InitComponents();
        }

        public override void InitComponents()
        {
            productInstance.transform.position = dockConfig.dockBuildingPosition;
            productInstance.transform.rotation = Quaternion.Euler(0, dockConfig.dockBuildingRotation, 0);
        }

        public void RegisterObserver(DockDashboardObserver observer)
        {
            dashboardObserver = observer;
        }

        public void RemoveObserver(DockDashboardObserver observer)
        {
            dashboardObserver = null;
        }

        private void NotifyUIObservers()
        {
            dashboardObserver.UpdateUI();
        }
    }
}
