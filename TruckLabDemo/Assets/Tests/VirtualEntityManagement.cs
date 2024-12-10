using System.Collections;
using ApplicationScripts.VirtualEntity.Building;
using ApplicationScripts.VirtualEntity.Building.Controllers;
using ApplicationScripts.VirtualEntity.Space;
using ApplicationScripts.VirtualEntity.Space.Controllers;
using ApplicationScripts.VirtualEntity.Vehicle;
using ApplicationScripts.VirtualEntity.Vehicle.Controllers;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Assets.Tests
{
    public class VirtualEntityManagement : PlayModeTests
    {
        #region Virtual Entities

        public VE_Building Building;
        public VE_Space Space;
        public VE_Vehicle Vehicle;

        public GameObject BuildingInstance;
        public GameObject SpaceInstance;
        public GameObject VehicleInstance;

        #endregion

        #region Test Methods

        /// <summary>
        /// Test to verify that a building can be created successfully.
        /// </summary>
        [UnityTest]
        public IEnumerator TestBuildingCreation()
        {
            // Wait until scene setup is complete
            while (!SceneSetupComplete)
            {
                yield return new WaitForSeconds(1);
            }

            // Invoke Create Building Button
            GameObject.Find("Create Building BTN").GetComponent<Button>().onClick.Invoke();

            // Retrieve Building component
            Building = GameObject.Find("Buildings").GetComponentInChildren<VE_Building>();
            BuildingInstance = Building.Instance;

            // Verify that the building instance exists
            Assert.IsNotNull(Building, "Building component is null.");
            Assert.IsNotNull(Building.Instance, "Building instance is null.");

            // Verify that the building is registered in the lookup table
            Assert.IsTrue(BuildingCreator.LookupTable.ContainsKey(Building.Id), "Building not registered in lookup table.");

            // Verify that Building_Controller component is attached to the building instance
            var buildingController = Building.Instance.GetComponent<Building_Controller>();
            Assert.IsNotNull(buildingController, "Building_Controller component is missing from Building instance.");

            // Verify that Building_Data component is attached and correctly configured
            var buildingData = Building.Instance.GetComponent<Building_Data>();
            Assert.IsNotNull(buildingData, "Building_Data component is not initialized.");

            // Verify that BuildingDashboardController is assigned and initialized
            var buildingDashboardController = GameObject.Find("Building Dashboards").GetComponentInChildren<Building_Dashboard_Controller>();
            Assert.IsNotNull(buildingDashboardController, "BuildingDashboardController is null.");
            Assert.IsNotNull(Building.BuildingDashboardController, "BuildingDashboardController reference is null.");
            Assert.AreSame(Building.BuildingDashboardController, buildingDashboardController, "BuildingDashboardController is not the same instance.");

            Debug.Log($"TestBuildingCreation finished on {Building.BuildingData.Config.Name}");

            yield return new WaitForSeconds(1);
        }

        /// <summary>
        /// Test to verify that a space can be created successfully.
        /// </summary>
        [UnityTest]
        public IEnumerator TestSpaceCreation()
        {
            // Wait until scene setup is complete
            while (!SceneSetupComplete)
            {
                yield return new WaitForSeconds(1);
            }


            // Invoke Create Space Button
            GameObject.Find("Create Space BTN").GetComponent<Button>().onClick.Invoke();

            // Retrieve Space component
            Space = GameObject.Find("Spaces").GetComponentInChildren<VE_Space>();
            SpaceInstance = Space.Instance;

            // Verify that the space was created successfully
            Assert.IsNotNull(Space, "Space component is null.");
            Assert.IsNotNull(Space.Instance, "Space instance is null.");

            // Verify that the space is registered in the lookup table
            Assert.IsTrue(SpaceCreator.LookupTable.ContainsKey(Space.Id), "Space not registered in lookup table.");

            // Verify that Space_Controller component is attached to the space instance
            var spaceController = Space.Instance.GetComponent<Space_Controller>();
            Assert.IsNotNull(spaceController, "Space_Controller component is missing from Space instance.");

            // Verify that Space_Data component is attached and correctly configured
            var spaceData = Space.Instance.GetComponent<Space_Data>();
            Assert.IsNotNull(spaceData, "Space_Data component is not initialized.");

            // Verify that SpaceDashboardController is assigned and initialized
            var spaceDashboardController = GameObject.Find("Space Dashboards").GetComponentInChildren<Space_Dashboard_Controller>();
            Assert.IsNotNull(spaceDashboardController, "SpaceDashboardController is null.");
            Assert.IsNotNull(Space.SpaceDashboardController, "SpaceDashboardController reference is null.");
            Assert.AreSame(Space.SpaceDashboardController, spaceDashboardController, "SpaceDashboardController is not the same instance.");

            // Verify SpaceMesh component
            Assert.IsNotNull(Space.SpaceMesh, "SpaceMesh component is null.");

            Debug.Log($"TestSpaceCreation finished on {Space.SpaceData.Config.Name}");

            yield return new WaitForSeconds(1);
        }

        /// <summary>
        /// Test to verify that a vehicle can be created successfully.
        /// </summary>
        [UnityTest]
        public IEnumerator TestVehicleCreation()
        {
            // Wait until scene setup is complete
            while (!SceneSetupComplete)
            {
                yield return new WaitForSeconds(1);
            }


            // Invoke Create Vehicle Button
            GameObject.Find("Create Vehicle BTN").GetComponent<Button>().onClick.Invoke();

            // Retrieve Vehicle component
            Vehicle = GameObject.Find("Vehicles").GetComponentInChildren<VE_Vehicle>();
            VehicleInstance = Vehicle.Instance;

            // Verify that the vehicle instance exists
            Assert.IsNotNull(Vehicle, "Vehicle component is null.");
            Assert.IsNotNull(Vehicle.Instance, "Vehicle instance is null.");

            // Verify that the vehicle is registered in the lookup table
            Assert.IsTrue(VehicleCreator.LookupTable.ContainsKey(Vehicle.Id), "Vehicle not registered in lookup table.");

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

            // Verify that Vehicle_Data component is attached and correctly configured
            var vehicleData = VehicleInstance.GetComponent<Vehicle_Data>();
            Assert.IsNotNull(vehicleData, "Vehicle_Data component is not initialized.");

            // Verify that 'Trailer' GameObject exists under the vehicle instance
            var trailerInstance = VehicleInstance.transform.Find("Trailer");
            Assert.IsNotNull(trailerInstance, "'Trailer' GameObject not found in Vehicle Instance.");

            // Verify that 'Truck Label' exists under 'Trailer'
            var truckLabelInstance = trailerInstance.Find("Truck Label");
            Assert.IsNotNull(truckLabelInstance, "'Truck Label' GameObject not found under 'Trailer'.");

            // Verify that 'Truck Label' has a TextMeshPro component
            var textMeshPro = truckLabelInstance.GetComponent<TextMeshPro>();
            Assert.IsNotNull(textMeshPro, "'Truck Label' does not have a TextMeshPro component.");

            // Verify that 'Sensor' exists under 'Trailer'
            var sensorInstance = VehicleInstance.transform.Find("Sensor");
            Assert.IsNotNull(sensorInstance, "'Sensor' GameObject not found under 'Trailer'.");

            // Verify that 'Sensor' has a SphereCollider component
            var sphereColliderInstance = sensorInstance.GetComponent<SphereCollider>();
            Assert.IsNotNull(sphereColliderInstance, "'Sensor' does not have a SphereCollider component.");

            // Verify that VehicleDashboardController is assigned and initialized
            var vehicleDashboardController = GameObject.Find("Vehicle Dashboards").GetComponentInChildren<Vehicle_Dashboard_Controller>();
            Assert.IsNotNull(vehicleDashboardController, "VehicleDashboardController is not assigned.");
            Assert.IsNotNull(Vehicle.VehicleDashboardController, "VehicleDashboardController reference is null.");
            Assert.AreSame(Vehicle.VehicleDashboardController, vehicleDashboardController, "VehicleDashboardController is not the same instance.");

            // Verify Input Strategies
            foreach (var strategy in Vehicle.Config.VehicleInputStrategiesDict.Values)
            {
                Assert.IsNotNull(strategy, $"Actuation Input Strategy '{strategy.StrategyName}' is not initialized.");
            }

            Debug.Log($"TestVehicleCreation finished on {Vehicle.Data.Config.Name}");

            yield return new WaitForSeconds(2);
        }

        /// <summary>
        /// Test to verify that all buildings can be deleted successfully.
        /// </summary>
        [UnityTest]
        public IEnumerator TestBuildingDeletion()
        {
            // Invoke Delete All Buildings Button
            GameObject.Find("Delete All Buildings BTN").GetComponent<Button>().onClick.Invoke();

            // Verify that the building is removed from the lookup table
            Assert.IsFalse(BuildingCreator.LookupTable.ContainsKey(Building.Id), "Building was not removed from lookup table.");

            // Verify that the GameObject is destroyed
            Assert.IsTrue(Building.Instance == null, "Building GameObject was not destroyed.");

            yield return new WaitForSeconds(1);
        }

        /// <summary>
        /// Test to verify that all spaces can be deleted successfully.
        /// </summary>
        [UnityTest]
        public IEnumerator TestSpaceDeletion()
        {
            // Invoke Delete All Spaces Button
            GameObject.Find("Delete All Spaces BTN").GetComponent<Button>().onClick.Invoke();

            // Verify that the space is removed from the lookup table
            Assert.IsFalse(SpaceCreator.LookupTable.ContainsKey(Space.Id), "Space was not removed from lookup table.");

            // Verify that the GameObject is destroyed
            Assert.IsTrue(Space.Instance == null, "Space GameObject was not destroyed.");

            yield return new WaitForSeconds(1);
        }

        /// <summary>
        /// Test to verify that all vehicles can be deleted successfully.
        /// </summary>
        [UnityTest]
        public IEnumerator TestVehicleDeletion()
        {
            // Invoke Delete All Vehicles Button
            GameObject.Find("Delete All Vehicles BTN").GetComponent<Button>().onClick.Invoke();

            // Verify that the vehicle is removed from the lookup table
            Assert.IsFalse(VehicleCreator.LookupTable.ContainsKey(Vehicle.Id), "Vehicle was not removed from lookup table.");

            // Verify that the GameObject is destroyed
            Assert.IsTrue(Vehicle.Instance == null, "Vehicle GameObject was not destroyed.");

            yield return new WaitForSeconds(1);
        }

        #endregion
    }
}


