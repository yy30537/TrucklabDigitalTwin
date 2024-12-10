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
    public class MiddlewareTests : PlayModeTests
    {

        public VE_Vehicle Vehicle;
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


            // Invoke Create Vehicle Button
            var vehicleDropDown = GameObject.Find("Vehicle Dropdown").GetComponent<TMP_Dropdown>();
            vehicleDropDown.value = 1;
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

            if (ServiceMenuToggle.isActiveAndEnabled)
            {
                ServiceMenuToggle.onValueChanged.Invoke(false);
                yield return null; // Allow UI to update
            }

            if (BuildingDashboardsToggle.isActiveAndEnabled)
            {
                BuildingDashboardsToggle.onValueChanged.Invoke(false);
            }

            if (SpaceDashboardsToggle.isActiveAndEnabled)
            {
                SpaceDashboardsToggle.onValueChanged.Invoke(false);
            }

            yield return new WaitForSeconds(2);
        }



        [UnityTest]
        public IEnumerator TestMiddlewareConnection()
        {
            var rosConnector = Vehicle.RosConnector;
            var activeRosTopicSubscriber = Vehicle.SharedTwistSubscriber;
            var activeRosTopicPublisher = Vehicle.TwistPublisher;

            //Assert.IsNotNull(activeRosTopicSubscriber.angularVelocity);
            //Assert.IsNotNull(activeRosTopicSubscriber.linearVelocity);
            //Assert.IsNotNull(activeRosTopicPublisher.x1);
            //Assert.IsNotNull(activeRosTopicPublisher.y1);

            var optitrackTractorRigidBody = Vehicle.TractorRigidBody;
            var optitrackTrailerRigidBody = Vehicle.TrailerRigidBody;
            //Assert.IsNotNull(optitrackTractorRigidBody.position);
            //Assert.IsNotNull(optitrackTractorRigidBody.rotation);
            //Assert.IsNotNull(optitrackTrailerRigidBody.position);
            //Assert.IsNotNull(optitrackTrailerRigidBody.rotation);

            yield return null;
        }


    }
}



