using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Vehicle.Controllers.Kinematics_Strategy
{
    /// <summary>
    /// Kinematics strategy that uses a mathematical actuation model to calculate vehicle state.
    /// Suitable for scenarios involving simulated control inputs.
    /// </summary>
    [CreateAssetMenu(fileName = "Forward_Kinematics_Strategy", menuName = "Kinematic Models/Actuation")]
    public class Forward_Kinematics_Strategy : Kinematics_Strategy
    {
        /// <summary>
        /// Updates the vehicle state based on actuation inputs and physical properties.
        /// </summary>
        /// <param name="vehicle">The vehicle to update.</param>
        /// <param name="deltaTime">The Time elapsed since the last update, in seconds.</param>
        /// <param name="inputVelocity">The input velocity of the vehicle (m/s).</param>
        /// <param name="inputSteerAngle">The input steering angle (radians).</param>
        public override void UpdateKinematics(VE_Vehicle vehicle, float deltaTime, float inputVelocity, float inputSteerAngle)
        {
            // Calculate the front axle position
            vehicle.Data.X0 = vehicle.Data.X1 + vehicle.Data.L1 * Mathf.Cos(vehicle.Data.Psi1);
            vehicle.Data.Y0 = vehicle.Data.Y1 + vehicle.Data.L1 * Mathf.Sin(vehicle.Data.Psi1);

            // Update tractor position based on velocity and Time
            vehicle.Data.X1 = vehicle.Data.X1Prev + vehicle.Data.X1dot * deltaTime;
            vehicle.Data.Y1 = vehicle.Data.Y1Prev + vehicle.Data.Y1dot * deltaTime;

            // Calculate the coupling position between tractor and trailer
            vehicle.Data.X1C = vehicle.Data.X1 + vehicle.Data.L1C * Mathf.Cos(vehicle.Data.Psi1);
            vehicle.Data.Y1C = vehicle.Data.Y1 + vehicle.Data.L1C * Mathf.Sin(vehicle.Data.Psi1);

            // Update trailer position based on the coupling point
            vehicle.Data.X2 = vehicle.Data.X1C - vehicle.Data.L2 * Mathf.Cos(vehicle.Data.Psi2);
            vehicle.Data.Y2 = vehicle.Data.Y1C - vehicle.Data.L2 * Mathf.Sin(vehicle.Data.Psi2);

            // Calculate intermediate states for further kinematic properties
            vehicle.KinematicsController.CalculateIntermediateStates(vehicle.Data, deltaTime, inputVelocity, inputSteerAngle);
        }
    }
}
