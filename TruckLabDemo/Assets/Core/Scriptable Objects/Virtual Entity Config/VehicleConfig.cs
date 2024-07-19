using UnityEngine;
using System;

namespace Core
{
    [CreateAssetMenu(fileName = "VehicleConfig", menuName = "VehicleConfig")]
    public class VehicleConfig : BaseEntityConfig
    {
        [Header("Prototype")]
        public GameObject VehiclePrototypePrefab;

        [Header("Parameters")]
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

        public string TwistSubscriberTopicThrustMaster;
        public string TwistSubscriberTopicController;
        public string TwistPublisherTopic;

        [Header("Vehicle Control")]
        public KinematicsSource KinematicsSource;
        public ActuationInputSource ActuationInputSource;

        [Header("Initial Vehicle States")]
        public float InitialTractorPosX;
        public float InitialTractorPosY;
        public float InitialTractorAngle;
        public float InitialTrailerAngle;
        public float InitialVelocity;
        public float InitialAcceleration;
        public float InitialSteeringAngle;

        // Overriding base class properties to maintain existing naming
        public int VehicleId { get => base.EntityId; set => base.EntityId = value; }
        public string VehicleName { get => base.EntityName; set => base.EntityName = value; }
        public GameObject VehicleUiPrefab { get => base.UiPrefab; set => base.UiPrefab = value; }
    }

    public enum ActuationInputSource
    {
        ThrustMaster,
        Controller,
        Keyboard,
    }

    public enum KinematicsSource
    {
        MotionCapture,
        Actuation
    }
}