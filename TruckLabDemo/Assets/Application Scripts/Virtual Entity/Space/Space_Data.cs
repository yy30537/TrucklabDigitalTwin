using System.Collections.Generic;
using Application_Scripts.Virtual_Entity.Vehicle;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Space
{
    /// <summary>
    /// Holds runtime data and configuration for a space entity, including vehicles within the space and default positions.
    /// </summary>
    public class Space_Data : MonoBehaviour
    {
        /// <summary>
        /// Configuration settings for the space entity.
        /// </summary>
        [Header("VE Data")]
        public Space_Config Config;

        /// <summary>
        /// A dictionary mapping vehicle IDs to the vehicles currently present in the space.
        /// </summary>
        public Dictionary<int, VE_Vehicle> VehiclesPresentInSpace;

        /// <summary>
        /// Default X position for vehicles in the space.
        /// </summary>
        [Header("Default Vehicle Position")]
        public float X1;

        /// <summary>
        /// Default Y position for vehicles in the space.
        /// </summary>
        public float Y1;

        /// <summary>
        /// Default orientation angle (in radians) for the tractor in the space.
        /// </summary>
        public float Psi1Rad;

        /// <summary>
        /// Default orientation angle (in radians) for the trailer in the space.
        /// </summary>
        public float Psi2Rad;

        /// <summary>
        /// Initializes the space data using the provided configuration.
        /// </summary>
        /// <param name="config">The configuration settings for the space.</param>
        public void Init(Space_Config config)
        {
            // Assign the provided configuration to the space data.
            Config = config;

            // Set default positions and orientations based on the configuration.
            X1 = Config.X1;
            Y1 = Config.Y1;
            Psi1Rad = Config.Psi1Rad;
            Psi2Rad = Config.Psi2Rad;

            // Initialize the dictionary for tracking vehicles in the space.
            VehiclesPresentInSpace = new Dictionary<int, VE_Vehicle>();
        }
    }
}
