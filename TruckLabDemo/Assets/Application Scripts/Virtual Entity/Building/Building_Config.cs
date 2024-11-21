﻿using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Building
{
    [CreateAssetMenu(fileName = "Building_Config", menuName = "VE_Config/Building_Config")]
    public class Building_Config : VE_Config
    {
        [Header("Prototype")]
        public GameObject BuildingPrefab;

        [Header("Data")]
        public Vector3 BuildingPosition;
        public float BuildingOrientation;
    }
}