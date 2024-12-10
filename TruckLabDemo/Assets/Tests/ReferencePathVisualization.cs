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
    public class ReferencePathVisualization : PlayModeTests
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
        public IEnumerator TestPathVisualization()
        {
            Debug.Log("Testing: TestPathVisualization");
            yield return new WaitForSeconds(2);

            // Retrieve necessary components
            var vehicleData = Vehicle.Data;
            var kinematicsController = Vehicle.KinematicsController;
            var kinematicsStrategy = Vehicle.Config.KinematicStrategy as Forward_Kinematics_Strategy;
            Assert.IsNotNull(kinematicsStrategy, "KinematicStrategy is not Forward_Kinematics_Strategy.");

            // Retrieve the Path_Manager component
            var pathManager = GameObject.Find("Path Manager")?.GetComponentInChildren<Path_Manager>();
            Assert.IsNotNull(pathManager, "Path_Manager is null.");

            // Ensure there is at least one path to visualize
            Assert.IsTrue(pathManager.Paths.Count > 0, "No Reference_Path available for visualization.");
            var testPath = pathManager.Paths[0];

            // Retrieve the Path_Previewer component
            var pathPreviewer = GameObject.Find("Path Previewer")?.GetComponent<Path_Previewer>();
            Assert.IsNotNull(pathPreviewer, "Path_Previewer component not found in the scene.");

            pathPreviewer.DrawPath(testPath);


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

            // Invoke StartPathVisualization to render the path
            kinematicsController.InitPathVisualization(testPath);
            yield return new WaitForSeconds(2);

            Vehicle.VehicleDashboardController.TractorTrailToggle.onValueChanged.Invoke(true);
            kinematicsController.StartPathVisualization(testPath);

            float totalSimulationTime = testPath.Time[testPath.Time.Count - 1];
            yield return new WaitForSeconds(totalSimulationTime + 0.5f);

            var data = Vehicle.Data;
            Vector2 finalPoint2D = testPath.FrontAxle[testPath.FrontAxle.Count - 1];
            Vector3 finalExpectedPos = new Vector3(finalPoint2D.x, 0f, finalPoint2D.y);
            Vector3 actualPos = new Vector3(data.X1, 0f, data.Y1);

            float tolerance = 1f;
            Assert.IsTrue(Vector3.Distance(finalExpectedPos, actualPos) < tolerance,
                $"Final position mismatch. Expected: {finalExpectedPos}, Actual: {actualPos}");

            yield return new WaitForSeconds(2);
            Vehicle.VehicleDashboardController.TractorTrailToggle.onValueChanged.Invoke(false);
            
        }



    }
}



