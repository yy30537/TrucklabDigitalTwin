using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class DockProduct : Product
    {
        public DockConfig dockConfig;
        private DockDashboard dashboard;

        public void Init(DockConfig config, GameObject instance, Camera cam)
        {
            base.Init(config.dockStationID, config.dockStationName, instance, cam);
            dockConfig = config;
            InitComponents();
        }

        public override void InitComponents()
        {
            productInstance.transform.position = dockConfig.dockBuildingPosition;
            productInstance.transform.rotation = Quaternion.Euler(0, dockConfig.dockBuildingRotation, 0);
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
            dashboard.UpdateUI();
        }
        
    }
}
