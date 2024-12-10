using UnityEngine;

namespace ApplicationScripts.VirtualEntity.Space
{
    [CreateAssetMenu(fileName = "Space_Config", menuName = "VE_Config/Space_Config")]
    public class Space_Config : VE_Config
    {
        [Header("Vertices (Calibration Data)")]
        public Vector3[] SpaceMarkings;

        [Header("Ground Materials")]
        public Material GroundMaterial;
        public Material GroundMaterialActive;

        [Header("Default Vehicle Position")]
        public float X1,  Y1, Psi1Rad, Psi2Rad;

        [Header("Default Space Label Position")]
        public float LabelX, LabelY, LabelPsi;
    }
}