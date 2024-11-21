using System.Collections.Generic;
using Application_Scripts.Manager;
using Application_Scripts.UI_Controller.Application_UI;
using Application_Scripts.Virtual_Entity.Space;
using Application_Scripts.Virtual_Entity.Vehicle.Controllers;
using Application_Scripts.Virtual_Entity.Vehicle.Controllers.Collision_Control;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Protocols;
using TMPro;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Vehicle
{
    public class VE_Vehicle : VE
    {
        /// <summary>
        /// Configuration data for the vehicle, defining properties and settings.
        /// </summary>
        [Header("Vehicle Data")]
        public Vehicle_Config Config;

        /// <summary>
        /// Runtime data instance for the vehicle, managing state and physics.
        /// </summary>
        public Vehicle_Data Data;

        /// <summary>
        /// List of cameras attached to the vehicle for various views.
        /// </summary>
        public List<Camera> VehicleCameras;

        /// <summary>
        /// The tractor component of the vehicle.
        /// </summary>
        public GameObject Tractor;

        /// <summary>
        /// The trailer component of the vehicle.
        /// </summary>
        public GameObject Trailer;

        /// <summary>
        /// Controllers for vehicle systems (dashboard, animation, kinematics, collision).
        /// </summary>
        [Header("Vehicle Controllers")]
        public Vehicle_Dashboard_Controller VehicleDashboardController;
        public Vehicle_Animation_Controller AnimationController;
        public Vehicle_Kinematics_Controller KinematicsController;
        public Vehicle_Collision_Controller CollisionController;

        /// <summary>
        /// Components for motion capture integration.
        /// </summary>
        [Header("Middleware Components")]
        [Header("   Motion Capture")]
        public GameObject TractorMoCapComponentInstance;
        public GameObject TrailerMoCapComponentInstance;
        public OptitrackRigidBody TractorRigidBody;
        public OptitrackRigidBody TrailerRigidBody;

        /// <summary>
        /// Components for ROS communication.
        /// </summary>
        [Header("   ROS")]
        public GameObject RosComponentInstance;
        public RosConnector RosConnector;
        public TwistPublisher TwistPublisher;
        public TwistSubscriber SharedTwistSubscriber;

        /// <summary>
        /// External dependencies required for vehicle initialization and operation.
        /// </summary>
        [Header("Application Dependencies")]
        public Transform VeInstanceParentTransform;
        public Transform VeUiInstanceParentTransform;
        public Camera_Manager CameraManager;
        public Space_Creator SpaceCreator;
        public Vehicle_Creator VehicleCreator;
        public UI_Controller_SystemLog SystemLogUiController;
        public VE_OnClick_Getter VeOnClickGetterGetter;


        /// <summary>
        /// Sets the dependencies required for the proper functioning of the vehicle entity.
        /// </summary>
        /// <param name="ve_instance">The vehicle GameObject instance in the scene.</param>
        /// <param name="ve_instance_parent_transform">The parent transform for the vehicle instance in the scene.</param>
        /// <param name="ve_ui_parent_transform">The parent transform for the vehicle's UI elements.</param>
        /// <param name="vehicle_config">The configuration object for the vehicle.</param>
        /// <param name="camera_manager">The camera manager responsible for handling camera operations.</param>
        /// <param name="vehicle__creator">The vehicle creator responsible for managing vehicle instantiation.</param>
        /// <param name="space__creator">The space creator responsible for managing spaces in the simulation.</param>
        /// <param name="systemLog_ui_controller">The system log UI controller for logging events and system activities.</param>
        /// <param name="ve_onclick_getter">The utility for detecting and handling mouse click interactions.</param>
        public void SetDependencies(
            GameObject ve_instance,
            Transform ve_instance_parent_transform,
            Transform ve_ui_parent_transform,
            Vehicle_Config vehicle_config,
            Camera_Manager camera_manager,
            Vehicle_Creator vehicle__creator,
            Space_Creator space__creator,
            UI_Controller_SystemLog systemLog_ui_controller,
            VE_OnClick_Getter ve_onclick_getter)
        {
            Instance = ve_instance;
            Config = vehicle_config;
            VeInstanceParentTransform = ve_instance_parent_transform;
            VeUiInstanceParentTransform = ve_ui_parent_transform;
            CameraManager = camera_manager;
            SpaceCreator = space__creator;
            VehicleCreator = vehicle__creator;
            SystemLogUiController = systemLog_ui_controller;
            VeOnClickGetterGetter = ve_onclick_getter;
        }

        /// <summary>
        /// Initializes the vehicle and its components.
        /// </summary>
        public override void Init()
        {
            InitializeCommonProperties();
            InitializeControllers();
            InitializeUiController();

            if (Config.IsMoCapAvailable)
            {
                InitMoCap();
            }

            if (Config.IsRosAvailable)
            {
                InitRos();
            }

            SystemLogUiController.LogEvent($"Initialized Vehicle: {Name}");
        }

        /// <summary>
        /// Initializes common properties like ID, name, and basic structure.
        /// </summary>
        private void InitializeCommonProperties()
        {
            Id = Config.Id;
            Name = Config.Name;
            Instance.name = Name;
            Instance.tag = "Vehicle";
            Instance.transform.Find("Trailer").Find("Truck Label").GetComponent<TextMeshPro>().text = Name;

            Tractor = Instance.transform.Find("Tractor").gameObject;
            Trailer = Instance.transform.Find("Trailer").gameObject;

            CollectVehicleCameras();
        }

        /// <summary>
        /// Initializes controllers such as kinematics, animation, and collision.
        /// </summary>
        private void InitializeControllers()
        {
            Data = Instance.AddComponent<Vehicle_Data>();
            Data.Init(Config.Id, Config.Name, Config, Instance);

            KinematicsController = Instance.AddComponent<Vehicle_Kinematics_Controller>();
            KinematicsController.Init(this, Data);

            AnimationController = Instance.AddComponent<Vehicle_Animation_Controller>();
            AnimationController.Init(this, Data);

            CollisionController = Instance.AddComponent<Vehicle_Collision_Controller>();
            CollisionController.Init(this, Data);

            AnimationController.IsActive = true;
            KinematicsController.IsActive = true;
            CollisionController.IsActive = true;
        }

        /// <summary>
        /// Initializes the vehicle's UI dashboard.
        /// </summary>
        private void InitializeUiController()
        {
            var ui_instance = Instantiate(Config.UiTemplate, VeUiInstanceParentTransform);
            VehicleDashboardController = ui_instance.AddComponent<Vehicle_Dashboard_Controller>();
            VehicleDashboardController.SetDependencies(this, Data, ui_instance, VeUiInstanceParentTransform, SystemLogUiController);
            VehicleDashboardController.Init();

            VehicleDashboardController.PopulateInputDropdown();
        }

        /// <summary>
        /// Collects all cameras associated with the vehicle, excluding mirrors.
        /// </summary>

        private void CollectVehicleCameras()
        {
            VehicleCameras = new List<Camera>();
            VehicleCameras.Clear();
            Camera[] cameras = Instance.GetComponentsInChildren<Camera>(true);
            foreach (Camera cam in cameras)
            {
                if (!cam.name.Contains("Mirror"))
                {
                    cam.targetDisplay = 0;
                    cam.enabled = false;
                    VehicleCameras.Add(cam);
                }
                else
                {
                    cam.targetDisplay = 1;
                    cam.enabled = true;
                }
            }

            //CameraManager.ActivateCamera(CameraManager.MainCamera);
        }

        /// <summary>
        /// Initializes ROS components for vehicle communication.
        /// </summary>
        private void InitRos()
        {
            RosComponentInstance = new GameObject("ROSComponent");
            RosComponentInstance.transform.SetParent(AnimationController.VehicleTransform);

            RosConnector = RosComponentInstance.AddComponent<RosConnector>();
            RosConnector.RosBridgeServerUrl = Config.RosBridgeServerAddress;
            RosConnector.SecondsTimeout = 10;
            RosConnector.Serializer = RosSocket.SerializerEnum.Microsoft;
            RosConnector.protocol = Protocol.WebSocketSharp;


            TwistPublisher = RosComponentInstance.AddComponent<TwistPublisher>();
            TwistPublisher.Topic = Config.TwistPublisherTopic;

            //// Create A single TwistSubscriber to be shared across strategies if needed
            //SharedTwistSubscriber = RosComponentInstance.AddComponent<TwistSubscriber>();
            //SharedTwistSubscriber.Topic = Config.TwistSubscriberTopic;

            //// Initialize each Controller_Input_Strategy with the shared TwistSubscriber
            //foreach (var inputStrategy in Config.VehicleInputStrategiesDict.Values)
            //{
            //    if (inputStrategy is Controller_Input_Strategy controllerInputStrategy)
            //    {
            //        controllerInputStrategy.Initialize(SharedTwistSubscriber);
            //    }
            //}
        }

        /// <summary>
        /// Initializes motion capture components for the tractor and trailer.
        /// </summary>
        private void InitMoCap()
        {
            TractorRigidBody = InitTractorMoCap();
            TrailerRigidBody = InitTrailerMoCap();
        }

        private OptitrackRigidBody InitTractorMoCap()
        {
            TractorMoCapComponentInstance = new GameObject("TractorMoCap");
            TractorMoCapComponentInstance.transform.SetParent(AnimationController.VehicleTransform);
            var tractorRigidBody = TractorMoCapComponentInstance.AddComponent<OptitrackRigidBody>();

            var streamingClient = TractorMoCapComponentInstance.AddComponent<OptitrackStreamingClient>();
            tractorRigidBody.StreamingClient = streamingClient;

            streamingClient.ServerAddress = Config.OptitrackServerAddress;
            streamingClient.LocalAddress = Config.OptitrackLocalAddress;
            streamingClient.ConnectionType = OptitrackStreamingClient.ClientConnectionType.Unicast;

            tractorRigidBody.RigidBodyId = Config.TractorOptitrackId;
            tractorRigidBody.NetworkCompensation = false;
            streamingClient.enabled = true;

            return tractorRigidBody;
        }

        private OptitrackRigidBody InitTrailerMoCap()
        {
            TrailerMoCapComponentInstance = new GameObject("TrailerMoCap");
            TrailerMoCapComponentInstance.transform.SetParent(AnimationController.VehicleTransform);
            var trailerRigidBody = TrailerMoCapComponentInstance.AddComponent<OptitrackRigidBody>();

            var streamingClient = TrailerMoCapComponentInstance.AddComponent<OptitrackStreamingClient>();
            trailerRigidBody.StreamingClient = streamingClient;

            streamingClient.ServerAddress = Config.OptitrackServerAddress;
            streamingClient.LocalAddress = Config.OptitrackLocalAddress;
            streamingClient.ConnectionType = OptitrackStreamingClient.ClientConnectionType.Unicast;

            trailerRigidBody.RigidBodyId = Config.TrailerOptitrackId;
            trailerRigidBody.NetworkCompensation = true;
            streamingClient.enabled = true;

            return trailerRigidBody;
        }

        private void OnDestroy()
        {
            Destroy(AnimationController);
            Destroy(KinematicsController);
            Destroy(CollisionController);
            Destroy(VehicleDashboardController);

            if (Config.IsMoCapAvailable)
            {
                Destroy(TractorMoCapComponentInstance);
                Destroy(TrailerMoCapComponentInstance);
            }

            if (RosComponentInstance != null)
            {
                Destroy(RosComponentInstance);
                SharedTwistSubscriber = null;
            }

            SystemLogUiController.LogEvent($"Destroyed Vehicle: {Name}");
        }
    }
}
