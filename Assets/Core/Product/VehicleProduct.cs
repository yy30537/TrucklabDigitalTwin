using UnityEngine;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Protocols;

namespace Core
{
    /// <summary>
    /// Represents a vehicle product in the simulation.
    /// </summary>
    public class VehicleProduct : MonoBehaviour, IProduct
    {
        [Header("Vehicle Identity")]
        public VehicleData vehicleData;
        public VehicleConfig vehicleConfig;
        
        [Header("Vehicle Components")]
        public VehicleAnimation vehicleAnimation;
        public VehicleKinematics vehicleKinematics;
        public CollisionController collisionController;
        public VehicleDashboardObserver dashboardObserver;
        
        [Header("Mocap")]
        public OptitrackRigidBody tractorRigidBody;
        public OptitrackRigidBody trailerRigidBody;
        public GameObject tractorOptitrackComponent;
        public GameObject trailerOptitrackComponent;
        
        [Header("ROS")]
        public RosConnector rosConnector;
        public GameObject rosComponent;
        public TwistSubscriber twistSubscriber;
        public TwistPublisher twistPublisher;
        
        [Header("Dashboard Parent Transform")]
        public Transform vehicleDashboardParent;
        //private SetSimulationServiceProvider setSimulationServiceProvider;
        
        public int productID { get; set; }
        public string productName { get; set; }
        public GameObject productInstance { get; set; }
        public Camera mainCamera { get; set; }
        public SystemLog systemLog { get; set; }
        public GetClickedObject getClickedObject { get; set; }

        /// <summary>
        /// Initializes the vehicle product with the provided configuration and dependencies.
        /// </summary>
        public void Initialize()
        {
            vehicleData = new VehicleData(vehicleConfig.vehicleID, vehicleConfig.vehicleName, vehicleConfig, productInstance);
            
            vehicleAnimation = productInstance.AddComponent<VehicleAnimation>();
            vehicleAnimation.Initialize(this, vehicleData);
            
            vehicleKinematics = productInstance.AddComponent<VehicleKinematics>();
            vehicleKinematics.Initialize(this, vehicleData);
            
            collisionController = productInstance.AddComponent<CollisionController>();
            collisionController.Initialize(this, vehicleData);
            
            dashboardObserver = productInstance.AddComponent<VehicleDashboardObserver>();
            dashboardObserver.Initialize(this, vehicleData);
            dashboardObserver.InitVehicleDashboard(vehicleDashboardParent);
            
            vehicleAnimation.isActive = true;
            vehicleKinematics.isActive = true;
            collisionController.isActive = true;
            
            dashboardObserver.isToggleActive = true;
            
            if (vehicleConfig.isMocapAvaialbe)
            {
                tractorRigidBody = InitOptitrackTractor();
                trailerRigidBody = InitOptitrackTrailer();
            }
            
            rosConnector = InitRosConnector();
        }

        /// <summary>
        /// Initializes the OptiTrack component for the tractor.
        /// </summary>
        /// <returns>The initialized OptiTrack rigid body for the tractor.</returns>
        private OptitrackRigidBody InitOptitrackTractor()
        {
            tractorOptitrackComponent = new GameObject("TractorMoCap");
            tractorOptitrackComponent.transform.SetParent(vehicleAnimation.vehicleTransform);

            tractorRigidBody = tractorOptitrackComponent.AddComponent<OptitrackRigidBody>();

            OptitrackStreamingClient tractorOptitrackStreamingClient =
                tractorOptitrackComponent.AddComponent<OptitrackStreamingClient>();

            tractorRigidBody.StreamingClient = tractorOptitrackStreamingClient;

            tractorOptitrackStreamingClient.ServerAddress = vehicleConfig.optitrackServerAddress;
            tractorOptitrackStreamingClient.LocalAddress = vehicleConfig.optitrackLocalAddress;
            tractorOptitrackStreamingClient.ConnectionType = OptitrackStreamingClient.ClientConnectionType.Unicast;

            tractorRigidBody.RigidBodyId = vehicleConfig.tractorOptitrackID;
            tractorRigidBody.NetworkCompensation = false;

            tractorOptitrackStreamingClient.enabled = true;

            return tractorRigidBody;
        }

        /// <summary>
        /// Initializes the OptiTrack component for the trailer.
        /// </summary>
        /// <returns>The initialized OptiTrack rigid body for the trailer.</returns>
        private OptitrackRigidBody InitOptitrackTrailer()
        {
            trailerOptitrackComponent = new GameObject("TrailerMoCap");
            trailerOptitrackComponent.transform.SetParent(vehicleAnimation.vehicleTransform);
            trailerRigidBody = trailerOptitrackComponent.AddComponent<OptitrackRigidBody>();

            OptitrackStreamingClient trailerOptitrackStreamingClient =
                trailerOptitrackComponent.AddComponent<OptitrackStreamingClient>();
            trailerRigidBody.StreamingClient = trailerOptitrackStreamingClient;
            trailerRigidBody.RigidBodyId = vehicleConfig.trailorOptitrackID;
            trailerRigidBody.NetworkCompensation = true;

            trailerOptitrackStreamingClient.ServerAddress = vehicleConfig.optitrackServerAddress;
            trailerOptitrackStreamingClient.LocalAddress = vehicleConfig.optitrackLocalAddress;
            trailerOptitrackStreamingClient.ConnectionType = OptitrackStreamingClient.ClientConnectionType.Unicast;

            trailerOptitrackStreamingClient.enabled = true;

            return trailerRigidBody;
        }

        /// <summary>
        /// Initializes the ROS connector component.
        /// </summary>
        /// <returns>The initialized ROS connector.</returns>
        private RosConnector InitRosConnector()
        {
            rosComponent = new GameObject("ROSComponent");
            rosComponent.transform.SetParent(vehicleAnimation.vehicleTransform);

            rosConnector = rosComponent.AddComponent<RosConnector>();
            rosConnector.RosBridgeServerUrl = vehicleConfig.rosBridgeServerAddress;
            rosConnector.SecondsTimeout = 10;
            rosConnector.Serializer = RosSocket.SerializerEnum.Microsoft;
            rosConnector.protocol = Protocol.WebSocketSharp;

            InitRosComponents();

            return rosConnector;
        }

        /// <summary>
        /// Initializes the ROS components (subscribers and publishers).
        /// </summary>
        private void InitRosComponents()
        {
            twistSubscriber = rosComponent.AddComponent<TwistSubscriber>();
            twistSubscriber.Topic = vehicleConfig.twistSubscriberTopicController;

            twistPublisher = rosComponent.AddComponent<TwistPublisher>();
            twistPublisher.Topic = vehicleConfig.twistPublisherTopicController;
            
            // TODO: Add additional ROS components if needed
        }

        /// <summary>
        /// Sets the simulation detail for the vehicle.
        /// </summary>
        /// <param name="vehicleID">Vehicle ID.</param>
        /// <param name="pathID">Path ID.</param>
        public void SetSimulationDetail(int vehicleID, int pathID)
        {
            //setSimulationServiceProvider.SetSimulationDetail(vehicleID, pathID);
        }

        /// <summary>
        /// Cleanup and destroy components when the vehicle product is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            if (vehicleConfig.isMocapAvaialbe)
            {
                Destroy(tractorOptitrackComponent);
                Destroy(trailerOptitrackComponent);
            }
            Destroy(rosComponent);
            Destroy(vehicleAnimation);
            Destroy(vehicleKinematics);
            Destroy(dashboardObserver);
        }
    }
}
