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
    public class CollisionDetection : PlayModeTests
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

            yield return new WaitForSeconds(2); 
        }


        [UnityTest]
        public IEnumerator TestObstacleDetection()
        {
            

            if (SpaceDashboardsToggle.isActiveAndEnabled) 
            {
                SpaceDashboardsToggle.onValueChanged.Invoke(false);
                yield return null; // Allow UI to update
            }

            if (BuildingDashboardsToggle.isActiveAndEnabled) 
            {
                BuildingDashboardsToggle.onValueChanged.Invoke(false);
                yield return null; // Allow UI to update
            }
            
            if (ServiceMenuToggle.isActiveAndEnabled)
            {
                ServiceMenuToggle.onValueChanged.Invoke(false);
                yield return null; // Allow UI to update
            }

            Debug.Log("Testing: TestObstacleDetection");
            //yield return TestKinematics();
            yield return new WaitForSeconds(2);

            var collisionController = Vehicle.CollisionController;
            Assert.IsNotNull(collisionController, "Vehicle_Collision_Controller not found.");
            collisionController.IsActive = true; // Ensure collision detection is active

            // Load the obstacle prefab
            var obstaclePrefab = Resources.Load<GameObject>("Tests/Prefabs/Obstacle Cube");
            Assert.IsNotNull(obstaclePrefab, "Obstacle Cube prefab not found in Resources/Tests/Prefabs.");


            Vector3 vehiclePos = new Vector3(Vehicle.Data.X1, 5f, Vehicle.Data.Y1);
            Vector3 obstaclePos = vehiclePos + Vehicle.Tractor.transform.forward * 100f;

            var obstacleInstance = GameObject.Instantiate(obstaclePrefab, obstaclePos, Quaternion.identity);
            Assert.IsNotNull(obstacleInstance, "Failed to instantiate obstacle cube.");


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


            
            Vehicle.VehicleDashboardController.TractorTrailToggle.onValueChanged.Invoke(true);
            yield return new WaitForSeconds(2);

            // Change the vehicle's input strategy
            bool strategyChanged = Vehicle.KinematicsController.ChangeInputStrategy(noInputStrategy.StrategyName);
            Assert.IsTrue(strategyChanged, "Failed to change input strategy to Manual - No Input.");


            mockInputStrategy.MockVelocity = 5f;
            mockInputStrategy.MockSteerAngle = 0;

            strategyChanged = Vehicle.KinematicsController.ChangeInputStrategy(mockInputStrategy.StrategyName);
            Assert.IsTrue(strategyChanged, "Failed to set input strategy to Mock_Input_Strategy.");

            // The mock input strategy gives a certain forward velocity (e.g. 10 m/s) and a steering angle
            // That means the vehicle will move straight forward. Wait until vehicle moves close enough
            // so that the obstacle enters the sensor's trigger range.

            // The sensor radius and braking threshold in the code:
            // BrakingThreshold default is 20f, sphere collider radius likely ~50f (adjust as needed).
            // Starting obstacle at 100m away. With velocity=10 m/s, in ~10 seconds it should be at ~0 distance.
            // We'll wait 12 seconds to ensure detection occurs.

            float waitTime = 20f;
            yield return new WaitForSeconds(waitTime);

            // After waiting, the vehicle should have moved close enough to detect the obstacle.
            // Verify that the obstacle is detected.
            var detectedObstacles = collisionController.DetectedObstacles;
            Assert.IsTrue(detectedObstacles.Count > 0, "No obstacles detected, expected at least one.");

            bool obstacleFound = false;
            foreach (var obs in detectedObstacles)
            {
                if (obs.Object == obstacleInstance)
                {
                    Debug.Log($"Obstacle: {obs.Name}");
                    obstacleFound = true;
                    break;
                }
            }

            Assert.IsTrue(obstacleFound, "The instantiated obstacle was not detected by the vehicle.");

            // Additionally, we can verify if braking was applied by checking the vehicle's InputVelocity
            // after detection, but that depends on how the code handles braking logic.
            // If you want to verify braking:
            // Assert.AreEqual(0f, Vehicle.KinematicsController.InputVelocity, 0.1f, "Vehicle did not brake as expected.");

            yield return new WaitForSeconds(2);
        }


    }
}



