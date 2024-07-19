using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Manages the factory menu for creating, deleting, and displaying vehicle information.
    /// </summary>
    public class FactoryMenu : BaseMenu
    {
        private VehicleConfig selectedVehicleConfig;

        [Header("UI Components")]
        [SerializeField] private TMP_Dropdown vehicleDropdown;
        [SerializeField] private TextMeshProUGUI vehicleInfoText;

        [Header("UI Event Channels")]
        [SerializeField] private VoidEventChannel confirmVehicleCreationEvent;
        [SerializeField] private VoidEventChannel deleteVehicleEvent;

        [Header("Factories")]
        [SerializeField] private List<VehicleConfig> vehicleConfigs;
        [SerializeField] private List<RegionConfig> regionConfigs;
        [SerializeField] private List<DockConfig> dockConfigs;

        [SerializeField] private BaseFactory<VehicleProduct, VehicleConfig> vehicleFactory;
        [SerializeField] private BaseFactory<RegionProduct, RegionConfig> regionFactory;
        [SerializeField] private BaseFactory<DockProduct, DockConfig> dockFactory;

        private void Start()
        {
            base.Init();
            MenuName = "Factory";

            ToggleEc.onEventRaised += PopulateVehicleDropdown;
            confirmVehicleCreationEvent.onEventRaised += OnCreateVehicle;
            deleteVehicleEvent.onEventRaised += OnDeleteVehicle;
        }

        /// <summary>
        /// Creates all regions defined in the region configurations.
        /// </summary>
        public void CreateRegions()
        {
            foreach (var regionConfig in regionConfigs)
            {
                regionFactory.GetProduct(regionConfig);
            }
        }

        /// <summary>
        /// Creates all dock stations defined in the dock configurations.
        /// </summary>
        public void CreateDockStations()
        {
            foreach (var dockConfig in dockConfigs)
            {
                dockFactory.GetProduct(dockConfig);
            }
        }

        /// <summary>
        /// Unsubscribes from event listeners when the menu is destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ToggleEc.onEventRaised -= PopulateVehicleDropdown;
            confirmVehicleCreationEvent.onEventRaised -= OnCreateVehicle;
            deleteVehicleEvent.onEventRaised -= OnDeleteVehicle;
        }

        /// <summary>
        /// Populates the vehicle dropdown with available vehicle configurations.
        /// </summary>
        public void PopulateVehicleDropdown(string menu)
        {
            if (vehicleDropdown == null)
            {
                Debug.LogError("Vehicle dropdown is not assigned.");
                return;
            }

            List<string> vehicleNames = new List<string>();
            vehicleDropdown.ClearOptions();

            foreach (var config in vehicleConfigs)
            {
                vehicleNames.Add(config.VehicleName);
            }

            vehicleDropdown.AddOptions(vehicleNames);
            OnVehicleSelectedFromDropdown();
        }

        /// <summary>
        /// Handles the creation of a new vehicle.
        /// </summary>
        public void OnCreateVehicle()
        {
            if (vehicleDropdown == null || vehicleFactory == null)
            {
                Debug.LogError("Vehicle dropdown or factory is not assigned.");
                return;
            }

            var selectedIndex = vehicleDropdown.value;
            selectedVehicleConfig = vehicleConfigs[selectedIndex];

            if (vehicleFactory.LookupProduct(selectedVehicleConfig.VehicleId) != null)
            {
                Debug.Log($"Vehicle Exists: {selectedVehicleConfig.VehicleName}");
            }
            else
            {
                Debug.Log($"Creating Vehicle: {selectedVehicleConfig.VehicleName}");
                vehicleFactory.GetProduct(selectedVehicleConfig);
            }
        }

        /// <summary>
        /// Handles the deletion of a selected vehicle.
        /// </summary>
        public void OnDeleteVehicle()
        {
            if (vehicleDropdown == null || vehicleFactory == null)
            {
                Debug.LogError("Vehicle dropdown or factory is not assigned.");
                return;
            }

            var selectedIndex = vehicleDropdown.value;
            var selectedVehicleName = vehicleDropdown.options[selectedIndex].text;
            Debug.Log($"Deleting Vehicle: {selectedVehicleName}");

            foreach (var vehicle in vehicleFactory.ProductLookupTable.Values)
            {
                if (vehicle.ProductName == selectedVehicleName)
                {
                    vehicleFactory.DeleteProduct(vehicle.ProductID);
                    break;
                }
            }
        }

        /// <summary>
        /// Updates the vehicle information display when a vehicle is selected from the dropdown.
        /// </summary>
        public void OnVehicleSelectedFromDropdown()
        {
            if (vehicleDropdown == null || vehicleInfoText == null)
            {
                Debug.LogError("Vehicle dropdown or info text is not assigned.");
                return;
            }

            var selectedIndex = vehicleDropdown.value;
            selectedVehicleConfig = vehicleConfigs[selectedIndex];

            vehicleInfoText.text = $"Vehicle Name: {selectedVehicleConfig.VehicleName}\n" +
                                   $"Vehicle ID: {selectedVehicleConfig.VehicleId}\n" +
                                   $"Motion Capture: {selectedVehicleConfig.IsMoCapAvailable}\n" +
                                   $"ROS: {selectedVehicleConfig.IsRosAvailable}";

            if (selectedVehicleConfig.IsMoCapAvailable)
            {
                vehicleInfoText.text += $"\nTractor Optitrack ID: {selectedVehicleConfig.TractorOptitrackId}\n" +
                                        $"Trailer Optitrack ID: {selectedVehicleConfig.TrailerOptitrackId}";
            }
        }
    }
}
