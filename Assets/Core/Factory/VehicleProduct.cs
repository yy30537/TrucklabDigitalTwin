using UnityEngine;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Protocols;

namespace Core
{
    public class VehicleProduct : Product
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
        public GameObject trailerOptitackComponent;
        
        [Header("ROS")]
        public RosConnector rosConnector;
        public GameObject rosComponent;
        public TwistSubscriber twistSubscriber;
        public TwistPublisher twistPublisher;
        
        [Header("Dashboard Parent Transform")]
        public Transform vehicleDashboardParent;
        
        private SetSimulationServiceProvider setSimulationServiceProvider;

        public void Init(VehicleConfig config, GameObject instance, Camera cam, Transform dashboardParent, SystemLog systemLog, GetClickedObject getClickedObject, SetSimulationServiceProvider simulationServiceProvider)
        {
            base.Init(config.vehicleID, config.vehicleName, instance, cam, systemLog, getClickedObject);
            vehicleConfig = config;
            vehicleDashboardParent = dashboardParent;
            this.setSimulationServiceProvider = simulationServiceProvider;
            InitComponents();
        }

        public override void InitComponents()
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

        private OptitrackRigidBody InitOptitrackTrailer()
        {
            trailerOptitackComponent = new GameObject("TrailerMoCap");
            trailerOptitackComponent.transform.SetParent(vehicleAnimation.vehicleTransform);
            trailerRigidBody = trailerOptitackComponent.AddComponent<OptitrackRigidBody>();

            OptitrackStreamingClient trailerOptitrackStreamingClient =
                trailerOptitackComponent.AddComponent<OptitrackStreamingClient>();
            trailerRigidBody.StreamingClient = trailerOptitrackStreamingClient;
            trailerRigidBody.StreamingClient = trailerOptitrackStreamingClient;
            trailerRigidBody.RigidBodyId = vehicleConfig.trailorOptitrackID;
            trailerRigidBody.NetworkCompensation = true;

            trailerOptitrackStreamingClient.ServerAddress = vehicleConfig.optitrackServerAddress;
            trailerOptitrackStreamingClient.LocalAddress = vehicleConfig.optitrackLocalAddress;
            trailerOptitrackStreamingClient.ConnectionType = OptitrackStreamingClient.ClientConnectionType.Unicast;

            trailerOptitrackStreamingClient.enabled = true;

            return trailerRigidBody;
        }

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

        private void InitRosComponents()
        {
            twistSubscriber = rosComponent.AddComponent<TwistSubscriber>();
            twistSubscriber.Topic = vehicleConfig.twistSubscriberTopicController;

            twistPublisher = rosComponent.AddComponent<TwistPublisher>();
            twistPublisher.Topic = vehicleConfig.twistPublisherTopicController;
            
            // TODO: ...
        }

        public void SetSimulationDetail(int vehicleID, int pathID)
        {
            setSimulationServiceProvider.SetSimulationDetail(vehicleID, pathID);
        }

        private void OnDestroy()
        {
            if (vehicleConfig.isMocapAvaialbe)
            {
                Destroy(tractorOptitrackComponent);
                Destroy(trailerOptitackComponent);
            }
            Destroy(rosComponent);
            Destroy(vehicleAnimation);
            Destroy(vehicleKinematics);
            Destroy(dashboardObserver);
        }
    }
}
