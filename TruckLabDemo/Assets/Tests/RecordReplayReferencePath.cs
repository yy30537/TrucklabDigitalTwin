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
using System.IO;
using ApplicationScripts.UIController.ApplicationUI;

namespace Assets.Tests
{
    /// <summary>
    /// Play Mode tests for Simulation Services Functional Requirements.
    /// Specifically testing FR4: Vehicle Dynamics Simulation and FR5: Path Previewing.
    /// </summary>
    public class RecordReplayReferencePath : PlayModeTests
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


            yield return new WaitForSeconds(2);

        }
        [UnityTest]
        public IEnumerator TestPathRecordingAndReplaying()
        {
            Debug.Log("Testing: TestPathRecordingAndReplaying");
            PathSimulationMenuButton.onClick.Invoke();

            yield return new WaitForSeconds(2);

            Button startPathRecordBtn = GameObject.Find("Start Path Recording BTN").GetComponent<Button>();
            Button stopPathRecordBtn = GameObject.Find("Stop Path Recording BTN").GetComponent<Button>();
            Button startPathReplayBtn = GameObject.Find("Start Path Replaying BTN").GetComponent<Button>();
            Button stopPathReplayBtn = GameObject.Find("Stop Path Replaying BTN").GetComponent<Button>();



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


            // Retrieve necessary components
            var kinematicsStrategy = Vehicle.Config.KinematicStrategy as Forward_Kinematics_Strategy;
            Assert.IsNotNull(kinematicsStrategy, "KinematicStrategy is not Forward_Kinematics_Strategy.");



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
            bool strategyChanged = Vehicle.KinematicsController.ChangeInputStrategy(mockInputStrategy.StrategyName);
            Assert.IsTrue(strategyChanged, "Failed to change input strategy to mockInputStrategy.");

            mockInputStrategy.MockVelocity = 5f;
            mockInputStrategy.MockSteerAngle = -3 * Mathf.Deg2Rad;

            Vehicle.VehicleDashboardController.TractorTrailToggle.onValueChanged.Invoke(true);
            startPathRecordBtn.onClick.Invoke();
            yield return new WaitForSeconds(10);
            stopPathRecordBtn.onClick.Invoke();
            Vehicle.VehicleDashboardController.TractorTrailToggle.onValueChanged.Invoke(false);

            strategyChanged = Vehicle.KinematicsController.ChangeInputStrategy(noInputStrategy.StrategyName);
            Assert.IsTrue(strategyChanged, "Failed to change input strategy to noInputStrategy.");

            yield return new WaitForSeconds(2);

            var pathSimulationMenuController = GameObject.Find("Application UI Controllers").GetComponent<UI_Controller_PathSimulation>();

            int lastRecordedPathIndex = pathSimulationMenuController.PathManager.Paths.Count - 1;
            var SelectedReferencePath = pathSimulationMenuController.PathManager.Paths[lastRecordedPathIndex];
            pathSimulationMenuController.PathDropdown.value = lastRecordedPathIndex;
            pathSimulationMenuController.PathDropdown.onValueChanged.Invoke(lastRecordedPathIndex);

            Vehicle.KinematicsController.InitPathReplay(SelectedReferencePath);
            yield return new WaitForSeconds(2);

            startPathReplayBtn.onClick.Invoke();
            Vehicle.VehicleDashboardController.TractorTrailToggle.onValueChanged.Invoke(true);
            yield return new WaitForSeconds(10);
            stopPathReplayBtn.onClick.Invoke();
            Vehicle.VehicleDashboardController.TractorTrailToggle.onValueChanged.Invoke(false);


            Vector2 finalPoint2D = SelectedReferencePath.FrontAxle[SelectedReferencePath.FrontAxle.Count - 1];
            Vector3 finalExpectedPos = new Vector3(finalPoint2D.x, 0f, finalPoint2D.y);
            Vector3 actualPos = new Vector3(vehicleData.X1, 0f, vehicleData.Y1);

            float tolerance = 5f;
            Debug.Log($"Expected final position: {finalExpectedPos}, Actual: {actualPos}");
            Assert.IsTrue(Vector3.Distance(finalExpectedPos, actualPos) < tolerance,
                $"Final position mismatch. Expected: {finalExpectedPos}, Actual: {actualPos}");

            yield return new WaitForSeconds(5);


        }
    }
}



