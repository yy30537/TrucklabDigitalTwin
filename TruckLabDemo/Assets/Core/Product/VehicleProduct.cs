using UnityEngine;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Protocols;
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// Represents a vehicle product in the simulation.
    /// Manages the initialization, configuration, and functionality of the vehicle.
    /// </summary>
    public class VehicleProduct : BaseProduct
    {
        public VehicleData Data;
        public VehicleConfig Config;

        public VehicleAnimation Animation;
        public VehicleKinematics Kinematics;
        public VehicleCollisionController Collision;

        public GameObject TractorMoCapComponentInstance;
        public GameObject TrailerMoCapComponentInstance;
        public OptitrackRigidBody TractorRigidBody;
        public OptitrackRigidBody TrailerRigidBody;

        public GameObject RosComponentInstance;
        public RosConnector RosConnector;
        public TwistSubscriber TwistSubscriber;
        public TwistPublisher TwistPublisher;

        public VehicleUI Ui;
        public VehicleObserver Observer;

        public List<Camera> VehicleCameras;

        public RegionFactory RegionFactory;

        /// <summary>
        /// Initializes the vehicle product.
        /// </summary>
        public override void Initialize()
        {
            Data = new VehicleData(Config.VehicleId, Config.VehicleName, Config, ProductInstance);
            RegionFactory = FindObjectOfType<RegionFactory>();

            InitializeComponents();
            base.Initialize();
            InitializeMoCap();
            InitializeRos();

            Animation.isActive = true;
            Kinematics.isActive = true;
            Collision.isActive = true;

            Observer.StartObserving();

            VehicleCameras = new List<Camera>();
            CollectVehicleCameras();

            //SystemLogWindow.LogEvent($"Initialized Vehicle: {ProductName}");
        }

        /// <summary>
        /// Collects all cameras attached to the vehicle and sets their initial states.
        /// </summary>
        private void CollectVehicleCameras()
        {
            VehicleCameras.Clear();
            Camera[] cameras = ProductInstance.GetComponentsInChildren<Camera>(true); // Include inactive cameras
            foreach (Camera cam in cameras)
            {
                cam.targetDisplay = 0; // Set to Display 1
                cam.enabled = false; // Ensure all vehicle cameras start disabled
                VehicleCameras.Add(cam);
            }
        }

        /// <summary>
        /// Initializes the components of the vehicle.
        /// </summary>
        private void InitializeComponents()
        {
            Animation = ProductInstance.AddComponent<VehicleAnimation>();
            Animation.Initialize(this, Data);

            Kinematics = ProductInstance.AddComponent<VehicleKinematics>();
            Kinematics.Initialize(this, Data);

            Collision = ProductInstance.AddComponent<VehicleCollisionController>();
            Collision.Initialize(this, Data);
        }

        /// <summary>
        /// Initializes the UI components for the vehicle.
        /// </summary>
        protected override void InitializeUI()
        {
            var uiInstance = Instantiate(Config.VehicleUiPrefab, UiParent);
            Ui = uiInstance.GetComponent<VehicleUI>();
            uiInstance.name = Config.VehicleName;
            if (Ui == null)
            {
                Ui = uiInstance.AddComponent<VehicleUI>();
            }
            Ui.Initialize(UiParent);
            Ui.SetVehicleData(this, Data);
            //SystemLogWindow.LogEvent($"Initialized Vehicle UI for: {ProductName}");
        }

        /// <summary>
        /// Initializes the observer component for the vehicle.
        /// </summary>
        protected override void InitializeObserver()
        {
            Observer = gameObject.AddComponent<VehicleObserver>();
            Observer.Initialize(this, Ui);
        }

        /// <summary>
        /// Initializes the motion capture components if available.
        /// </summary>
        private void InitializeMoCap()
        {
            if (Config.IsMoCapAvailable)
            {
                TractorRigidBody = InitTractorMoCap();
                TrailerRigidBody = InitTrailerMoCap();
            }
        }

        /// <summary>
        /// Initializes the ROS components.
        /// </summary>
        private void InitializeRos()
        {
            RosConnector = InitRosConnector();
        }

        /// <summary>
        /// Initializes the OptiTrack component for the tractor.
        /// </summary>
        /// <returns>The initialized OptiTrack rigid body for the tractor.</returns>
        private OptitrackRigidBody InitTractorMoCap()
        {
            TractorMoCapComponentInstance = new GameObject("TractorMoCap");
            TractorMoCapComponentInstance.transform.SetParent(Animation.vehicleTransform);

            TractorRigidBody = TractorMoCapComponentInstance.AddComponent<OptitrackRigidBody>();

            OptitrackStreamingClient tractorOptitrackStreamingClient =
                TractorMoCapComponentInstance.AddComponent<OptitrackStreamingClient>();

            TractorRigidBody.StreamingClient = tractorOptitrackStreamingClient;

            tractorOptitrackStreamingClient.ServerAddress = Config.OptitrackServerAddress;
            tractorOptitrackStreamingClient.LocalAddress = Config.OptitrackLocalAddress;
            tractorOptitrackStreamingClient.ConnectionType = OptitrackStreamingClient.ClientConnectionType.Unicast;

            TractorRigidBody.RigidBodyId = Config.TractorOptitrackId;
            TractorRigidBody.NetworkCompensation = false;

            tractorOptitrackStreamingClient.enabled = true;

            return TractorRigidBody;
        }

        /// <summary>
        /// Initializes the OptiTrack component for the trailer.
        /// </summary>
        /// <returns>The initialized OptiTrack rigid body for the trailer.</returns>
        private OptitrackRigidBody InitTrailerMoCap()
        {
            TrailerMoCapComponentInstance = new GameObject("TrailerMoCap");
            TrailerMoCapComponentInstance.transform.SetParent(Animation.vehicleTransform);
            TrailerRigidBody = TrailerMoCapComponentInstance.AddComponent<OptitrackRigidBody>();

            OptitrackStreamingClient trailerOptitrackStreamingClient =
                TrailerMoCapComponentInstance.AddComponent<OptitrackStreamingClient>();
            TrailerRigidBody.StreamingClient = trailerOptitrackStreamingClient;
            TrailerRigidBody.RigidBodyId = Config.TrailerOptitrackId;
            TrailerRigidBody.NetworkCompensation = true;

            trailerOptitrackStreamingClient.ServerAddress = Config.OptitrackServerAddress;
            trailerOptitrackStreamingClient.LocalAddress = Config.OptitrackLocalAddress;
            trailerOptitrackStreamingClient.ConnectionType = OptitrackStreamingClient.ClientConnectionType.Unicast;

            trailerOptitrackStreamingClient.enabled = true;

            return TrailerRigidBody;
        }

        /// <summary>
        /// Initializes the ROS connector component.
        /// </summary>
        /// <returns>The initialized ROS connector.</returns>
        private RosConnector InitRosConnector()
        {
            RosComponentInstance = new GameObject("ROSComponent");
            RosComponentInstance.transform.SetParent(Animation.vehicleTransform);

            RosConnector = RosComponentInstance.AddComponent<RosConnector>();
            RosConnector.RosBridgeServerUrl = Config.RosBridgeServerAddress;
            RosConnector.SecondsTimeout = 10;
            RosConnector.Serializer = RosSocket.SerializerEnum.Microsoft;
            RosConnector.protocol = Protocol.WebSocketSharp;

            InitRosComponents();

            return RosConnector;
        }

        /// <summary>
        /// Initializes the ROS components (subscribers and publishers).
        /// </summary>
        private void InitRosComponents()
        {
            TwistSubscriber = RosComponentInstance.AddComponent<TwistSubscriber>();
            TwistSubscriber.Topic = Config.TwistSubscriberTopicController;

            TwistPublisher = RosComponentInstance.AddComponent<TwistPublisher>();
            TwistPublisher.Topic = Config.TwistPublisherTopic;
        }

        /// <summary>
        /// Cleanup and destroy components when the vehicle product is destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Config.IsMoCapAvailable)
            {
                Destroy(TractorMoCapComponentInstance);
                Destroy(TrailerMoCapComponentInstance);
            }
            Destroy(RosComponentInstance);
            Destroy(Animation);
            Destroy(Kinematics);
            Destroy(Collision);

            //if (Observer != null)
            //{
            //    Observer.StopObserving();
            //    Destroy(Observer);
            //}
            //if (Ui != null)
            //{
            //    Destroy(Ui.gameObject);
            //}


            SystemLogWindow.LogEvent($"Destroyed Vehicle: {ProductName}");
        }
    }
}
