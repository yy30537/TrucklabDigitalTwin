using System;
using System.Collections.Generic;
using RosSharp.RosBridgeClient;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class FactoryMenu : Menu
    {
        private VehicleConfig selectedVehicleConfig;
        
        [Header("UI Components")]
        public TMP_Dropdown vehicleDropdown;
        public TextMeshProUGUI vehicleInfoText;
        
        [Header("UI Event Channels")]
        public VoidEventChannel confirmVehicleCreation;
        public VoidEventChannel deleteVehicleEvent;
        
        [Header("Factories")]
        public List<VehicleConfig> vehiclesConfig;
        public List<SpaceConfig> spacesConfig;
        public List<DockConfig> docksConfig;
        
        public Factory<VehicleProduct, VehicleConfig> vehicleFactory;
        public Factory<SpaceProduct, SpaceConfig> spaceFactory;
        public Factory<DockProduct, DockConfig> dockFactory;
        
        void Start()
        {
            base.Init();

            ecToggle.onEventRaised += PopulateVehicleDropdown;
            confirmVehicleCreation.onEventRaised += OnCreateVehicle;
            deleteVehicleEvent.onEventRaised += OnDeleteVehicle;
            
            foreach (var spaceConfig in spacesConfig)
            {
                spaceFactory.GetProduct(spaceConfig);
            }
            foreach (var dockConfig in docksConfig)
            {
                dockFactory.GetProduct(dockConfig);
            }
        }
        private void OnDestroy()
        {
            ecToggle.onEventRaised -= PopulateVehicleDropdown;
            confirmVehicleCreation.onEventRaised -= OnCreateVehicle;
            deleteVehicleEvent.onEventRaised -= OnDeleteVehicle;
        }
        public void PopulateVehicleDropdown(string menu)
        {
            List<string> vehicleNames = new List<string>();
            vehicleDropdown.ClearOptions();

            if (vehiclesConfig.Count > 0)
            {
                foreach (var config in vehiclesConfig)
                {
                    vehicleNames.Add(config.vehicleName);
                }
            }
            vehicleDropdown.AddOptions(vehicleNames);
            OnVehicleSelectedFromDropdown();
        }
        public void OnCreateVehicle()
        {
            var selectedIndex = vehicleDropdown.value;
            selectedVehicleConfig = vehiclesConfig[selectedIndex];
            Debug.Log("Create Vehicle: " + selectedVehicleConfig.vehicleName);
            vehicleFactory.GetProduct(selectedVehicleConfig);
        }
        public void OnDeleteVehicle()
        {
            var selectedIndex = vehicleDropdown.value;
            var selectedVehicleName = vehicleDropdown.options[selectedIndex].text;
            Debug.Log("Delete Vehicle: " + selectedVehicleName);

            foreach (var vehicle in vehicleFactory.productLookupTable.Values)
            {
                if (vehicle.productName == selectedVehicleName)
                {
                    vehicleFactory.DeleteProduct(vehicle.productID);
                    break;
                }
            }
        }
        public void OnVehicleSelectedFromDropdown()
        {
            var selectedIndex = vehicleDropdown.value;
            selectedVehicleConfig = vehiclesConfig[selectedIndex];
            vehicleInfoText.text =
                $"Vehicle Name: {selectedVehicleConfig.vehicleName} \n" +
                $"Vehicle ID: {selectedVehicleConfig.vehicleID} \n" +
                $"Motion Capture: {selectedVehicleConfig.isMocapAvaialbe} \n" +
                $"ROS: {selectedVehicleConfig.isRosAvaialbe} \n";
            if (selectedVehicleConfig.isMocapAvaialbe)
            {
                vehicleInfoText.text +=
                    $"Tractor Optitrack ID: {selectedVehicleConfig.tractorOptitrackID} \n" +
                    $"Trailer Optitrack ID: {selectedVehicleConfig.trailorOptitrackID} \n";
            }

        }
    }
}