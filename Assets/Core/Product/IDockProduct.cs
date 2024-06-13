using UnityEngine;

namespace Core
{
    public interface IDockProduct
    {
        void Init(DockConfig config, GameObject instance, Camera cam, SystemLog systemLog, GetClickedObject getClickedObject);
        void RegisterObserver(DockDashboardObserver observer);
        void RemoveObserver(DockDashboardObserver observer);
    }
}
