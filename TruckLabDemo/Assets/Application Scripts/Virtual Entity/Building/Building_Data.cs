using System.Collections.Generic;
using Application_Scripts.Virtual_Entity.Vehicle;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Building
{
    /// <summary>
    /// Holds runtime data and configuration for a building entity.
    /// </summary>
    public class Building_Data : MonoBehaviour
    {
        /// <summary>
        /// Configuration settings for the building.
        /// </summary>
        [Header("VE Data")]
        public Building_Config Config;

        /// <summary>
        /// A dictionary mapping vehicle IDs to their corresponding vehicle objects within the building.
        /// </summary>
        public Dictionary<int, VE_Vehicle> Vehicles;

        /// <summary>
        /// Initializes the building data with the provided configuration.
        /// </summary>
        /// <param name="config">The configuration settings for the building.</param>
        public void Init(Building_Config config)
        {
            // Assign the configuration to the building data.
            Config = config;

            // Initialize the dictionary to store vehicles present in the building.
            Vehicles = new Dictionary<int, VE_Vehicle>();
        }
    }
}