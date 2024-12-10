using System.Threading.Tasks;
using ApplicationScripts.Manager;
using ApplicationScripts.VirtualEntity.Building;
using ApplicationScripts.VirtualEntity.Space;
using ApplicationScripts.VirtualEntity.Vehicle;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Tests
{
    public class PlayModeTests
    {
        #region Scene Management

        protected string OriginalScene;
        public Scene LoadedScene;

        #endregion

        #region GameObject References

        public GameObject CameraManagerGo;
        public Camera_Manager CameraManager;

        public GameObject MainMenuToggleGo;
        public Button MainMenuToggle;

        public Vehicle_Creator VehicleCreator;
        public Space_Creator SpaceCreator;
        public Building_Creator BuildingCreator;

        public GameObject ServiceMenuToggleGo;
        public Toggle ServiceMenuToggle;

        public GameObject VeCreatorMenuToggleGo;
        public Button VeCreatorMenuToggle;

        public GameObject PathSimulationMenuToggleGo;
        public Button PathSimulationMenuButton;

        public GameObject SystemLogToggleGo;
        public Toggle SystemLogToggle;

        public GameObject BuildingDashboardsToggleGo;
        public Toggle BuildingDashboardsToggle;

        public GameObject SpaceDashboardsToggleGo;
        public Toggle SpaceDashboardsToggle;

        public GameObject VehicleDashboardsToggleGo;
        public Toggle VehicleDashboardsToggle;

        #endregion

        #region Setup Completion Flag

        public bool SceneSetupComplete = false;

        #endregion

        #region Helper Methods

        /// <summary>
        /// Finds a GameObject by name and asserts its existence.
        /// </summary>
        /// <param name="name">Name of the GameObject to find.</param>
        /// <returns>The found GameObject.</returns>
        protected GameObject FindGameObject(string name)
        {
            var go = GameObject.Find(name);
            Assert.IsNotNull(go, $"GameObject '{name}' not found in the scene.");
            return go;
        }

        /// <summary>
        /// Retrieves a component of type T from a GameObject and asserts its existence and enabled state.
        /// </summary>
        /// <typeparam name="T">Type of the component.</typeparam>
        /// <param name="go">The GameObject from which to retrieve the component.</param>
        /// <param name="componentName">Name of the component for assertion messages.</param>
        /// <returns>The retrieved component.</returns>
        protected T GetComponentFromGameObject<T>(GameObject go, string componentName) where T : Behaviour
        {
            var component = go.GetComponent<T>();
            Assert.IsNotNull(component, $"{componentName} component is missing on GameObject '{go.name}'.");
            Assert.IsTrue(component.enabled, $"{componentName} component is disabled on GameObject '{go.name}'.");
            return component;
        }

        #endregion

        #region OneTimeSetUp and OneTimeTearDown

        [OneTimeSetUp]
        public async void OneTimeSetUp()
        {
            Debug.Log("PlayModeTests: OneTimeSetUp");

            // Save the original active scene
            OriginalScene = SceneManager.GetActiveScene().path;

            // Load the main scene additively
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Application Space", LoadSceneMode.Additive);
            if (loadOperation == null)
            {
                Assert.Fail("Failed to load scene 'Application Space'.");
            }

            // Wait until the scene is fully loaded
            while (!loadOperation.isDone)
            {
                await Task.Yield();
            }

            // Assign the loaded scene
            LoadedScene = SceneManager.GetSceneByName("Application Space");
            Assert.IsTrue(LoadedScene.isLoaded, "Loaded scene 'Application Space' is not active.");

            // Ensure all GameObjects are initialized
            await Task.Yield();

            #region Initialize GameObjects and Components

            // Initialize Vehicle Creator
            VehicleCreator = Object.FindObjectOfType<Vehicle_Creator>();
            Assert.IsNotNull(VehicleCreator, "Vehicle_Creator component not found in the scene.");
            Assert.IsTrue(VehicleCreator.enabled, "Vehicle_Creator component is disabled.");

            // Initialize Space Creator
            SpaceCreator = Object.FindObjectOfType<Space_Creator>();
            Assert.IsNotNull(SpaceCreator, "Space_Creator component not found in the scene.");
            Assert.IsTrue(SpaceCreator.enabled, "Space_Creator component is disabled.");

            // Initialize Building Creator
            BuildingCreator = Object.FindObjectOfType<Building_Creator>();
            Assert.IsNotNull(BuildingCreator, "Building_Creator component not found in the scene.");
            Assert.IsTrue(BuildingCreator.enabled, "Building_Creator component is disabled.");

            // Initialize Camera Manager
            CameraManagerGo = FindGameObject("Camera Manager");
            CameraManager = GetComponentFromGameObject<Camera_Manager>(CameraManagerGo, "Camera_Manager");

            // Initialize Main Menu Toggle
            MainMenuToggleGo = FindGameObject("Main Menu Toggle");
            MainMenuToggle = GetComponentFromGameObject<Button>(MainMenuToggleGo, "Button");
            MainMenuToggle.onClick.Invoke();

            // Initialize Service Menu Toggle
            ServiceMenuToggleGo = FindGameObject("Toggle Service Menu");
            ServiceMenuToggle = GetComponentFromGameObject<Toggle>(ServiceMenuToggleGo, "Toggle");
            ServiceMenuToggle.onValueChanged.Invoke(true);

            // Initialize Virtual Entity Creator Menu Toggle
            VeCreatorMenuToggleGo = FindGameObject("Toggle VE Creator Menu");
            VeCreatorMenuToggle = GetComponentFromGameObject<Button>(VeCreatorMenuToggleGo, "Button");
            VeCreatorMenuToggle.onClick.Invoke();

            // Initialize Path Simulation Menu Toggle
            PathSimulationMenuToggleGo = FindGameObject("Toggle Path Simulation Menu"); ;
            PathSimulationMenuButton = GetComponentFromGameObject<Button>(PathSimulationMenuToggleGo, "Button");

            // Initialize System Log Toggle
            SystemLogToggleGo = FindGameObject("Toggle System Log");
            SystemLogToggle = GetComponentFromGameObject<Toggle>(SystemLogToggleGo, "Toggle");
            SystemLogToggle.onValueChanged.Invoke(true);

            // Initialize Building Dashboards Toggle
            BuildingDashboardsToggleGo = FindGameObject("Toggle Building Dashboards");
            BuildingDashboardsToggle = GetComponentFromGameObject<Toggle>(BuildingDashboardsToggleGo, "Toggle");
            BuildingDashboardsToggle.onValueChanged.Invoke(true);

            // Initialize Space Dashboards Toggle
            SpaceDashboardsToggleGo = FindGameObject("Toggle Space Dashboards");
            SpaceDashboardsToggle = GetComponentFromGameObject<Toggle>(SpaceDashboardsToggleGo, "Toggle");
            SpaceDashboardsToggle.onValueChanged.Invoke(true);

            // Initialize Vehicle Dashboards Toggle
            VehicleDashboardsToggleGo = FindGameObject("Toggle Vehicle Dashboards");
            VehicleDashboardsToggle = GetComponentFromGameObject<Toggle>(VehicleDashboardsToggleGo, "Toggle");
            VehicleDashboardsToggle.onValueChanged.Invoke(true);

            #endregion

            // Mark setup as complete
            SceneSetupComplete = true;
        }

        [OneTimeTearDown]
        public async void OneTimeTearDown()
        {
            Debug.Log("PlayModeTests: OneTimeTearDown");

            // Unload the additively loaded scene
            if (LoadedScene.isLoaded)
            {
                AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(LoadedScene);
                if (unloadOperation != null)
                {
                    while (!unloadOperation.isDone)
                    {
                        await Task.Yield();
                    }
                }
            }

            // Reload the original scene
            AsyncOperation loadOriginal = SceneManager.LoadSceneAsync(OriginalScene, LoadSceneMode.Single);
            if (loadOriginal != null)
            {
                while (!loadOriginal.isDone)
                {
                    await Task.Yield();
                }
            }

            // Optional: Reset the setup flag
            SceneSetupComplete = false;
        }

        #endregion
    }
}
