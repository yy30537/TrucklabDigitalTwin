using UnityEngine;


namespace Core
{
    [CreateAssetMenu(fileName = "SpaceConfig", menuName = "SpaceConfig")]
    public class SpaceConfig: ScriptableObject
    {
        public int spaceID;
        public string spaceName;
        public GameObject spaceDashboard;
        public Vector3[] spacePoints;
        public Material spaceMaterialActive;
        public Material spaceMaterial;
        public VoidEventChannel ecToggleDashboard;
    }
}

