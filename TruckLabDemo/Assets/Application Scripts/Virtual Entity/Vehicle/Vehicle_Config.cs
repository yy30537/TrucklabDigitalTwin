using System;
using System.Collections.Generic;
using Application_Scripts.Virtual_Entity.Vehicle.Controllers.Actuation_Input_Strategy;
using Application_Scripts.Virtual_Entity.Vehicle.Controllers.Kinematics_Strategy;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Vehicle
{
    [CreateAssetMenu(fileName = "Vehicle_Config", menuName = "VE_Config/Vehicle_Config")]
    public class Vehicle_Config : VE_Config
    {
        [Header("Prototype")]
        public GameObject VehiclePrototypePrefab;

        [Header("Vehicle Control")]
        public Kinematics_Strategy KinematicStrategy;
        public List<Actuation_Input_Strategy> InputStrategies;

        public Dictionary<string, Actuation_Input_Strategy> VehicleInputStrategiesDict = new Dictionary<string, Actuation_Input_Strategy>();

        [Header("Vehicle Dimensions")]
        public float Scale = 13f;
        public float L1Scaled = 0.27f;
        public float L1CScaled = 0.055f;
        public float R2Scaled = 0.567f;
        public float TractorWidthScaled = 0.165f;
        public float TrailerWidthScaled = 0.169f;

        [Header("Mocap Configuration")]
        public bool IsMoCapAvailable;
        public string OptitrackServerAddress;
        public string OptitrackLocalAddress;
        public Int32 TractorOptitrackId;
        public Int32 TrailerOptitrackId;

        [Header("ROS Configuration")]
        public bool IsRosAvailable;
        public string RosBridgeServerAddress;
        public string TwistSubscriberTopic;
        public string TwistPublisherTopic;

        [Header("Initial VeVehicle States")]
        public float InitialTractorPosX;
        public float InitialTractorPosY;
        public float InitialTractorAngle;
        public float InitialTrailerAngle;
        public float InitialVelocity;
        public float InitialAcceleration;
        public float InitialSteeringAngle;

        private void OnEnable()
        {
            // Initialize the dictionary from the list of assets
            if (InputStrategies.Count > 0)
            {
                VehicleInputStrategiesDict.Clear();
                foreach (var asset in InputStrategies)
                {
                    if (asset != null && !VehicleInputStrategiesDict.TryAdd(asset.StrategyName, asset))
                    {
                        VehicleInputStrategiesDict.Add(asset.StrategyName, asset);
                    }
                }
            }
            
        }
    }
}
