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
using ApplicationScripts.VirtualEntity.Space.Controllers;

namespace Assets.Tests
{
    /// <summary>
    /// Play Mode tests for Simulation Services Functional Requirements.
    /// Specifically testing FR4: Vehicle Dynamics Simulation and FR5: Path Previewing.
    /// </summary>
    public class SpaceOccupancy : PlayModeTests
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
        public IEnumerator TestSpaceOccupancyDetection()
        {
            // Ensure the scene and everything is set up
            // This test assumes UnitySetUp has already created a space and a vehicle.

            Debug.Log("Testing: TestSpaceOccupancyDetection");

            // Retrieve a VE_Space instance
            var spaceObj = GameObject.Find("Spaces");
            Assert.IsNotNull(spaceObj, "No 'Spaces' GameObject found.");
            var space = spaceObj.GetComponentInChildren<VE_Space>();
            Assert.IsNotNull(space, "VE_Space not found under 'Spaces'.");

            var spaceController = space.GetComponent<Space_Controller>();
            Assert.IsNotNull(spaceController, "Space_Controller not found on VE_Space.");

            // Access the vehicle data
            var data = Vehicle.Data;
            Assert.IsNotNull(data, "Vehicle_Data is null.");

            // The polygon for the space is defined in space.Config.SpaceMarkings
            // Let's assume the space is centered around (0,0) or known area, and we know how to get in/out.

            // Move vehicle far away (outside space)
            float outsideX = 0;
            float outsideY = 110;
            Vehicle.KinematicsController.SetVehiclePosition(
                outsideX, 
                outsideY, 
                0f, 
                0f);
            yield return new WaitForSeconds(2); 

            // Check vehicle not inside space
            Assert.IsFalse(spaceController.IsVehicleInSpace(Vehicle.Id),
                "Vehicle should not be inside the space initially.");

            // Move vehicle inside space polygon
            // Assume the space polygon includes (0,0) for simplicity, or known coordinates inside polygon
            float insideX = space.SpaceData.X1;
            float insideY = space.SpaceData.Y1;

            Vehicle.KinematicsController.SetVehiclePosition(
                insideX, 
                insideY, 
                space.Config.Psi1Rad * Mathf.Rad2Deg, 
                space.Config.Psi2Rad * Mathf.Rad2Deg);
            yield return new WaitForSeconds(2); // Wait for space controller update

            // Verify vehicle now inside space
            Assert.IsTrue(spaceController.IsVehicleInSpace(Vehicle.Id),
                "Vehicle should be inside the space after moving inside polygon.");

            // Move vehicle outside again
            Vehicle.KinematicsController.SetVehiclePosition(
                outsideX, 
                outsideY, 
                0f, 
                0f);

            yield return new WaitForSeconds(2); // Wait for space controller update

            // Verify vehicle is no longer inside
            Assert.IsFalse(spaceController.IsVehicleInSpace(Vehicle.Id),
                "Vehicle should not be inside the space after moving out.");

            Debug.Log("TestSpaceOccupancyDetection completed successfully.");
            yield return null;
        }
    }
}



