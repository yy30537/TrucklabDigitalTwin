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
using ApplicationScripts.Manager;
using System.Collections.Generic;

namespace Assets.Tests
{
    /// <summary>
    /// Play Mode tests for Simulation Services Functional Requirements.
    /// Specifically testing FR4: Vehicle Dynamics Simulation and FR5: Path Previewing.
    /// </summary>
    public class CameraControlTest : PlayModeTests
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
        public IEnumerator TestCameraControl()
        {
            // Locate the camera manager and its dropdown
            var cameraManagerObj = GameObject.Find("Camera Manager");
            Assert.IsNotNull(cameraManagerObj, "Camera Manager not found in the scene.");

            var cameraManager = cameraManagerObj.GetComponent<Camera_Manager>();
            Assert.IsNotNull(cameraManager, "Camera_Manager component not found.");

            var camerasDropDownObj = GameObject.Find("Camera Dropdown");
            Assert.IsNotNull(camerasDropDownObj, "Camera Dropdown UI not found.");

            var camerasDropDown = camerasDropDownObj.GetComponent<TMP_Dropdown>();
            Assert.IsNotNull(camerasDropDown, "TMP_Dropdown component not found on 'Camera Dropdown' object.");

            // Verify initial active camera is Main Camera
            Assert.AreEqual("Main Camera", camerasDropDown.options[camerasDropDown.value].text, "Initial camera is not the Main Camera.");
            Assert.IsTrue(cameraManager.MainCamera.enabled, "Main Camera should be enabled initially.");

            yield return null;

            // Iterate over all camera options and switch to each one, verifying correctness
            for (int i = 0; i < camerasDropDown.options.Count; i++)
            {
                var cameraName = camerasDropDown.options[i].text;
                camerasDropDown.value = i;
                camerasDropDown.onValueChanged.Invoke(i);
                yield return new WaitForSeconds(1); // Allow time for camera to switch

                // Verify that the correct camera is active
                Assert.AreEqual(cameraName, camerasDropDown.options[camerasDropDown.value].text,
                    $"Expected camera '{cameraName}' but got '{camerasDropDown.options[camerasDropDown.value].text}'.");

                Camera expectedCamera;
                var success = TryGetCameraByName(cameraManager, cameraName, out expectedCamera);
                Assert.IsTrue(success, $"Camera '{cameraName}' not found in cameraManager dictionary.");

                // Check that only the expected camera is enabled
                foreach (var camEntry in GetAllCameras(cameraManager))
                {
                    if (camEntry.Value == expectedCamera)
                        Assert.IsTrue(camEntry.Value.enabled, $"{cameraName} should be enabled.");
                    else
                        Assert.IsFalse(camEntry.Value.enabled, $"Camera {camEntry.Key} should be disabled.");
                }
            }

            yield return null;

            // Test enabling free-look mode
            var menuToggleObj = GameObject.Find("Toggle Free Look"); // Adjust name if needed
            Assert.IsNotNull(menuToggleObj, "FreeLook Menu Toggle not found. Adjust the name to match the actual UI element.");
            var menuToggle = menuToggleObj.GetComponent<Toggle>();
            Assert.IsNotNull(menuToggle, "Toggle component for FreeLook not found.");

            // Switch back to Main Camera for testing free look movement
            int mainCamIndex = camerasDropDown.options.FindIndex(o => o.text == "Main Camera");
            Assert.IsTrue(mainCamIndex >= 0, "Main Camera option not found in the dropdown.");
            camerasDropDown.value = mainCamIndex;
            camerasDropDown.onValueChanged.Invoke(mainCamIndex);
            yield return new WaitForSeconds(1);

            // Enable free look
            menuToggle.isOn = true;
            menuToggle.onValueChanged.Invoke(true);
            yield return new WaitForSeconds(1);

            // Verify IsControlActive is now true
            Assert.IsTrue(cameraManager.IsControlActive, "Free look was not activated.");

            // Simulate camera movement by directly modifying target fields via reflection
            // Get private fields targetPosition and targetRotation
            var targetPosField = typeof(Camera_Manager).GetField("targetPosition", BindingFlags.NonPublic | BindingFlags.Instance);
            var targetRotField = typeof(Camera_Manager).GetField("targetRotation", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(targetPosField, "targetPosition field not found via reflection.");
            Assert.IsNotNull(targetRotField, "targetRotation field not found via reflection.");

            // Set a new position and rotation for the main camera
            Vector3 testPosition = cameraManager.MainCamera.transform.position + new Vector3(10, 10, 10);
            Quaternion testRotation = Quaternion.Euler(30, 45, 0);

            targetPosField.SetValue(cameraManager, testPosition);
            targetRotField.SetValue(cameraManager, testRotation);

            // Wait a few frames to let the camera interpolate
            yield return new WaitForSeconds(2);

            // Now check if the camera has moved close to the test position and rotation
            float positionTolerance = 1f;
            float rotationTolerance = 5f; // degrees

            Vector3 currentPos = cameraManager.MainCamera.transform.position;
            Assert.IsTrue(Vector3.Distance(currentPos, testPosition) < positionTolerance,
                $"Camera did not move towards the target position. Current: {currentPos}, Target: {testPosition}");

            Vector3 currentAngles = cameraManager.MainCamera.transform.rotation.eulerAngles;
            Vector3 testAngles = testRotation.eulerAngles;
            Assert.IsTrue(Mathf.Abs(Mathf.DeltaAngle(currentAngles.x, testAngles.x)) < rotationTolerance &&
                          Mathf.Abs(Mathf.DeltaAngle(currentAngles.y, testAngles.y)) < rotationTolerance,
                $"Camera did not rotate towards the target rotation. Current: {currentAngles}, Target: {testAngles}");

            // Disable free look again
            menuToggle.isOn = false;
            menuToggle.onValueChanged.Invoke(false);
            yield return new WaitForSeconds(1);

            // Verify IsControlActive is now false
            Assert.IsFalse(cameraManager.IsControlActive, "Free look was not deactivated.");

            Debug.Log("TestCameraControl completed successfully.");
        }


        // Helper methods
        private bool TryGetCameraByName(Camera_Manager manager, string cameraName, out Camera camera)
        {
            var cameraDropdown = manager.CameraDropdown;
            if (cameraDropdown == null)
            {
                camera = null;
                return false;
            }

            // We know cameraName matches exactly how the manager stored it
            // We'll try to guess from known fields or reflection
            var cams = GetAllCameras(manager);
            return cams.TryGetValue(cameraName, out camera);
        }

        private Dictionary<string, Camera> GetAllCameras(Camera_Manager manager)
        {
            // Extract the private 'allCameras' dictionary via reflection
            var field = typeof(Camera_Manager).GetField("allCameras", BindingFlags.NonPublic | BindingFlags.Instance);
            var dict = (Dictionary<string, Camera>)field.GetValue(manager);
            return dict;
        }


    }
}



