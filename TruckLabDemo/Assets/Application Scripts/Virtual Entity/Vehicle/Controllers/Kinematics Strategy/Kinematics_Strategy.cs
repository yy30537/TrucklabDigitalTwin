using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Vehicle.Controllers.Kinematics_Strategy
{
    /// <summary>
    /// Abstract base class for kinematics strategies.
    /// Provides a framework for updating vehicle kinematics based on specific models or data sources.
    /// </summary>
    public abstract class Kinematics_Strategy : ScriptableObject
    {
        /// <summary>
        /// Name of the kinematics strategy, used for identification and debugging.
        /// </summary>
        public string KinematicsStrategyName;

        /// <summary>
        /// Updates the kinematic properties of the specified vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle to update.</param>
        /// <param name="deltaTime">The Time elapsed since the last update, in seconds.</param>
        /// <param name="inputVelocity">The input velocity of the vehicle (m/s).</param>
        /// <param name="inputSteerAngle">The input steering angle (radians).</param>
        public abstract void UpdateKinematics(VE_Vehicle vehicle, float deltaTime, float inputVelocity, float inputSteerAngle);
    }
}