using Application_Scripts.Virtual_Entity.Vehicle;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Building.Controllers
{
    /// <summary>
    /// Controls the behavior and interactions of a building within the simulation.
    /// </summary>
    public class Building_Controller : MonoBehaviour
    {
        /// <summary>
        /// Runtime data associated with the building, including its state and configuration.
        /// </summary>
        [Header("Building Data")]
        public Building_Data BuildingData;

        /// <summary>
        /// Reference to the vehicle creator for managing vehicles related to the building.
        /// </summary>
        [Header("Vehicle Creator")]
        public Vehicle_Creator VehicleCreator;

        /// <summary>
        /// Initializes the building controller with the specified data and vehicle creator.
        /// </summary>
        /// <param name="data">The data associated with the building.</param>
        /// <param name="creator">The vehicle creator to manage related vehicles.</param>
        public void Init(Building_Data data, Vehicle_Creator creator)
        {
            BuildingData = data;
            VehicleCreator = creator;
        }
    }
}