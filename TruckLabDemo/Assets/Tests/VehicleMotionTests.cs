// Assets/Tests/SimulationServices.cs
using System.Collections;
using System.Reflection; // For reflection
using ApplicationScripts.Manager.PathManager;
using ApplicationScripts.VirtualEntity.Space;
using ApplicationScripts.VirtualEntity.Vehicle;
using ApplicationScripts.VirtualEntity.Vehicle.Controllers;
using ApplicationScripts.VirtualEntity.Vehicle.Controllers.ActuationInputStrategy;
using ApplicationScripts.VirtualEntity.Vehicle.Controllers.KinematicsStrategy;
using NUnit.Framework;
using Tests.Scripts;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.GameObject;

namespace Assets.Tests
{
    /// <summary>
    /// Play Mode tests for Simulation Services Functional Requirements.
    /// Specifically testing FR4: Vehicle Dynamics Simulation and FR5: Path Previewing.
    /// </summary>
    public class VehicleMotionTests : PlayModeTests
    {
        public VE_Space Space;
        public VE_Vehicle Vehicle;

        public GameObject SpaceInstance;
        public GameObject VehicleInstance;


        /// <summary>
        /// Sets up the simulation environment and vehicle.
        /// </summary>
        /// <returns>IEnumerator for UnityTest.</returns>
        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            // Wait until scene setup is complete
            while (!SceneSetupComplete)
            {
                yield return new WaitForSeconds(1);
            }


            // Invoke Create Space Button
            GameObject.Find("Create All Spaces BTN").GetComponent<Button>().onClick.Invoke();

            // Invoke Create Building Button
            GameObject.Find("Create Building BTN").GetComponent<Button>().onClick.Invoke();

            

            // Invoke Create Vehicle Button
            var createVehicleButton = GameObject.Find("Create Vehicle BTN")?.GetComponent<Button>();
            Assert.IsNotNull(createVehicleButton, "Create Vehicle Button not found in the scene.");
            createVehicleButton.onClick.Invoke();

            // Wait for vehicle to be instantiated
            yield return new WaitForSeconds(1);

            // Retrieve Vehicle component
            Vehicle = GameObject.Find("Vehicles")?.GetComponentInChildren<VE_Vehicle>();
            VehicleInstance = Vehicle?.Instance;

            // Verify that the vehicle instance exists
            Assert.IsNotNull(Vehicle, "Vehicle component is null.");
            Assert.IsNotNull(VehicleInstance, "Vehicle instance is null.");

            // Verify that the vehicle is registered in the lookup table
            Assert.IsTrue(VehicleCreator.LookupTable.ContainsKey(Vehicle.Id),
                "Vehicle not registered in lookup table.");

            // Verify that VehicleData component is attached
            var vehicleData = Vehicle.Data;
            Assert.IsNotNull(vehicleData, "Vehicle_Data component is missing.");

            // Verify that KinematicsController component is attached and active
            var kinematicsController = Vehicle.KinematicsController;
            Assert.IsNotNull(kinematicsController, "KinematicsController component is missing from Vehicle instance.");
            Assert.IsTrue(kinematicsController.IsActive, "KinematicsController component is not active.");

            // Verify that AnimationController component is attached and active
            var animationController = Vehicle.AnimationController;
            Assert.IsNotNull(animationController, "AnimationController component is missing from Vehicle instance.");
            Assert.IsTrue(animationController.IsActive, "AnimationController component is not active.");

            // Verify that CollisionController component is attached and active
            var collisionController = Vehicle.CollisionController;
            Assert.IsNotNull(collisionController, "CollisionController component is missing from Vehicle instance.");
            Assert.IsTrue(collisionController.IsActive, "CollisionController component is not active.");

            if (ServiceMenuToggle.isActiveAndEnabled)
            {
                ServiceMenuToggle.onValueChanged.Invoke(false);
                yield return null; // Allow UI to update
            }
            
            if (BuildingDashboardsToggle.isActiveAndEnabled)
            {
                BuildingDashboardsToggle.onValueChanged.Invoke(false);
            }
            
            yield return new WaitForSeconds(2); 
        }

        [UnityTest]
        public IEnumerator TestKinematics()
        {
            Debug.Log("Testing: TestKinematics");

            // Assign the Mock Input Strategy to the vehicle's InputStrategiesDict
            var noInputStrategy = Resources.Load<Keyboard_Input_Strategy>("Tests/ScriptableObjects/Manual - No Input");
            Assert.IsNotNull(noInputStrategy, "Manual - No Input asset not found in Resources folder.");
            var mockInputStrategy = Resources.Load<Mock_Input_Strategy>("Tests/ScriptableObjects/Mock_Input_Strategy");
            Assert.IsNotNull(mockInputStrategy, "Mock_Input_Strategy asset not found.");


            // Clear existing input strategies and add the mock strategy
            Vehicle.Config.InputStrategies.Clear();
            Vehicle.Config.InputStrategies.Add(noInputStrategy);
            Vehicle.Config.InputStrategies.Add(mockInputStrategy);
            Vehicle.Config.OnEnable(); // Reinitialize the dictionary

            // Change the vehicle's input strategy
            bool strategyChanged = Vehicle.KinematicsController.ChangeInputStrategy(noInputStrategy.StrategyName);
            Assert.IsTrue(strategyChanged, "Failed to change input strategy to Manual - No Input.");

            yield return new WaitForSeconds(2);

            var data = Vehicle.Data;
            //var kinController = Vehicle.KinematicsController;

            mockInputStrategy.MockVelocity = 10f;
            mockInputStrategy.MockSteerAngle = -2.5f * Mathf.Deg2Rad;

            // Retrieve input
            float v = mockInputStrategy.MockVelocity;        
            float delta = mockInputStrategy.MockSteerAngle; 
            float dt = Time.fixedDeltaTime;

            //kinController.InputVelocity = v;
            //kinController.InputSteerAngle = delta;

            int steps = 500;

            // step-by-step calculation:
            // The equations from Forward_Kinematics_Strategy:
            // psi1dot = (V/L1)*tan(delta)
            // psi1_new = psi1_old + psi1dot*dt
            // X1dot = V*cos(psi1_new)
            // Y1dot = V*sin(psi1_new)
            // X1_new = X1_old + X1dot*dt
            // Y1_new = Y1_old + Y1dot*dt
            // gamma = psi1 - psi2
            // psi2dot = (V*sin(gamma)+psi1dot*L1C*cos(gamma))/L2
            // psi2_new = psi2_old + psi2dot*dt

            // vehicle parameters: L1, L1C, L2 from data
            float L1 = data.L1;
            float L1C = data.L1C;
            float L2 = data.L2;

            float X1 = data.X1;
            float Y1 = data.Y1;
            float Psi1 = data.Psi1;
            float Psi2 = data.Psi2;

            strategyChanged = Vehicle.KinematicsController.ChangeInputStrategy(mockInputStrategy.StrategyName);
            Assert.IsTrue(strategyChanged, "Failed to set mock input strategy.");
            Vehicle.VehicleDashboardController.TractorTrailToggle.onValueChanged.Invoke(true);

            for (int i = 0; i < steps; i++)
            {
                // Manually compute expected next state
                float gamma = Psi1 - Psi2;
                float psi1dot = (v / L1) * Mathf.Tan(delta);
                float Psi1_new = Psi1 + psi1dot * dt;

                float X1dot = v * Mathf.Cos(Psi1_new);
                float Y1dot = v * Mathf.Sin(Psi1_new);

                float X1_new = X1 + X1dot * dt;
                float Y1_new = Y1 + Y1dot * dt;

                float psi2dot = (v * Mathf.Sin(gamma) + psi1dot * L1C * Mathf.Cos(gamma)) / L2;
                float Psi2_new = Psi2 + psi2dot * dt;

                // Let the simulation run one physics step
                yield return new WaitForFixedUpdate();

                // After the step, check the actual vehicle data
                float tol = 0.2f; // small tolerance

                Assert.AreEqual(X1_new, data.X1, tol, $"Step {i}: X1 mismatch");
                Assert.AreEqual(Y1_new, data.Y1, tol, $"Step {i}: Y1 mismatch");
                Assert.AreEqual(Psi1_new, data.Psi1, tol, $"Step {i}: Psi1 mismatch");
                Assert.AreEqual(Psi2_new, data.Psi2, tol, $"Step {i}: Psi2 mismatch");

                // Update for next iteration
                X1 = X1_new;
                Y1 = Y1_new;
                Psi1 = Psi1_new;
                Psi2 = Psi2_new;
            }
            strategyChanged = Vehicle.KinematicsController.ChangeInputStrategy(noInputStrategy.StrategyName);
            Assert.IsTrue(strategyChanged, "Failed to change input strategy to Manual - No Input.");
            yield return new WaitForSeconds(2);
            Vehicle.VehicleDashboardController.TractorTrailToggle.onValueChanged.Invoke(false);

            // If we reach here without assertion failures,
            // it means at each step the code's results matched the manual calculations.
        }
        
    }
}



