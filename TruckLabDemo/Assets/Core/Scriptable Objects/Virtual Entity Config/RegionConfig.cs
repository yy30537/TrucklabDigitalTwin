using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "RegionConfig", menuName = "RegionConfig")]
    public class RegionConfig : BaseEntityConfig
    {
        public Vector3[] RegionPoints;
        public Material RegionMaterial;

        public float x,  y, psi1Rad, psi2Rad;

        // Overriding base class properties to maintain existing naming
        public int RegionId { get => base.EntityId; set => base.EntityId = value; }
        public string RegionName { get => base.EntityName; set => base.EntityName = value; }
        public GameObject RegionUiPrefab { get => base.UiPrefab; set => base.UiPrefab = value; }
    }
}