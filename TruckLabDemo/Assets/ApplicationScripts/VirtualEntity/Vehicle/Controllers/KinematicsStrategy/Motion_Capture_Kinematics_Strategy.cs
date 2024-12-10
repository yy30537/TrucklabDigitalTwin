using UnityEngine;

namespace ApplicationScripts.VirtualEntity.Vehicle.Controllers.KinematicsStrategy
{
    /// <summary>
    /// Kinematics strategy that uses motion capture data to update vehicle state.
    /// For when real-world position and orientation data are available from Motive.
    /// </summary>
    [CreateAssetMenu(fileName = "Motion_Capture_Kinematics_Strategy", menuName = "Kinematic Models/Motion Capture")]
    public class Motion_Capture_Kinematics_Strategy : Kinematics_Strategy
    {
        /// <summary>
        /// Updates the vehicle state based on motion capture data.
        /// </summary>
        /// <param name="vehicle">The vehicle to update.</param>
        /// <param name="deltaTime">The Time elapsed since the last update (not used in this strategy).</param>
        /// <param name="inputVelocity">The input velocity (not used in this strategy).</param>
        /// <param name="inputSteerAngle">The input steering angle (not used in this strategy).</param>
        public override void UpdateKinematics(VE_Vehicle vehicle, float deltaTime, float inputVelocity, float inputSteerAngle)
        {
            // Update tractor position and orientation from motion capture data
            vehicle.Data.X1 = vehicle.TractorRigidBody.position.z * vehicle.Data.Config.Scale;
            vehicle.Data.Y1 = -vehicle.TractorRigidBody.position.x * vehicle.Data.Config.Scale;
            vehicle.Data.Psi1 = -vehicle.TractorRigidBody.rotation.eulerAngles.y * Mathf.Deg2Rad;

            // Update trailer position and orientation from motion capture data
            vehicle.Data.X2 = vehicle.TrailerRigidBody.position.z * vehicle.Data.Config.Scale;
            vehicle.Data.Y2 = -vehicle.TrailerRigidBody.position.x * vehicle.Data.Config.Scale;
            vehicle.Data.Psi2 = -vehicle.TrailerRigidBody.rotation.eulerAngles.y * Mathf.Deg2Rad;

            // Calculate the front axle position
            vehicle.Data.X0 = vehicle.Data.X1 + vehicle.Data.L1 * Mathf.Cos(vehicle.Data.Psi1);
            vehicle.Data.Y0 = vehicle.Data.Y1 + vehicle.Data.L1 * Mathf.Sin(vehicle.Data.Psi1);

            // Calculate the coupling position between tractor and trailer
            vehicle.Data.X1C = vehicle.Data.X1 + vehicle.Data.L1C * Mathf.Cos(vehicle.Data.Psi1);
            vehicle.Data.Y1C = vehicle.Data.Y1 + vehicle.Data.L1C * Mathf.Sin(vehicle.Data.Psi1);

            // Calculate intermediate states for further kinematic properties
            CalculateIntermediateStates(vehicle.Data, deltaTime, inputVelocity, inputSteerAngle);
        }

        /// <summary>
        /// Calculates intermediate kinematic states based on the current vehicle state and inputs.
        /// </summary>
        /// <param name="data">The vehicle data object.</param>
        /// <param name="deltaTime">The Time step for calculations.</param>
        /// <param name="inputVelocity">The input velocity.</param>
        /// <param name="inputSteerAngle">The input steering angle.</param>
        public override void CalculateIntermediateStates(Vehicle_Data data, float deltaTime, float inputVelocity, float inputSteerAngle)
        {
            // Store previous states
            data.X1Prev = data.X1;
            data.Y1Prev = data.Y1;
            data.Psi1Prev = data.Psi1;
            data.Psi2Prev = data.Psi2;

            // Update vehicle states using method parameters
            data.V1 = inputVelocity; // Velocity (m/s)
            data.Delta = inputSteerAngle; // Steering angle (radians)

            data.Gamma = data.Psi1 - data.Psi2;

            data.Psi1dot = (data.V1 / data.L1) * Mathf.Tan(data.Delta);
            data.Psi1 += data.Psi1dot * deltaTime;

            data.X1dot = data.V1 * Mathf.Cos(data.Psi1);
            data.Y1dot = data.V1 * Mathf.Sin(data.Psi1);

            data.Psi2dot = (data.V1 * Mathf.Sin(data.Gamma) + data.Psi1dot * data.L1C * Mathf.Cos(data.Gamma)) / data.L2;
            data.Psi2 += data.Psi2dot * deltaTime;

            //data.V2 = data.V1 * Mathf.Cos(data.Gamma) - data.Psi1dot * data.L1C * Mathf.Sin(data.Gamma);
        }
    }
}
