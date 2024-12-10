using System.Collections;
using ApplicationScripts.Manager.PathManager;
using ApplicationScripts.VirtualEntity.Vehicle.Controllers.ActuationInputStrategy;
using ApplicationScripts.VirtualEntity.Vehicle.Controllers.KinematicsStrategy;
using UnityEngine;

namespace ApplicationScripts.VirtualEntity.Vehicle.Controllers
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
        public Kinematics_Strategy KinematicStrategy;

        /// <summary>
        /// The currently active actuation input strategy.
        /// </summary>
        public Actuation_Input_Strategy ActiveInputStrategy;

        // Variables to handle simulation inputs
        private bool isPathReplaying = false;
        private Reference_Path currentReferencePath;
 

        /// <summary>
        /// The current input velocity of the vehicle.
        /// </summary>
        public float InputVelocity;

        /// <summary>
        /// The current input steering angle of the vehicle.
        /// </summary>
        public float InputSteerAngle;

        private bool isBraking = false; // Indicates whether the vehicle is currently braking.

        // Simulation time tracker
        private float simulationTime = 0f;

        // Input event indices
        private int steerIndex = 0;
        private int vxIndex = 0;

        // Input event arrays
        private float[] steerTimes;
        private float[] steerValues;
        private float[] vxTimes;
        private float[] vxValues;


        /// <summary>
        /// Called at fixed intervals, processes kinematics updates and input strategies.
        /// </summary>
        public void FixedUpdate()
        {
            if (IsActive && ActiveInputStrategy != null)
            {
                if (isBraking)
                {
                    Brake();
                }
                else
                {
                    if (isPathReplaying && currentReferencePath != null)
                    {
                        // Increment simulation time based on fixed delta time
                        simulationTime += Time.fixedDeltaTime;

                        // Check if the simulation has reached the end of the referencePath
                        if (simulationTime >= currentReferencePath.Time[currentReferencePath.Time.Count - 1])
                        {
                            StopPathReplaying();
                            Debug.Log("Reference_Path replay completed.");
                        }
                    }
                    else
                    {
                        // Retrieve inputs from the active input strategy
                        var (inputV, inputPsi) = ActiveInputStrategy.GetInput();
                        InputVelocity = inputV;
                        InputSteerAngle = inputPsi;
                    }
                }

                
            }
            // Update kinematics based on the selected strategy using fixedDeltaTime
            KinematicStrategy?.UpdateKinematics(VeVehicle, Time.fixedDeltaTime, InputVelocity, InputSteerAngle);
        }

        /// <summary>
        /// Applies the steering input if the simulation time has reached the specified timestamp.
        /// </summary>
        /// <param name="currentTime">The current simulation time.</param>
        private void ApplySteeringInput(float currentTime)
        {
            // Check if there are remaining steering inputs
            if (steerIndex < steerTimes.Length)
            {
                // If current simulation time has reached or passed the steer input timestamp
                if (currentTime >= steerTimes[steerIndex])
                {
                    float newSteerAngle = steerValues[steerIndex];
                    InputSteerAngle = newSteerAngle;
                    Debug.Log($"Steer input at {steerTimes[steerIndex]:F3}s: {newSteerAngle}");
                    steerIndex++;
                }
            }
        }

        /// <summary>
        /// Applies the velocity input if the simulation time has reached the specified timestamp.
        /// </summary>
        /// <param name="currentTime">The current simulation time.</param>
        private void ApplyVelocityInput(float currentTime)
        {
            // Check if there are remaining velocity inputs
            if (vxIndex < vxTimes.Length)
            {
                // If current simulation time has reached or passed the velocity input timestamp
                if (currentTime >= vxTimes[vxIndex])
                {
                    float newVx = vxValues[vxIndex];
                    InputVelocity = newVx;
                    Debug.Log($"Vx input at {vxTimes[vxIndex]:F3}s: {newVx}");
                    vxIndex++;
                }
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

            // Initialize referencePath replay variables
            currentReferencePath = null;
            simulationTime = 0f;
            steerIndex = 0;
            vxIndex = 0;
        }


        /****************************
        *       InputStrategy
        *****************************/

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
        public void SetInputStrategy(Actuation_Input_Strategy inputStrategy)
        {
            ActiveInputStrategy = inputStrategy;

            // Initialize the controller input strategy if applicable
            if (inputStrategy is Controller_Input_Strategy controllerInputStrategy && VeVehicle.SharedTwistSubscriber != null)
            {
                controllerInputStrategy.Initialize(VeVehicle.SharedTwistSubscriber);
            }
        }


        /**********************************
         * Position and Orientation setter
         **********************************/
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

            // Use Time.fixedDeltaTime instead of Time.deltaTime
            KinematicStrategy.CalculateIntermediateStates(VehicleData, Time.fixedDeltaTime, 0, 0);
        }

        /// <summary>
        /// Adjusts the angle of the tractor by the specified input value and adjustment rate.
        /// </summary>
        /// <param name="inputValue">The input value affecting the tractor angle.</param>
        /// <param name="angleAdjustmentRate">The rate at which the angle changes.</param>
        public void SetTractorAngle(float inputValue, float angleAdjustmentRate)
        {
            VehicleData.Psi1 += inputValue * angleAdjustmentRate;
            KinematicStrategy.CalculateIntermediateStates(VehicleData, Time.fixedDeltaTime, 0, 0);
        }

        /// <summary>
        /// Adjusts the angle of the trailer by the specified input value and adjustment rate.
        /// </summary>
        /// <param name="inputValue">The input value affecting the trailer angle.</param>
        /// <param name="angleAdjustmentRate">The rate at which the angle changes.</param>
        public void SetTrailerAngle(float inputValue, float angleAdjustmentRate)
        {
            VehicleData.Psi2 += inputValue * angleAdjustmentRate;
            KinematicStrategy.CalculateIntermediateStates(VehicleData, Time.fixedDeltaTime, 0, 0);
        }


        /****************************
         * Reference_Path Replay
         *****************************/
        /// <summary>
        /// Initializes the vehicle's position and orientation based on the first referencePath frame.
        /// </summary>
        /// <param name="referencePath">The referencePath containing the position and orientation data.</param>
        public void InitPathReplay(Reference_Path referencePath)
        {
            if (referencePath.FrontAxle.Count == 0 || referencePath.RearAxle.Count == 0 || referencePath.TrailerAxle.Count == 0 || referencePath.Psi.Count == 0)
            {
                Debug.LogError("Reference_Path data is incomplete.");
                return;
            }

            // Extract data from the first frame of the referencePath
            Vector2 frontAxlePosition = referencePath.FrontAxle[0];
            Vector2 rearAxlePosition = referencePath.RearAxle[0];
            Vector2 trailerAxlePosition = referencePath.TrailerAxle[0];
            Vector2 orientation = referencePath.Psi[0];

            // Initialize vehicle state
            VehicleData.X1 = frontAxlePosition.x;
            VehicleData.Y1 = frontAxlePosition.y;
            VehicleData.Psi1 = orientation.x;
            VehicleData.Psi2 = orientation.y;
            VehicleData.X2 = trailerAxlePosition.x;
            VehicleData.Y2 = trailerAxlePosition.y;

            // Calculate derived positions for axles
            VehicleData.X0 = frontAxlePosition.x + VehicleData.L1 * Mathf.Cos(VehicleData.Psi1);
            VehicleData.Y0 = frontAxlePosition.y + VehicleData.L1 * Mathf.Sin(VehicleData.Psi1);
            VehicleData.X1C = rearAxlePosition.x + VehicleData.L1C * Mathf.Cos(VehicleData.Psi1);
            VehicleData.Y1C = rearAxlePosition.y + VehicleData.L1C * Mathf.Sin(VehicleData.Psi1);

            // Use Time.fixedDeltaTime instead of Time.deltaTime
            KinematicStrategy.CalculateIntermediateStates(VehicleData, Time.fixedDeltaTime, 0, 0);
        }

        /// <summary>
        /// Starts replaying the vehicle's motion along a predefined referencePath.
        /// </summary>
        /// <param name="referencePath">The referencePath containing the motion data.</param>
        public void StartPathReplaying(Reference_Path referencePath)
        {
            if (isPathReplaying)
            {
                Debug.LogWarning("Reference_Path replay is already in progress.");
                return;
            }

            if (referencePath == null || !referencePath.IsValid)
            {
                Debug.LogError("Invalid referencePath provided for replay.");
                return;
            }

            currentReferencePath = referencePath;
            simulationTime = 0f;
            steerIndex = 0;
            vxIndex = 0;
            isPathReplaying = true;

            // Initialize vehicle state based on the referencePath
            InitPathReplay(referencePath);

            // Initialize InputVelocity to SimInput.Vx at the start
            if (referencePath.SimInput != null)
            {
                InputVelocity = referencePath.SimInput.Vx;

                // Initialize input event arrays
                if (referencePath.SimInput.SteerInput != null && referencePath.SimInput.SteerInput.Length >= 2)
                {
                    steerTimes = referencePath.SimInput.SteerInput[0];
                    steerValues = referencePath.SimInput.SteerInput[1];
                }

                if (referencePath.SimInput.VxInput != null && referencePath.SimInput.VxInput.Length >= 2)
                {
                    vxTimes = referencePath.SimInput.VxInput[0];
                    vxValues = referencePath.SimInput.VxInput[1];
                }

            }
            //else
            //{
            //    Debug.LogError("SimInput is null in the provided referencePath.");
            //    InputVelocity = 0f; // Default value or handle appropriately

            //    // Initialize input event arrays as empty
            //    steerTimes = new float[0];
            //    steerValues = new float[0];
            //    vxTimes = new float[0];
            //    vxValues = new float[0];
            //}

            Debug.Log($"Started replaying referencePath: {referencePath.PathName}");

            InvokeRepeating("ReplayInputs", 0.0f, 0.1f);
            
        }

        void ReplayInputs()
        {
            // Apply simulation steering inputs 
            ApplySteeringInput(simulationTime);

            // Apply simulation velocity inputs 
            ApplyVelocityInput(simulationTime);
        }

        /// <summary>
        /// Stops replaying the vehicle's motion along a predefined referencePath.
        /// </summary>
        public void StopPathReplaying()
        {
            if (isPathReplaying)
            {
                isPathReplaying = false;
                Debug.Log("Reference_Path replay stopped.");
            }
        }

        /****************************
         * Reference_Path Visualization
         *****************************/

        /// <summary>
        /// Initializes the vehicle's position and orientation based on the first referencePath frame.
        /// </summary>
        /// <param name="referencePath">The referencePath containing the position and orientation data.</param>
        public void InitPathVisualization(Reference_Path referencePath)
        {
            // Extract data from the first frame of the referencePath
            Vector2 frontAxlePosition = referencePath.FrontAxle[0];
            Vector2 rearAxlePosition = referencePath.RearAxle[0];
            Vector2 trailerAxlePosition = referencePath.TrailerAxle[0];
            Vector2 orientation = referencePath.Psi[0];

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

            KinematicStrategy.CalculateIntermediateStates(VehicleData, Time.deltaTime, 0, 0);
        }

        /// <summary>
        /// Starts replaying the vehicle's motion along a predefined referencePath.
        /// </summary>
        /// <param name="referencePath">The referencePath containing the motion data.</param>
        public void StartPathVisualization(Reference_Path referencePath)
        {
            StartCoroutine(PathVisualizationCoroutine(referencePath));
        }

        /// <summary>
        /// Stops replaying the vehicle's motion along a predefined referencePath.
        /// </summary>
        /// <param name="referencePath">The referencePath containing the motion data.</param>
        public void StopPathVisualization(Reference_Path referencePath)
        {
            StopCoroutine(PathVisualizationCoroutine(referencePath));
        }

        /// <summary>
        /// Coroutine for replaying the vehicle's motion along a predefined referencePath.
        /// </summary>
        /// <param name="referencePath">The referencePath containing the motion data.</param>
        private IEnumerator PathVisualizationCoroutine(Reference_Path referencePath)
        {
            for (int i = 0; i < referencePath.Time.Count; i++)
            {
                // Extract data for the current frame
                Vector2 frontAxlePosition = referencePath.FrontAxle[i];
                Vector2 rearAxlePosition = referencePath.RearAxle[i];
                Vector2 trailerAxlePosition = referencePath.TrailerAxle[i];
                Vector2 orientation = referencePath.Psi[i];

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

                KinematicStrategy.CalculateIntermediateStates(VehicleData, Time.fixedDeltaTime, 0, 0);
                // Wait for the appropriate Time interval before updating the next frame
                float waitTime = referencePath.Time[i] - (i > 0 ? referencePath.Time[i - 1] : 0);
                yield return new WaitForSeconds(waitTime);
            }
        }

        /****************************
         *    Braking Adhoc
         *****************************/
        /// <summary>
        /// Executes braking logic for the vehicle.
        /// </summary>
        /// <remarks>
        /// Placeholder for more advanced braking logic in the future.
        /// </remarks>
        private void Brake()
        {
            // TODO: Implement advanced braking logic.
            // For now, set InputVelocity to 0 to simulate braking
            InputVelocity = 0;
            //Debug.Log("Brakes applied.");
        }

        /// <summary>
        /// Activates the braking mechanism for the vehicle.
        /// </summary>
        public void ApplyBrakes()
        {
            isBraking = true;
            //Debug.Log("Brakes activated.");
        }

        /// <summary>
        /// Deactivates the braking mechanism for the vehicle.
        /// </summary>
        public void ReleaseBrakes()
        {
            isBraking = false;
            //Debug.Log("Brakes released.");
        }
    }
}
