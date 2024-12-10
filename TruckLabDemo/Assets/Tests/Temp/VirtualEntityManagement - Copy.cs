//using System.Collections;
//using ApplicationScripts.VirtualEntity.Building;
//using ApplicationScripts.VirtualEntity.Building.Controllers;
//using ApplicationScripts.VirtualEntity.Space;
//using ApplicationScripts.VirtualEntity.Space.Controllers;
//using ApplicationScripts.VirtualEntity.Vehicle;
//using ApplicationScripts.VirtualEntity.Vehicle.Controllers;
//using NUnit.Framework;
//using TMPro;
//using UnityEngine;
//using UnityEngine.TestTools;
//using UnityEngine.UI;
//#if UNITY_EDITOR
//#endif


//namespace Assets.Tests
//{
//    public class VirtualEntityManagement : PlayModeTests
//    {
//        public VE_Building Building;
//        public VE_Space Space;
//        public VE_Vehicle Vehicle;

//        public GameObject BuildingInstance;
//        public GameObject SpaceInstance;
//        public GameObject VehicleInstance;

//        [UnityTest]
//        public IEnumerator TestBuildingCreation()
//        {
//            while (!SceneSetupComplete)
//            {
//                yield return new WaitForSeconds(1);
//            }
//            if (!MainMenuToggle.enabled) MainMenuToggle.onClick.Invoke();
//            if (!VeCreatorMenuToggle.enabled) VeCreatorMenuToggle.onClick.Invoke();


//            GameObject.Find("Create Building BTN").GetComponent<Button>().onClick.Invoke();
//            Building = GameObject.Find("Buildings").GetComponentInChildren<VE_Building>();
//            BuildingInstance = Building.Instance;

//            // Verify that the building instance exists
//            Assert.IsNotNull(Building, "Building is null");
//            Assert.IsNotNull(Building.Instance, "Building instance is null");

//            // Verify that the building is registered in the lookup table
//            Assert.IsTrue(BuildingCreator.LookupTable.ContainsKey(Building.Id), "Building not registered in lookup table");

//            // Verify that Building_Controller component is attached to the building instance
//            var buildingController = Building.Instance.GetComponent<Building_Controller>();
//            Assert.IsNotNull(buildingController, "Building_Controller component is missing from Building instance");

//            // Verify that Building_Data component is attached and correctly configured
//            var buildingData = Building.Instance.GetComponent<Building_Data>();
//            Assert.IsNotNull(buildingData, "BuildingData is not initialized");

//            // Verify that BuildingDashboardController is assigned and initialized
//            var buildingDashboardController = GameObject.Find("Building Dashboards").GetComponentInChildren<Building_Dashboard_Controller>();
            
//            Assert.IsNotNull(buildingDashboardController, "BuildingDashboardController is null");
//            Assert.IsNotNull(Building.BuildingDashboardController, "BuildingDashboardController is null");
//            Assert.AreSame(Building.BuildingDashboardController, buildingDashboardController);

//            Debug.Log($"TestBuildingCreation finished on {Building.BuildingData.Config.Name}");

//            yield return new WaitForSeconds(1);
//        }

//        [UnityTest]
//        public IEnumerator TestSpaceCreation()
//        {
//            while (!SceneSetupComplete)
//            {
//                yield return new WaitForSeconds(1);
//            }
//            if (!MainMenuToggle.enabled) MainMenuToggle.onClick.Invoke();
//            if (!VeCreatorMenuToggle.enabled) VeCreatorMenuToggle.onClick.Invoke();


//            GameObject.Find("Create Space BTN").GetComponent<Button>().onClick.Invoke();
//            Space = GameObject.Find("Spaces").GetComponentInChildren<VE_Space>();
//            SpaceInstance = Space.Instance;

//            // Verify that the space was created successfully
//            Assert.IsNotNull(Space, "Space creation failed");
//            Assert.IsNotNull(SpaceInstance, "Space creation failed");

//            // Verify that the space is registered in the lookup table
//            Assert.IsTrue(SpaceCreator.LookupTable.ContainsKey(Space.Id), "Space not registered in lookup table");

//            // Verify that Space_Controller component is attached to the space instance
//            var spaceController = Space.Instance.GetComponent<Space_Controller>();
//            Assert.IsNotNull(spaceController, "Space_Controller component is missing from Space instance");

//            // Verify that Space_Data component is attached and correctly configured
//            var spaceData = Space.Instance.GetComponent<Space_Data>();
//            Assert.IsNotNull(spaceData, "SpaceData is not initialized");

//            // Verify that BuildingDashboardController is assigned and initialized
//            var spaceDashboardController = GameObject.Find("Space Dashboards").GetComponentInChildren<Space_Dashboard_Controller>();

//            Assert.IsNotNull(spaceDashboardController, "SpaceDashboardController is not assigned");
//            Assert.IsNotNull(Space.SpaceDashboardController, "SpaceDashboardController is not assigned");
//            Assert.AreSame(Space.SpaceDashboardController, spaceDashboardController);

//            // Verify SpaceMesh component
//            Assert.IsNotNull(Space.SpaceMesh, "Space mesh is null");

//            Debug.Log($"TestSpaceCreation finished on {Space.SpaceData.Config.Name}");

//            yield return new WaitForSeconds(1);
//        }

//        [UnityTest]
//        public IEnumerator TestVehicleCreation()
//        {
//            while (!SceneSetupComplete)
//            {
//                yield return new WaitForSeconds(1);
//            }
//            if (!MainMenuToggle.enabled) MainMenuToggle.onClick.Invoke();
//            if (!VeCreatorMenuToggle.enabled) VeCreatorMenuToggle.onClick.Invoke();

//            GameObject.Find("Create Vehicle BTN").GetComponent<Button>().onClick.Invoke();

//            Vehicle = GameObject.Find("Vehicles").GetComponentInChildren<VE_Vehicle>();
//            VehicleInstance = Vehicle.Instance;

//            // Verify that the vehicle instance exists
//            Assert.IsNotNull(Vehicle, "Vehicle is null");
//            Assert.IsNotNull(Vehicle.Instance, "Vehicle instance is null");

//            // Verify that the vehicle is registered in the lookup table
//            Assert.IsTrue(VehicleCreator.LookupTable.ContainsKey(Vehicle.Id), "Vehicle not registered in lookup table");

            
            
//            // Verify that KinematicsController component is attached and active
//            var kinematicsController = Vehicle.KinematicsController;
//            Assert.IsNotNull(kinematicsController, "KinematicsController component is missing from Vehicle instance");
//            Assert.IsTrue(kinematicsController.IsActive, "KinematicsController is not active");

//            // Verify that AnimationController component is attached and active
//            var animationController = Vehicle.AnimationController;
//            Assert.IsNotNull(animationController, "AnimationController component is missing from Vehicle instance");
//            Assert.IsTrue(animationController.IsActive, "AnimationController is not active");

//            // Verify that CollisionController component is attached and active
//            var collisionController = Vehicle.CollisionController;
//            Assert.IsNotNull(collisionController, "CollisionController component is missing from Vehicle instance");
//            Assert.IsTrue(collisionController.IsActive, "CollisionController is not active");

//            // Verify that Vehicle_Data component is attached and correctly configured
//            var vehicleData = VehicleInstance.GetComponent<Vehicle_Data>();
//            Assert.IsNotNull(vehicleData, "Vehicle_Data is not initialized");

//            // Verify that 'Trailer' GameObject exists under the vehicle instance
//            var trailerInstance = VehicleInstance.transform.Find("Trailer");
//            Assert.IsNotNull(trailerInstance, "'Trailer' GameObject not found in Vehicle Instance");

//            // Verify that 'Truck Label' exists under 'Trailer'
//            var truckLabelInstance = trailerInstance.Find("Truck Label");
//            Assert.IsNotNull(truckLabelInstance, "'Truck Label' GameObject not found under 'Trailer'");

//            // Verify that 'Truck Label' has a TextMeshPro component
//            var textMeshPro = truckLabelInstance.GetComponent<TextMeshPro>();
//            Assert.IsNotNull(textMeshPro, "'Truck Label' does not have a TextMeshPro component");

//            // Verify that 'Sensor' exists under 'Trailer'
//            var sensorInstance = VehicleInstance.transform.Find("Sensor");
//            Assert.IsNotNull(sensorInstance, "'Sensor' GameObject not found under 'Trailer'");

//            // Verify that 'Sensor' has a SphereCollider component
//            var sphereColliderInstance = sensorInstance.GetComponent<SphereCollider>();
//            Assert.IsNotNull(sphereColliderInstance, "'Sensor' does not have a SphereCollider component");

//            // Verify that VehicleDashboardController is assigned and initialized
//            var vehicleDashboardController = GameObject.Find("Vehicle Dashboards").GetComponentInChildren<Vehicle_Dashboard_Controller>();

//            Assert.IsNotNull(vehicleDashboardController, "VehicleDashboardController is not assigned");
//            Assert.IsNotNull(Vehicle.VehicleDashboardController, "VehicleDashboardController is not assigned");
//            Assert.AreSame(Vehicle.VehicleDashboardController, vehicleDashboardController);

//            // Verify Input Strategies
//            foreach (var strategy in Vehicle.Config.VehicleInputStrategiesDict.Values)
//            {
//                Assert.IsNotNull(strategy, $"Actuation Input Strategy '{strategy.StrategyName}' is not initialized");
//            }

//            Debug.Log($"TestVehicleCreation on {Vehicle.Data.Config.Name}");

//            yield return new WaitForSeconds(5);
//        }

//        [UnityTest]
//        public IEnumerator TestBuildingDeletion()
//        {
//            GameObject.Find("Delete All Buildings BTN").GetComponent<Button>().onClick.Invoke();
//            // Verify that the building is removed from the lookup table
//            Assert.IsFalse(BuildingCreator.LookupTable.ContainsKey(Building.Id), "Building was not removed from lookup table");

//            // Verify that the GameObject is destroyed
//            Assert.IsTrue(Building.Instance == null, "Building GameObject was not destroyed");
//            yield return null; // Ensure all assertions are processed
//        }

//        [UnityTest]
//        public IEnumerator TestSpaceDeletion()
//        {
//            GameObject.Find("Delete All Spaces BTN").GetComponent<Button>().onClick.Invoke();
//            Assert.IsFalse(SpaceCreator.LookupTable.ContainsKey(Space.Id), "Space was not removed from lookup table");
//            Assert.IsTrue(Space.Instance == null, "Space GameObject was not destroyed");
//            yield return null; // Ensure all assertions are processed 
//        }

//        [UnityTest]
//        public IEnumerator TestVehicleDeletion()
//        {
//            GameObject.Find("Delete All Vehicles BTN").GetComponent<Button>().onClick.Invoke();
//            Assert.IsFalse(VehicleCreator.LookupTable.ContainsKey(Vehicle.Id), "Vehicle was not removed from lookup table");
//            Assert.IsTrue(Vehicle.Instance == null, "Vehicle GameObject was not destroyed");
//            yield return null; // Ensure all assertions are processed 
//        }

//    }
//}


