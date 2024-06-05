using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class FactoryMenu : Menu
    {
        
        [Header("UI Components")]
        public TMP_Dropdown vehicleDropdown;
        public TextMeshProUGUI vehicleInfoText;
        
        [Header("UI Event Channels")]
        public VoidEventChannel confirmVehicleCreation;
        public VoidEventChannel deleteVehicleEvent;
        
        [Header("Dependencies")]
        public ApplicationManager applicationManager;
        private VehicleConfig selectedVehicleConfig;
        
        void Start()
        {
            base.Init();

            ecToggle.onEventRaised += PopulateVehicleDropdown;
            confirmVehicleCreation.onEventRaised += OnCreateVehicle;
            deleteVehicleEvent.onEventRaised += OnDeleteVehicle;
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

            if (applicationManager.vehiclesConfig.Count > 0)
            {
                foreach (var config in applicationManager.vehiclesConfig)
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
            selectedVehicleConfig = applicationManager.vehiclesConfig[selectedIndex];
            Debug.Log("Create Vehicle: " + selectedVehicleConfig.vehicleName);
            applicationManager.vehicleFactory.ManufactureProduct(selectedVehicleConfig);
        }
        public void OnDeleteVehicle()
        {
            var selectedIndex = vehicleDropdown.value;
            var selectedVehicleName = vehicleDropdown.options[selectedIndex].text;
            Debug.Log("Delete Vehicle: " + selectedVehicleName);

            foreach (var vehicle in applicationManager.vehicleFactory.productLookupTable.Values)
            {
                if (vehicle.productName == selectedVehicleName)
                {
                    applicationManager.vehicleFactory.DeleteProduct(vehicle.productID);
                    break;
                }
            }
        }
        public void OnVehicleSelectedFromDropdown()
        {
            var selectedIndex = vehicleDropdown.value;
            selectedVehicleConfig = applicationManager.vehiclesConfig[selectedIndex];
            vehicleInfoText.text =
                $"Vehicle Name: {selectedVehicleConfig.vehicleName} \n" +
                $"Vehicle ID: {selectedVehicleConfig.vehicleID} \n" +
                $"\n" +
                $"Vehicle Components:\n" +
                $"\n" +
                $"Animation Controller \n" +
                $"Kinematics Controller \n" +
                $"Collision Controller \n" +
                $"\n" +
                $"Motion Capture: {selectedVehicleConfig.isMocapAvaialbe} \n" +
                $"ROS Connection: {selectedVehicleConfig.isRosAvaialbe} \n" +
                $"Kinematic Source: {selectedVehicleConfig.kinematicsSource} \n" +
                $"Actuation Input Source: {selectedVehicleConfig.actuationInputSource} \n";


        }
    }
}