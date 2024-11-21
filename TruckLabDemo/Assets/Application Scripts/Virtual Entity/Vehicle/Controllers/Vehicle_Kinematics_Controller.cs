using System.Collections;
using Application_Scripts.Manager.Path_Manager;
using Application_Scripts.Virtual_Entity.Vehicle.Controllers.Actuation_Input_Strategy;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Vehicle.Controllers
{
    /// <summary>
    /// Controls the kinematics of the VeVehicle, managing its motion, steering, and actuation inputs.
    /// </summary>
    public class Vehicle_Kinematics_Controller : MonoBehaviour
    {
        /// <summary>
        /// The associated vehicle object.
        /// </summary>
        public VE_Vehicle VeVehicle;

        /// <summary>
        /// The associated vehicle data object.
        /// </summary>
        public Vehicle_Data VehicleData;

        /// <summary>
        /// Indicates whether the kinematics controller is active.
        /// </summary>
        public bool IsActive = false;

        /// <summary>
        /// The selected kinematics strategy for vehicle motion.
        /// </summary>
        public Kinematics_Strategy.Kinematics_Strategy KinematicStrategy;

        /// <summary>
        /// The currently active actuation input strategy.
        /// </summary>
        public Actuation_Input_Strategy.Actuation_Input_Strategy ActiveInputStrategy;

        /// <summary>
        /// The current input velocity of the vehicle.
        /// </summary>
        public float InputVelocity;

        /// <summary>
        /// The current input steering angle of the vehicle.
        /// </summary>
        public float InputSteerAngle;

        private bool isBraking = false; // Indicates whether the vehicle is currently braking.

        /// <summary>
        /// Called at fixed intervals, processes kinematics updates and input strategies.
        /// </summary>
        private void FixedUpdate()
        {
            if (IsActive && ActiveInputStrategy != null)
            {
                if (isBraking)
                {
                    Brake();
                }

                // Retrieve inputs from the active input strategy
                var (inputV, inputPsi) = ActiveInputStrategy.GetInput();
                InputVelocity = inputV;
                InputSteerAngle = inputPsi;

                // Update kinematics based on the selected strategy
                KinematicStrategy?.UpdateKinematics(VeVehicle, Time.deltaTime, InputVelocity, InputSteerAngle);
            }
        }

        /// <summary>
        /// Initializes the kinematics controller with the specified vehicle and data.
        /// </summary>
        /// <param name="vehicle">The vehicle object.</param>
        /// <param name="data">The vehicle data object.</param>
        public void Init(VE_Vehicle vehicle, Vehicle_Data data)
        {
            VeVehicle = vehicle;
            VehicleData = data;
            KinematicStrategy = data.Config.KinematicStrategy;

            // Set the initial input strategy if available
            if (vehicle.Config.VehicleInputStrategiesDict.Count > 0)
            {
                var firstStrategy = vehicle.Config.VehicleInputStrategiesDict.Values.GetEnumerator();
                if (firstStrategy.MoveNext())
                {
                    SetInputStrategy(firstStrategy.Current);
                }
            }

            // Initialize vehicle position and states
            SetVehiclePosition(
                VehicleData.Config.InitialTractorPosX,
                VehicleData.Config.InitialTractorPosY,
                VehicleData.Config.InitialTractorAngle,
                VehicleData.Config.InitialTrailerAngle
            );

            data.V1 = VehicleData.Config.InitialVelocity;
            data.Delta = VehicleData.Config.InitialSteeringAngle;
            data.a = VehicleData.Config.InitialAcceleration;
        }

        /// <summary>
        /// Retrieves the name of the currently active input strategy.
        /// </summary>
        /// <returns>The name of the active input strategy, or null if none is active.</returns>
        public string GetCurrentInputStrategyName()
        {
            foreach (var kvp in VeVehicle.Config.VehicleInputStrategiesDict)
            {
                if (kvp.Value == ActiveInputStrategy)
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Changes the input strategy to the specified strategy by name.
        /// </summary>
        /// <param name="strategyName">The name of the new input strategy.</param>
        /// <returns>True if the strategy was changed successfully, false otherwise.</returns>
        public bool ChangeInputStrategy(string strategyName)
        {
            if (VeVehicle.Config.VehicleInputStrategiesDict.TryGetValue(strategyName, out var newStrategy))
            {
                SetInputStrategy(newStrategy);
                return true;
            }
            Debug.LogWarning($"Input strategy '{strategyName}' not found.");
            ActiveInputStrategy = null;
            return false;
        }

        /// <summary>
        /// Sets the active input strategy.
        /// </summary>
        /// <param name="inputStrategy">The input strategy to activate.</param>
        public void SetInputStrategy(Actuation_Input_Strategy.Actuation_Input_Strategy inputStrategy)
        {
            ActiveInputStrategy = inputStrategy;

            // Initialize the controller input strategy if applicable
            if (inputStrategy is Controller_Input_Strategy controllerInputStrategy && VeVehicle.SharedTwistSubscriber != null)
            {
                controllerInputStrategy.Initialize(VeVehicle.SharedTwistSubscriber);
            }
        }

        /// <summary>
        /// Calculates intermediate kinematic states based on the current vehicle state and inputs.
        /// </summary>
        /// <param name="data">The vehicle data object.</param>
        /// <param name="deltaTime">The Time step for calculations.</param>
        /// <param name="inputVelocity">The input velocity.</param>
        /// <param name="inputSteerAngle">The input steering angle.</param>
        public void CalculateIntermediateStates(Vehicle_Data data, float deltaTime, float inputVelocity, float inputSteerAngle)
        {
            // Store previous states
            data.X1Prev = data.X1;
            data.Y1Prev = data.Y1;
            data.Psi1Prev = data.Psi1;
            data.Psi2Prev = data.Psi2;

            // Update vehicle states
            data.V1 = InputVelocity; // Velocity (m/s)
            data.Delta = InputSteerAngle; // Steering angle (radians)

            data.Gamma = data.Psi1 - data.Psi2;

            data.Psi1dot = (data.V1 / data.L1) * Mathf.Tan(data.Delta);
            data.Psi1 += data.Psi1dot * deltaTime;

            data.X1dot = data.V1 * Mathf.Cos(data.Psi1);
            data.Y1dot = data.V1 * Mathf.Sin(data.Psi1);

            data.Psi2dot = (data.V1 * Mathf.Sin(data.Gamma) + data.Psi1dot * data.L1C * Mathf.Cos(data.Gamma)) / data.L2;
            data.Psi2 += data.Psi2dot * deltaTime;

            data.V2 = data.V1 * Mathf.Cos(data.Gamma) - data.Psi1dot * data.L1C * Mathf.Sin(data.Gamma);
        }

        /// <summary>
        /// Sets the vehicle's position and orientation.
        /// </summary>
        /// <param name="x">The X-coordinate of the tractor's position.</param>
        /// <param name="y">The Y-coordinate of the tractor's position.</param>
        /// <param name="psi1Degrees">The tractor's orientation angle in degrees.</param>
        /// <param name="psi2Degrees">The trailer's orientation angle in degrees.</param>
        public void SetVehiclePosition(float x, float y, float psi1Degrees, float psi2Degrees)
        {
            VehicleData.X1 = x;
            VehicleData.Y1 = y;
            VehicleData.Psi1 = psi1Degrees * Mathf.Deg2Rad;
            VehicleData.Psi2 = psi2Degrees * Mathf.Deg2Rad;

            CalculateIntermediateStates(VehicleData, Time.deltaTime, 0, 0);
        }

        /// <summary>
        /// Adjusts the angle of the tractor by the specified input value and adjustment rate.
        /// </summary>
        /// <param name="inputValue">The input value affecting the tractor angle.</param>
        /// <param name="angleAdjustmentRate">The rate at which the angle changes.</param>
        public void SetTractorAngle(float inputValue, float angleAdjustmentRate)
        {
            VehicleData.Psi1 += inputValue * angleAdjustmentRate;
            CalculateIntermediateStates(VehicleData, Time.deltaTime, 0, 0);
        }

        /// <summary>
        /// Adjusts the angle of the trailer by the specified input value and adjustment rate.
        /// </summary>
        /// <param name="inputValue">The input value affecting the trailer angle.</param>
        /// <param name="angleAdjustmentRate">The rate at which the angle changes.</param>
        public void SetTrailerAngle(float inputValue, float angleAdjustmentRate)
        {
            VehicleData.Psi2 += inputValue * angleAdjustmentRate;
            CalculateIntermediateStates(VehicleData, Time.deltaTime, 0, 0);
        }

        /// <summary>
        /// Initializes the vehicle's position and orientation based on the first path frame.
        /// </summary>
        /// <param name="path">The path containing the position and orientation data.</param>
        public void InitPathReplay(Path path)
        {
            // Extract data from the first frame of the path
            Vector2 frontAxlePosition = path.FrontAxle[0];
            Vector2 rearAxlePosition = path.RearAxle[0];
            Vector2 trailerAxlePosition = path.TrailerAxle[0];
            Vector2 orientation = path.Psi[0];

            // Initialize vehicle state
            VehicleData.X1 = frontAxlePosition.x;
            VehicleData.Y1 = frontAxlePosition.y;
            VehicleData.Psi1 = orientation.x;
            VehicleData.X2 = trailerAxlePosition.x;
            VehicleData.Y2 = trailerAxlePosition.y;
            VehicleData.Psi2 = orientation.y;

            // Calculate derived positions for axles
            VehicleData.X0 = frontAxlePosition.x + VehicleData.L1 * Mathf.Cos(VehicleData.Psi1);
            VehicleData.Y0 = frontAxlePosition.y + VehicleData.L1 * Mathf.Sin(VehicleData.Psi1);
            VehicleData.X1C = rearAxlePosition.x + VehicleData.L1C * Mathf.Cos(VehicleData.Psi1);
            VehicleData.Y1C = rearAxlePosition.y + VehicleData.L1C * Mathf.Sin(VehicleData.Psi1);

            CalculateIntermediateStates(VehicleData, Time.deltaTime, 0, 0);
        }

        /// <summary>
        /// Starts replaying the vehicle's motion along a predefined path.
        /// </summary>
        /// <param name="path">The path containing the motion data.</param>
        public void StartPathReplaying(Path path)
        {
            StartCoroutine(PathReplayCoroutine(path));
        }

        /// <summary>
        /// Stops replaying the vehicle's motion along a predefined path.
        /// </summary>
        /// <param name="path">The path containing the motion data.</param>
        public void StopPathReplaying(Path path)
        {
            StopCoroutine(PathReplayCoroutine(path));
        }

        /// <summary>
        /// Coroutine for replaying the vehicle's motion along a predefined path.
        /// </summary>
        /// <param name="path">The path containing the motion data.</param>
        private IEnumerator PathReplayCoroutine(Path path)
        {
            for (int i = 0; i < path.Time.Count; i++)
            {
                // Extract data for the current frame
                Vector2 frontAxlePosition = path.FrontAxle[i];
                Vector2 rearAxlePosition = path.RearAxle[i];
                Vector2 trailerAxlePosition = path.TrailerAxle[i];
                Vector2 orientation = path.Psi[i];

                // Update vehicle state
                VehicleData.X0 = VehicleData.X1 + VehicleData.L1 * Mathf.Cos(VehicleData.Psi1);
                VehicleData.Y0 = VehicleData.Y1 + VehicleData.L1 * Mathf.Sin(VehicleData.Psi1);
                VehicleData.X1 = frontAxlePosition.x;
                VehicleData.Y1 = frontAxlePosition.y;
                VehicleData.Psi1 = orientation.x;
                VehicleData.X1C = VehicleData.X1 + VehicleData.L1C * Mathf.Cos(VehicleData.Psi1);
                VehicleData.Y1C = VehicleData.Y1 + VehicleData.L1C * Mathf.Sin(VehicleData.Psi1);
                VehicleData.X2 = trailerAxlePosition.x;
                VehicleData.Y2 = trailerAxlePosition.y;
                VehicleData.Psi2 = orientation.y;

                // Wait for the appropriate Time interval before updating the next frame
                float waitTime = path.Time[i] - (i > 0 ? path.Time[i - 1] : 0);
                CalculateIntermediateStates(VehicleData, Time.deltaTime, 0, 0);
                yield return new WaitForSeconds(waitTime);
            }
        }

        /// <summary>
        /// Executes braking logic for the vehicle.
        /// </summary>
        /// <remarks>
        /// Placeholder for more advanced braking logic in the future.
        /// </remarks>
        private void Brake()
        {
            // TODO: Implement advanced braking logic.
            // InputVelocity = 0;
        }

        /// <summary>
        /// Activates the braking mechanism for the vehicle.
        /// </summary>
        public void ApplyBrakes()
        {
            isBraking = true;
        }

        /// <summary>
        /// Deactivates the braking mechanism for the vehicle.
        /// </summary>
        public void ReleaseBrakes()
        {
            isBraking = false;
        }

    }
}
