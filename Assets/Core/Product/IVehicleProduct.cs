using RosSharp.RosBridgeClient;
using UnityEngine;

namespace Core
{
    public interface IVehicleProduct
    {
        void Init(VehicleConfig config, GameObject instance, Camera cam, Transform dashboardParent, SystemLog systemLog, GetClickedObject getClickedObject, SetSimulationServiceProvider simulationServiceProvider);
        void SetSimulationDetail(int vehicleID, int pathID);
    }
}
