using UnityEngine;

namespace Core
{
    public interface ISpaceProduct
    {
        void Init(SpaceConfig config, GameObject instance, Camera cam, SystemLog systemLog, GetClickedObject getClickedObject, VehicleFactory vehicleFactory);
        bool IsVehicleProductInSpace(int id);
        void RegisterObserver(SpaceDashboardObserver observer);
        void RemoveObserver(SpaceDashboardObserver observer);
    }
}
