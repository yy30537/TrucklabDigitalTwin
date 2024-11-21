using System.Collections.Generic;
using Application_Scripts.Event_Channel;
using Application_Scripts.Manager;
using Application_Scripts.Virtual_Entity.Building;
using Application_Scripts.Virtual_Entity.Space;
using Application_Scripts.Virtual_Entity.Vehicle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Application_Scripts.UI_Controller.Application_UI
{
    /// <summary>
    /// Manages the creator menu for creating, deleting, and displaying information about vehicles, spaces, and buildings.
    /// Handles dropdown population, prefab preview rendering, and event-driven creation and deletion operations.
    /// </summary>
    public class UI_Controller_VE_Creator : Base_UI_Controller
    {
        /// <summary>
        /// Displays detailed information about the selected vehicle, space, or building.
        /// </summary>
        [Header("===Children: UI_Controller_VE_Creator===")]
        [Header("UI Objects")]
        public TextMeshProUGUI MenuText;

        /// <summary>
        /// Dropdown menu for selecting vehicles.
        /// </summary>
        public TMP_Dropdown VehicleDropdown;

        /// <summary>
        /// Dropdown menu for selecting spaces.
        /// </summary>
        public TMP_Dropdown SpaceDropdown;

        /// <summary>
        /// Dropdown menu for selecting buildings.
        /// </summary>
        public TMP_Dropdown BuildingDropdown;

        /// <summary>
        /// Image used for displaying a rendered preview of the selected prefab.
        /// </summary>
        [Header("Prefab Display Components")]
        public RawImage PrefabDisplayImage;

        /// <summary>
        /// Camera used for rendering the prefab preview.
        /// </summary>
        public Camera PrefabDisplayCamera;

        /// <summary>
        /// Currently displayed prefab instance in the preview.
        /// </summary>
        public GameObject CurrentPreviewInstance;

        /// <summary>
        /// Camera manager for managing camera operations.
        /// </summary>
        [Header("Application Dependencies")]
        public Camera_Manager CameraManager;

        /// <summary>
        /// RenderTexture used by the prefab display camera.
        /// </summary>
        public RenderTexture PrefabRenderTexture;

        /// <summary>
        /// Manages vehicle creation and deletion.
        /// </summary>
        [Header("VE Creators")]
        public Vehicle_Creator VehicleCreator;

        /// <summary>
        /// Manages space creation and deletion.
        /// </summary>
        public Space_Creator SpaceCreator;

        /// <summary>
        /// Manages building creation and deletion.
        /// </summary>
        public Building_Creator BuildingCreator;

        /// <summary>
        /// List of available vehicle configurations.
        /// </summary>
        [Header("VE Configurations")]
        public List<Vehicle_Config> VehicleConfigs;

        /// <summary>
        /// List of available space configurations.
        /// </summary>
        public List<Space_Config> SpaceConfigs;

        /// <summary>
        /// List of available building configurations.
        /// </summary>
        public List<Building_Config> BuildingConfigs;

        private Vehicle_Config selectedVehicleConfig;
        private Space_Config selectedSpaceConfig;
        private Building_Config selectedBuildingConfig;

        /// <summary>
        /// Event channel for confirming vehicle creation.
        /// </summary>
        [Header("Event Channels")]
        public EventChannel_Void ConfirmVehicleCreationEvent;

        /// <summary>
        /// Event channel for deleting a vehicle.
        /// </summary>
        public EventChannel_Void DeleteVehicleEvent;

        /// <summary>
        /// Event channel for confirming the creation of all vehicles.
        /// </summary>
        public EventChannel_Void ConfirmAllVehicleCreationEvent;

        /// <summary>
        /// Event channel for deleting all vehicles.
        /// </summary>
        public EventChannel_Void DeleteAllVehicleEvent;

        /// <summary>
        /// Event channel for confirming space creation.
        /// </summary>
        public EventChannel_Void ConfirmSpaceCreationEvent;

        /// <summary>
        /// Event channel for deleting a space.
        /// </summary>
        public EventChannel_Void DeleteSpaceEvent;

        /// <summary>
        /// Event channel for confirming the creation of all spaces.
        /// </summary>
        public EventChannel_Void ConfirmAllSpaceCreationEvent;

        /// <summary>
        /// Event channel for deleting all spaces.
        /// </summary>
        public EventChannel_Void DeleteAllSpaceEvent;

        /// <summary>
        /// Event channel for confirming building creation.
        /// </summary>
        public EventChannel_Void ConfirmBuildingCreationEvent;

        /// <summary>
        /// Event channel for deleting a building.
        /// </summary>
        public EventChannel_Void DeleteBuildingEvent;

        /// <summary>
        /// Event channel for confirming the creation of all buildings.
        /// </summary>
        public EventChannel_Void ConfirmAllBuildingCreationEvent;

        /// <summary>
        /// Event channel for deleting all buildings.
        /// </summary>
        public EventChannel_Void DeleteAllBuildingEvent;

        /// <summary>
        /// Initializes the menu and subscribes to event channels for managing vehicles, spaces, and buildings.
        /// </summary>
        private void Start()
        {
            Init();

            UiNavigationEventChannel.OnEventRaised += OnUiNavigation;
            ConfirmBuildingCreationEvent.OnEventRaised += OnCreateBuilding;
            DeleteBuildingEvent.OnEventRaised += OnDeleteBuilding;

            ConfirmSpaceCreationEvent.OnEventRaised += OnCreateSpace;
            DeleteSpaceEvent.OnEventRaised += OnDeleteSpace;

            ConfirmVehicleCreationEvent.OnEventRaised += OnCreateVehicle;
            DeleteVehicleEvent.OnEventRaised += OnDeleteVehicle;

            ConfirmAllBuildingCreationEvent.OnEventRaised += OnCreateAllBuildings;
            DeleteAllBuildingEvent.OnEventRaised += OnDeleteAllBuildings;

            ConfirmAllSpaceCreationEvent.OnEventRaised += OnCreateAllSpaces;
            DeleteAllSpaceEvent.OnEventRaised += OnDeleteAllSpaces;

            ConfirmAllVehicleCreationEvent.OnEventRaised += OnCreateAllVehicles;
            DeleteAllVehicleEvent.OnEventRaised += OnDeleteAllVehicles;
        }

        /// <summary>
        /// Rotates the currently displayed prefab instance if it exists.
        /// </summary>
        private void Update()
        {
            if (CurrentPreviewInstance != null)
            {
                CurrentPreviewInstance.transform.Rotate(Vector3.up, 20f * Time.deltaTime);
            }
        }

        /// <summary>
        /// Handles UI navigation events and refreshes the dropdown menus.
        /// </summary>
        /// <param name="menu">The name of the target menu.</param>
        public void OnUiNavigation(string menu)
        {
            PopulateVehicleDropdown();
            PopulateSpaceDropdown();
            PopulateBuildingDropdown();
        }

        /// <summary>
        /// Populates the vehicle dropdown menu with available configurations.
        /// </summary>
        public void PopulateVehicleDropdown()
        {
            if (VehicleDropdown == null)
            {
                Debug.LogError("Vehicle dropdown is not assigned.");
                return;
            }

            List<string> vehicleNames = new List<string>();
            VehicleDropdown.ClearOptions();

            foreach (var config in VehicleConfigs)
            {
                vehicleNames.Add(config.Name);
            }

            VehicleDropdown.AddOptions(vehicleNames);
        }

        /// <summary>
        /// Populates the space dropdown menu with available configurations.
        /// </summary>
        public void PopulateSpaceDropdown()
        {
            if (SpaceDropdown == null)
            {
                Debug.LogError("Space dropdown is not assigned.");
                return;
            }

            List<string> spaceNames = new List<string>();
            SpaceDropdown.ClearOptions();

            foreach (var config in SpaceConfigs)
            {
                spaceNames.Add(config.Name);
            }

            SpaceDropdown.AddOptions(spaceNames);
        }

        /// <summary>
        /// Populates the building dropdown menu with available configurations.
        /// </summary>
        public void PopulateBuildingDropdown()
        {
            if (BuildingDropdown == null)
            {
                Debug.LogError("Building dropdown is not assigned.");
                return;
            }

            List<string> buildingNames = new List<string>();
            BuildingDropdown.ClearOptions();

            foreach (var config in BuildingConfigs)
            {
                buildingNames.Add(config.Name);
            }

            BuildingDropdown.AddOptions(buildingNames);
        }

        /// <summary>
        /// Creates all vehicles defined in the configuration list.
        /// Checks if each vehicle already exists before creating it.
        /// </summary>
        public void OnCreateAllVehicles()
        {
            foreach (var vehicleConfig in VehicleConfigs)
            {
                // Check if the vehicle already exists in the lookup table
                if (VehicleCreator.Lookup_VE(vehicleConfig.Id) != null)
                {
                    Debug.Log($"Vehicle Exists: {vehicleConfig.Name}");
                }
                else
                {
                    // Create the vehicle if it does not exist
                    Debug.Log($"Creating Vehicle: {vehicleConfig.Name}");
                    VehicleCreator.Create_VE(vehicleConfig);
                }
            }
        }

        /// <summary>
        /// Creates all spaces defined in the configuration list.
        /// Checks if each space already exists before creating it.
        /// </summary>
        public void OnCreateAllSpaces()
        {
            foreach (var spaceConfig in SpaceConfigs)
            {
                // Check if the space already exists in the lookup table
                if (SpaceCreator.Lookup_VE(spaceConfig.Id) != null)
                {
                    Debug.Log($"Space Exists: {spaceConfig.Name}");
                }
                else
                {
                    // Create the space if it does not exist
                    Debug.Log($"Creating Space: {spaceConfig.Name}");
                    SpaceCreator.Create_VE(spaceConfig);
                }
            }
        }

        /// <summary>
        /// Creates all buildings defined in the configuration list.
        /// Checks if each building already exists before creating it.
        /// </summary>
        public void OnCreateAllBuildings()
        {
            foreach (var buildingConfig in BuildingConfigs)
            {
                // Check if the building already exists in the lookup table
                if (BuildingCreator.Lookup_VE(buildingConfig.Id) != null)
                {
                    Debug.Log($"Building Exists: {buildingConfig.Name}");
                }
                else
                {
                    // Create the building if it does not exist
                    Debug.Log($"Creating Building: {buildingConfig.Name}");
                    BuildingCreator.Create_VE(buildingConfig);
                }
            }
        }

        /// <summary>
        /// Deletes all vehicles currently present in the lookup table.
        /// </summary>
        public void OnDeleteAllVehicles()
        {
            // Collect all vehicle IDs from the lookup table
            List<int> veIdsToDelete = new List<int>(VehicleCreator.LookupTable.Keys);
            foreach (var vehicleId in veIdsToDelete)
            {
                // Delete each vehicle by its ID
                VehicleCreator.Delete_VE(vehicleId);
            }
        }

        /// <summary>
        /// Deletes all spaces currently present in the lookup table.
        /// Disables layout adjustments before deleting spaces.
        /// </summary>
        public void OnDeleteAllSpaces()
        {
            // Disable layout adjustments during deletion
            SpaceCreator.SetLayout(false);

            // Collect all space IDs from the lookup table
            List<int> veIdsToDelete = new List<int>(SpaceCreator.LookupTable.Keys);
            foreach (var spaceId in veIdsToDelete)
            {
                // Delete each space by its ID
                SpaceCreator.Delete_VE(spaceId);
            }
        }

        /// <summary>
        /// Deletes all buildings currently present in the lookup table.
        /// </summary>
        public void OnDeleteAllBuildings()
        {
            // Collect all building IDs from the lookup table
            List<int> veIdsToDelete = new List<int>(BuildingCreator.LookupTable.Keys);
            foreach (var buildingId in veIdsToDelete)
            {
                // Delete each building by its ID
                BuildingCreator.Delete_VE(buildingId);
            }
        }
        /// <summary>
        /// Creates a vehicle based on the selected configuration in the dropdown menu.
        /// Ensures the vehicle does not already exist before creation.
        /// </summary>
        public void OnCreateVehicle()
        {
            if (VehicleDropdown == null || VehicleCreator == null)
            {
                Debug.LogError("Vehicle dropdown or factory is not assigned.");
                return;
            }

            // Retrieve the selected vehicle configuration from the dropdown
            var selectedIndex = VehicleDropdown.value;
            selectedVehicleConfig = VehicleConfigs[selectedIndex];

            // Check if the vehicle already exists
            if (VehicleCreator.Lookup_VE(selectedVehicleConfig.Id) != null)
            {
                Debug.Log($"Vehicle Exists: {selectedVehicleConfig.Name}");
            }
            else
            {
                // Create the vehicle if it does not exist
                Debug.Log($"Creating Vehicle: {selectedVehicleConfig.Name}");
                VehicleCreator.Create_VE(selectedVehicleConfig);
            }
        }

        /// <summary>
        /// Deletes a vehicle based on the selected item in the dropdown menu.
        /// Ensures the vehicle exists before attempting deletion.
        /// </summary>
        public void OnDeleteVehicle()
        {
            if (VehicleDropdown == null || VehicleCreator == null)
            {
                Debug.LogError("Vehicle dropdown or factory is not assigned.");
                return;
            }

            // Retrieve the selected vehicle name from the dropdown
            var selectedIndex = VehicleDropdown.value;
            var selectedVehicleName = VehicleDropdown.options[selectedIndex].text;
            Debug.Log($"Deleting VeVehicle: {selectedVehicleName}");

            // Find and delete the vehicle by matching its name
            foreach (var vehicle in VehicleCreator.LookupTable.Values)
            {
                if (vehicle.Name == selectedVehicleName)
                {
                    VehicleCreator.Delete_VE(vehicle.Id);
                    break;
                }
            }
        }

        /// <summary>
        /// Creates a space based on the selected configuration in the dropdown menu.
        /// Ensures the space does not already exist before creation.
        /// </summary>
        public void OnCreateSpace()
        {
            if (SpaceDropdown == null || SpaceCreator == null)
            {
                Debug.LogError("Space dropdown or factory is not assigned.");
                return;
            }

            // Retrieve the selected space configuration from the dropdown
            var selectedIndex = SpaceDropdown.value;
            selectedSpaceConfig = SpaceConfigs[selectedIndex];

            // Check if the space already exists
            if (SpaceCreator.Lookup_VE(selectedSpaceConfig.Id) != null)
            {
                Debug.Log($"Space Exists: {selectedSpaceConfig.Name}");
            }
            else
            {
                // Create the space if it does not exist
                Debug.Log($"Creating Space: {selectedSpaceConfig.Name}");
                SpaceCreator.Create_VE(selectedSpaceConfig);
            }
        }

        /// <summary>
        /// Deletes a space based on the selected item in the dropdown menu.
        /// Ensures the space exists before attempting deletion.
        /// </summary>
        public void OnDeleteSpace()
        {
            if (SpaceDropdown == null || SpaceCreator == null)
            {
                Debug.LogError("Space dropdown or factory is not assigned.");
                return;
            }

            // Retrieve the selected space name from the dropdown
            var selectedIndex = SpaceDropdown.value;
            var selectedSpaceName = SpaceDropdown.options[selectedIndex].text;
            Debug.Log($"Deleting VeSpace: {selectedSpaceName}");

            // Find and delete the space by matching its name
            foreach (var space in SpaceCreator.LookupTable.Values)
            {
                if (space.Name == selectedSpaceName)
                {
                    SpaceCreator.Delete_VE(space.Id);
                    break;
                }
            }
        }

        /// <summary>
        /// Creates a building based on the selected configuration in the dropdown menu.
        /// Ensures the building does not already exist before creation.
        /// </summary>
        public void OnCreateBuilding()
        {
            if (BuildingDropdown == null || BuildingCreator == null)
            {
                Debug.LogError("Building dropdown or factory is not assigned.");
                return;
            }

            // Retrieve the selected building configuration from the dropdown
            var selectedIndex = BuildingDropdown.value;
            selectedBuildingConfig = BuildingConfigs[selectedIndex];

            // Check if the building already exists
            if (BuildingCreator.Lookup_VE(selectedBuildingConfig.Id) != null)
            {
                Debug.Log($"Building Exists: {selectedBuildingConfig.Name}");
            }
            else
            {
                // Create the building if it does not exist
                Debug.Log($"Creating Building: {selectedBuildingConfig.Name}");
                BuildingCreator.Create_VE(selectedBuildingConfig);
            }
        }

        /// <summary>
        /// Deletes a building based on the selected item in the dropdown menu.
        /// Ensures the building exists before attempting deletion.
        /// </summary>
        public void OnDeleteBuilding()
        {
            if (BuildingDropdown == null || BuildingCreator == null)
            {
                Debug.LogError("Building dropdown or factory is not assigned.");
                return;
            }

            // Retrieve the selected building name from the dropdown
            var selectedIndex = BuildingDropdown.value;
            var selectedBuildingName = BuildingDropdown.options[selectedIndex].text;
            Debug.Log($"Deleting VeBuilding: {selectedBuildingName}");

            // Find and delete the building by matching its name
            foreach (var building in BuildingCreator.LookupTable.Values)
            {
                if (building.Name == selectedBuildingName)
                {
                    BuildingCreator.Delete_VE(building.Id);
                    break;
                }
            }
        }

        /// <summary>
        /// Handles vehicle selection from the dropdown and updates the UI accordingly.
        /// Displays the selected vehicle's information and instantiates a preview of its prefab.
        /// </summary>
        public void OnVehicleSelectedFromDropdown()
        {
            if (VehicleDropdown == null || MenuText == null || PrefabDisplayImage == null || PrefabDisplayCamera == null)
            {
                Debug.LogError("Vehicle dropdown, info text, or prefab display components are not assigned.");
                return;
            }

            // Clean up the current preview instance if it exists
            DeletePreviewPrefabInstance();

            // Retrieve the selected vehicle configuration
            var selectedIndex = VehicleDropdown.value;
            selectedVehicleConfig = VehicleConfigs[selectedIndex];

            // Update the UI text with vehicle information
            MenuText.text =
                $"Vehicle Name: {selectedVehicleConfig.Name}\n" +
                $"Vehicle Id: {selectedVehicleConfig.Id}\n";

            MenuText.text += "Actuation Input Sources: \n";
            if (selectedVehicleConfig.InputStrategies.Count > 0)
            {
                foreach (var source in selectedVehicleConfig.InputStrategies)
                {
                    MenuText.text += $"\t{source.StrategyName}\n";
                }
            }
            else
            {
                MenuText.text += "\tNone\n";
            }

            if (selectedVehicleConfig.IsMoCapAvailable)
            {
                MenuText.text +=
                    $"Motion Capture Enabled: \n" +
                    $"\tTractorOptitrackId = {selectedVehicleConfig.TractorOptitrackId}\n" +
                    $"\tTrailerOptitrackId = {selectedVehicleConfig.TrailerOptitrackId}\n";
            }

            if (selectedVehicleConfig.IsRosAvailable)
            {
                MenuText.text +=
                    $"ROS Connector Enabled: \n" +
                    $"\tRosBridgeServerAddress = {selectedVehicleConfig.RosBridgeServerAddress}";
            }

            // Instantiate and render the selected prefab
            if (selectedVehicleConfig.VehiclePrototypePrefab != null)
            {
                CurrentPreviewInstance = Instantiate(
                    selectedVehicleConfig.VehiclePrototypePrefab,
                    new Vector3(100, 0, 100),
                    Quaternion.identity
                );

                // Set the prefab layer for preview rendering
                SetLayerRecursive(CurrentPreviewInstance, LayerMask.NameToLayer("PrefabDisplay"));
            }
            else
            {
                Debug.LogWarning("VehiclePrototypePrefab is null in the selected config.");
            }
        }

        /// <summary>
        /// Recursively sets the layer of the provided GameObject and its children.
        /// </summary>
        /// <param name="obj">The GameObject whose layer is to be set.</param>
        /// <param name="newLayer">The new layer to apply.</param>
        private void SetLayerRecursive(GameObject obj, int newLayer)
        {
            obj.layer = newLayer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursive(child.gameObject, newLayer);
            }
        }

        /// <summary>
        /// Handles space selection from the dropdown and updates the UI with the selected space's information.
        /// </summary>
        public void OnSpaceSelectedFromDropdown()
        {
            // Clean up the current preview instance if it exists
            DeletePreviewPrefabInstance();

            if (SpaceDropdown == null || MenuText == null)
            {
                Debug.LogError("Space dropdown or info text is not assigned.");
                return;
            }

            // Retrieve the selected space configuration
            var selectedIndex = SpaceDropdown.value;
            selectedSpaceConfig = SpaceConfigs[selectedIndex];

            // Update the UI text with space information
            MenuText.text = $"Space Name: {selectedSpaceConfig.Name}\n" +
                            $"Space Id: {selectedSpaceConfig.Id}\n";
        }

        /// <summary>
        /// Handles building selection from the dropdown and updates the UI with the selected building's information.
        /// </summary>
        public void OnBuildingSelectedFromDropdown()
        {
            // Clean up the current preview instance if it exists
            DeletePreviewPrefabInstance();

            if (BuildingDropdown == null || MenuText == null)
            {
                Debug.LogError("Building dropdown or info text is not assigned.");
                return;
            }

            // Retrieve the selected building configuration
            var selectedIndex = BuildingDropdown.value;
            selectedBuildingConfig = BuildingConfigs[selectedIndex];

            // Update the UI text with building information
            MenuText.text = $"Building Name: {selectedBuildingConfig.Name}\n" +
                            $"Building Id: {selectedBuildingConfig.Id}\n";
        }

        /// <summary>
        /// Deletes the currently displayed preview prefab instance, if any.
        /// </summary>
        public void DeletePreviewPrefabInstance()
        {
            if (CurrentPreviewInstance != null)
            {
                Destroy(CurrentPreviewInstance);
            }
        }

        /// <summary>
        /// Cleans up event subscriptions and deletes the preview prefab instance when the object is destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Unsubscribe from all event channels
            UiNavigationEventChannel.OnEventRaised -= OnUiNavigation;

            ConfirmVehicleCreationEvent.OnEventRaised -= OnCreateVehicle;
            DeleteVehicleEvent.OnEventRaised -= OnDeleteVehicle;

            ConfirmSpaceCreationEvent.OnEventRaised -= OnCreateSpace;
            DeleteSpaceEvent.OnEventRaised -= OnDeleteSpace;

            ConfirmAllBuildingCreationEvent.OnEventRaised -= OnCreateAllBuildings;
            DeleteAllBuildingEvent.OnEventRaised -= OnDeleteAllBuildings;

            ConfirmAllVehicleCreationEvent.OnEventRaised -= OnCreateAllVehicles;
            DeleteAllVehicleEvent.OnEventRaised -= OnDeleteAllVehicles;

            ConfirmAllSpaceCreationEvent.OnEventRaised -= OnCreateAllSpaces;
            DeleteAllSpaceEvent.OnEventRaised -= OnDeleteAllSpaces;

            // Clean up the current preview instance
            DeletePreviewPrefabInstance();
        }

    }
}
