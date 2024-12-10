//using System.Threading.Tasks;
//using ApplicationScripts.Manager;
//using ApplicationScripts.VirtualEntity.Building;
//using ApplicationScripts.VirtualEntity.Space;
//using ApplicationScripts.VirtualEntity.Vehicle;
//using NUnit.Framework;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//#if UNITY_EDITOR
//#endif


//namespace Assets.Tests
//{
//    public class PlayModeTests
//    {

//        protected string OriginalScene;
//        public Scene LoadedScene;

//        public GameObject CameraManagerGo;
//        public Camera_Manager CameraManager;

//        public GameObject MainMenuToggleGo;
//        public Button MainMenuToggle;

//        public Vehicle_Creator VehicleCreator;
//        public Space_Creator SpaceCreator;
//        public Building_Creator BuildingCreator;

//        public GameObject ServiceMenuToggleGo;
//        public Toggle ServiceMenuToggle;

//        public GameObject VeCreatorMenuToggleGo;
//        public Button VeCreatorMenuToggle;

//        public GameObject SystemLogToggleGo;
//        public Toggle SystemLogToggle;

//        public GameObject BuildingDashboardsToggleGo;
//        public Toggle BuildingDashboardsToggle;

//        public GameObject SpaceDashboardsToggleGo;
//        public Toggle SpaceDashboardsToggle;

//        public GameObject VehicleDashboardsToggleGo;
//        public Toggle VehicleDashboardsToggle;

//        public bool SceneSetupComplete = false;

//        [OneTimeSetUp]
//        public async void OneTimeSetUp()
//        {
//            Debug.Log("PlayModeTests: OneTimeSetUp");

//            // Save the original active scene
//            OriginalScene = SceneManager.GetActiveScene().path;

//            // Load the main scene additively
//            AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Application Space", LoadSceneMode.Additive);
//            if (loadOperation == null)
//            {
//                Assert.Fail("Failed to load scene Application Space");
//            }

//            // Wait until the scene is fully loaded
//            while (!loadOperation.isDone)
//            {
//                await Task.Yield();
//            }

//            // Assign the loaded scene
//            LoadedScene = SceneManager.GetSceneByName("Application Space");

//            // Ensure all GameObjects are initialized
//            await Task.Yield();

//            // List all active GameObjects for verification
//            //foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
//            //{
//            //    Debug.Log("Active GameObject: " + obj.name);
//            //}

//            // Vehicle creator
//            VehicleCreator = Object.FindObjectOfType<Vehicle_Creator>();
//            Assert.IsNotNull(VehicleCreator, "Vehicle_Creator component not found in the scene.");
//            Assert.IsTrue(VehicleCreator.enabled, "Vehicle_Creator component is disabled.");

//            // Space creator
//            SpaceCreator = Object.FindObjectOfType<Space_Creator>();
//            Assert.IsNotNull(SpaceCreator, "Space_Creator component not found in the scene.");
//            Assert.IsTrue(SpaceCreator.enabled, "Space_Creator component is disabled.");

//            // Building creator
//            BuildingCreator = Object.FindObjectOfType<Building_Creator>();
//            Assert.IsNotNull(BuildingCreator, "Building_Creator component not found in the scene.");
//            Assert.IsTrue(BuildingCreator.enabled, "Building_Creator component is disabled.");

//            // Camera manager
//            CameraManagerGo = GameObject.Find("Camera Manager");
//            Assert.IsNotNull(CameraManagerGo, "Camera Manager GameObject not found in the scene.");
//            CameraManager = CameraManagerGo.GetComponent<Camera_Manager>();
//            Assert.IsNotNull(CameraManager, "Camera_Manager component is missing on the Camera Manager GameObject.");
//            Assert.IsTrue(CameraManager.enabled, "Camera_Manager component is disabled.");

//            // Main menu
//            MainMenuToggleGo = GameObject.Find("Main Menu Toggle");
//            Assert.IsNotNull(MainMenuToggleGo, "Main Menu Toggle GameObject not found in the scene.");
//            MainMenuToggle = MainMenuToggleGo.GetComponent<Button>();
//            Assert.IsNotNull(MainMenuToggle, "MainMenuToggle missing");
//            Assert.IsTrue(MainMenuToggle.enabled, "MainMenuToggle component is disabled.");

//            MainMenuToggle.onClick.Invoke();

//            // Service menu
//            ServiceMenuToggleGo = GameObject.Find("Toggle Service Menu");
//            Assert.IsNotNull(ServiceMenuToggleGo, "Toggle Service Menu GameObject not found in the scene.");
//            ServiceMenuToggle = ServiceMenuToggleGo.GetComponent<Toggle>();
//            Assert.IsNotNull(ServiceMenuToggle, "ServiceMenuToggle missing");
//            Assert.IsTrue(ServiceMenuToggle.enabled, "ServiceMenuToggle component is disabled.");

//            ServiceMenuToggle.onValueChanged.Invoke(true);

//            // Virtual entity creator menu
//            VeCreatorMenuToggleGo = GameObject.Find("Toggle VE Creator Menu");
//            Assert.IsNotNull(VeCreatorMenuToggleGo, "Toggle VE Creator Menu GameObject not found in the scene.");
//            VeCreatorMenuToggle = VeCreatorMenuToggleGo.GetComponent<Button>();
//            Assert.IsNotNull(VeCreatorMenuToggle, "VeCreatorMenuToggle missing");
//            Assert.IsTrue(VeCreatorMenuToggle.enabled, "VeCreatorMenuToggle component is disabled.");

//            VeCreatorMenuToggle.onClick.Invoke();

//            // System Log
//            SystemLogToggleGo = GameObject.Find("Toggle System Log");
//            Assert.IsNotNull(SystemLogToggleGo, "Toggle System Log GameObject not found in the scene.");
//            SystemLogToggle = SystemLogToggleGo.GetComponent<Toggle>();
//            Assert.IsNotNull(SystemLogToggle, "Toggle System Log missing");
//            Assert.IsTrue(SystemLogToggle.enabled, "Toggle System Log component is disabled.");

//            SystemLogToggle.onValueChanged.Invoke(true);

//            // Building Dashboards
//            BuildingDashboardsToggleGo = GameObject.Find("Toggle Building Dashboards");
//            Assert.IsNotNull(BuildingDashboardsToggleGo, "Toggle Building Dashboards GameObject not found in the scene.");
//            BuildingDashboardsToggle = BuildingDashboardsToggleGo.GetComponent<Toggle>();
//            Assert.IsNotNull(BuildingDashboardsToggle, "Toggle Building Dashboards missing");
//            Assert.IsTrue(BuildingDashboardsToggle.enabled, "Toggle Building Dashboards component is disabled.");

//            BuildingDashboardsToggle.onValueChanged.Invoke(true);

//            // Space Dashboards
//            SpaceDashboardsToggleGo = GameObject.Find("Toggle Space Dashboards");
//            Assert.IsNotNull(SpaceDashboardsToggleGo, "Toggle Space Dashboards GameObject not found in the scene.");
//            SpaceDashboardsToggle = SpaceDashboardsToggleGo.GetComponent<Toggle>();
//            Assert.IsNotNull(SystemLogToggle, "Toggle Space Dashboards missing");
//            Assert.IsTrue(SystemLogToggle.enabled, "Toggle Space Dashboards component is disabled.");

//            SpaceDashboardsToggle.onValueChanged.Invoke(true);

//            // Vehicle Dashboards
//            VehicleDashboardsToggleGo = GameObject.Find("Toggle Vehicle Dashboards");
//            Assert.IsNotNull(VehicleDashboardsToggleGo, "Toggle Vehicle Dashboards GameObject not found in the scene.");
//            VehicleDashboardsToggle = VehicleDashboardsToggleGo.GetComponent<Toggle>();
//            Assert.IsNotNull(SystemLogToggle, "Toggle Vehicle Dashboards missing");
//            Assert.IsTrue(SystemLogToggle.enabled, "Toggle Vehicle Dashboards component is disabled.");

//            VehicleDashboardsToggle.onValueChanged.Invoke(true);


//            SceneSetupComplete = true;
//        }

//        [OneTimeTearDown]
//        public async void OneTimeTearDown()
//        {
//            Debug.Log("PlayModeTests: OneTimeTearDown");

//            // Unload the additively loaded scene
//            if (LoadedScene.isLoaded)
//            {
//                AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(LoadedScene);
//                if (unloadOperation != null)
//                {
//                    while (!unloadOperation.isDone)
//                    {
//                        await Task.Yield();
//                    }
//                }
//            }

//            // Reload the original scene
//            SceneManager.LoadScene(OriginalScene);

//            await Task.Yield();
//        }

//    }
//}

