using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Core
{
    [CreateAssetMenu(fileName = "DockConfig", menuName = "DockConfig")]
    public class DockConfig : ScriptableObject
    {
        public int dockStationID;
        public string dockStationName;
        public GameObject dockBuildingPrefab;
        public Vector3 dockBuildingPosition;
        public float dockBuildingRotation;
        
        public GameObject dockBuildingDashboard;
        
        public VoidEventChannel ecToggleDashboard;
    }
}

