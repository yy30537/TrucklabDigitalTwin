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
using ApplicationScripts.UIController;

namespace Assets.Tests
{
    /// <summary>
    /// Play Mode tests for Simulation Services Functional Requirements.
    /// Specifically testing FR4: Vehicle Dynamics Simulation and FR5: Path Previewing.
    /// </summary>
    public class DraggableUiTests : PlayModeTests
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



            yield return new WaitForSeconds(2);
        }


        [UnityTest]
        public IEnumerator TestDraggableUiComponents()
        {
            // Locate various UI panels
            var serviceMenuUiGameObj = GameObject.Find("Service Menu");
            var systemLogMenuUiGameObj = GameObject.Find("System Log");
            var buildingDashboardsUiGameObj = GameObject.Find("Building Dashboards");
            var spaceDashboardsUiGameObj = GameObject.Find("Space Dashboards");
            var vehicleDashboardsUiGameObj = Vehicle.VehicleDashboardController.UiInstance;

            Assert.IsNotNull(serviceMenuUiGameObj, "Service Menu UI not found.");
            Assert.IsNotNull(systemLogMenuUiGameObj, "System Log UI not found.");
            Assert.IsNotNull(buildingDashboardsUiGameObj, "Building Dashboards UI not found.");
            Assert.IsNotNull(spaceDashboardsUiGameObj, "Space Dashboards UI not found.");
            Assert.IsNotNull(vehicleDashboardsUiGameObj, "Vehicle Dashboards UI not found.");

            // Retrieve their top bars
            var serviceMenuTopbar = serviceMenuUiGameObj.transform.Find("Top Bar");
            var systemLogMenuTopbar = systemLogMenuUiGameObj.transform.Find("Top Bar");
            var buildingDashboardsTopbar = buildingDashboardsUiGameObj.transform.Find("Top Bar");
            var spaceDashboardsTopbar = spaceDashboardsUiGameObj.transform.Find("Top Bar");
            var vehicleDashboardsTopbar = vehicleDashboardsUiGameObj.transform.Find("Top Bar");

            Assert.IsNotNull(serviceMenuTopbar, "Service Menu Top Bar not found.");
            Assert.IsNotNull(systemLogMenuTopbar, "System Log Top Bar not found.");
            Assert.IsNotNull(buildingDashboardsTopbar, "Building Dashboards Top Bar not found.");
            Assert.IsNotNull(spaceDashboardsTopbar, "Space Dashboards ToTop Barpbar not found.");
            Assert.IsNotNull(vehicleDashboardsTopbar, "Vehicle Dashboards Top Bar not found.");

            // Unpin vehicle dashboard if pinned
            Vehicle.VehicleDashboardController.PinDashboardToggle.onValueChanged.Invoke(false);
            Vehicle.VehicleDashboardController.PinDashboardToggle.isOn = false;

            // Helper function to test dragging a UI panel
            void TestDrag(GameObject uiPanel, Transform topbar)
            {
                var draggable = topbar.GetComponent<Draggable_UI>();
                Assert.IsNotNull(draggable, $"Draggable_UI component not found on {topbar.name}.");

                // Store original position
                Vector3 originalPosition = uiPanel.transform.position;

                // Create PointerEventData to simulate drag events
                var eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);

                // Simulate starting the drag
                // Let's say the mouse started at screen position (100,100)
                eventData.position = new Vector2(100, 100);
                draggable.OnBeginDrag(eventData);

                // Now simulate dragging to a new position (200,200)
                eventData.position = new Vector2(200, 200);
                draggable.OnDrag(eventData);

                // After dragging, the UiGameObject should have moved
                Vector3 newPosition = uiPanel.transform.position;
                Assert.AreNotEqual(originalPosition, newPosition, $"{uiPanel.name} did not move after drag simulation.");
                Debug.Log($"{uiPanel.name} successfully dragged from {originalPosition} to {newPosition}.");

                
            }

            // Test dragging each UI panel
            TestDrag(serviceMenuUiGameObj, serviceMenuTopbar);
            yield return new WaitForSeconds(1);
            TestDrag(systemLogMenuUiGameObj, systemLogMenuTopbar);
            yield return new WaitForSeconds(1);
            TestDrag(buildingDashboardsUiGameObj, buildingDashboardsTopbar);
            yield return new WaitForSeconds(1);
            TestDrag(spaceDashboardsUiGameObj, spaceDashboardsTopbar);
            yield return new WaitForSeconds(1);
            TestDrag(vehicleDashboardsUiGameObj, vehicleDashboardsTopbar);
            yield return new WaitForSeconds(1);
        }

    }
}



