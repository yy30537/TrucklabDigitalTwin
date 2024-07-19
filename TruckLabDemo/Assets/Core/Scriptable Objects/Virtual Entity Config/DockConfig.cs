using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "DockConfig", menuName = "DockConfig")]
    public class DockConfig : BaseEntityConfig
    {
        public GameObject DockBuildingPrefab;
        public Vector3 DockBuildingPosition;
        public float DockBuildingRotation;

        // Overriding base class properties to maintain existing naming
        public int DockStationId { get => base.EntityId; set => base.EntityId = value; }
        public string DockStationName { get => base.EntityName; set => base.EntityName = value; }
        public GameObject DockUiPrefab { get => base.UiPrefab; set => base.UiPrefab = value; }
    }
}