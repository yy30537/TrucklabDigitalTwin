using UnityEngine;
using System;

namespace Core
{
    public enum ActuationInputSource
    {
        keyboard,
        manual,
        controller,
    }
    
    public enum KinematicsSource
    {
        actuation,
        motioncapture
    }
    
    [CreateAssetMenu(fileName = "VehicleConfig", menuName = "VehicleConfig")]
    public class VehicleConfig: ScriptableObject
    {
        public int vehicleID;
        public string vehicleName;
        
        [Header("Prototype")]
        public GameObject vehiclePrototypePrefab;
        public GameObject vehicleDashboardPrefab;
        
        [Header("Dimensions")]
        public float scale = 13f;
        public float l1Scaled = 0.27f;
        public float l1CScaled = 0.055f;
        public float l2Scaled = 0.567f;
        public float tractorWidthScaled = 0.165f;
        public float trailerWidthScaled = 0.169f;
        
        [Header("Mocap Configuration")] 
        public bool isMocapAvaialbe;
        public string optitrackServerAddress;
        public string optitrackLocalAddress;
        public Int32 tractorOptitrackID;
        public Int32 trailorOptitrackID;
        
        [Header("ROS Configuration")]
        public bool isRosAvaialbe;
        public string rosBridgeServerAddress;
        public string twistSubscriberTopicManual;
        public string twistSubscriberTopicController;
        
        public string twistPublisherTopicManual;
        
        // TODO: add more fields for ros to pre configure stuff
        
        [Header("Vehicle Control")]
        public KinematicsSource kinematicsSource;
        public ActuationInputSource actuationInputSource;
        
        [Header("Initial Vehicle States")]
        public float initialTractorPosX;
        public float initialTractorPosY;
        public float initialTractorAngle;
        public float initialTrailerAngle;
        public float initialVelocity;
        public float initialAcceleration;
        public float initialSteeringAngle;

        [Header("Dashboard Toggle Event Channel")]
        public VoidEventChannel ecToggleDashboard;
    }
}


