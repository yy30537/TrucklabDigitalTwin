using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Vehicle.Controllers.Collision_Control
{
    /// <summary>
    /// Represents an obstacle detected by the vehicle's sensors.
    /// </summary>
    [System.Serializable]
    public class Obstacle_Info
    {
        /// <summary>
        /// The detected obstacle's GameObject.
        /// </summary>
        public GameObject Object;

        /// <summary>
        /// The distance from the vehicle's sensor to the obstacle.
        /// </summary>
        public float Distance;

        /// <summary>
        /// The angle of the obstacle relative to the vehicle's forward direction.
        /// </summary>
        public float Angle;

        /// <summary>
        /// A descriptive name for the obstacle.
        /// </summary>
        public string Name;
    }
}