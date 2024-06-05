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
        public VehicleDashboard dashboard;
        
        [Header("Mocap")]
        public OptitrackRigidBody tractorRigidBody;
        public OptitrackRigidBody trailerRigidBody;
        public GameObject tractorOptitrackComponent;
        public GameObject trailerOptitackComponent;
        
        [Header("ROS")]
        public RosConnector rosConnector;
        public GameObject rosComponent;
        public TwistSubscriber twistSubscriberManual;
        public TwistSubscriber twistSubscriberController;

        public TwistPublisher twistPublisherManual;
        
        // TODO: add more topics or services connection
        public SetSimulationServiceProvider setSimulationServiceProvider;
        
        [Header("Dashboard Parent Transform")]
        public Transform vehicleDashboardParent;

        
        public void Init(VehicleConfig config, GameObject instance, Camera cam, 
                         Transform dashboardParent)
        {
            base.Init(config.vehicleID, config.vehicleName, instance, cam);
            vehicleConfig = config;
            vehicleDashboardParent = dashboardParent;
            setSimulationServiceProvider = FindObjectOfType<SetSimulationServiceProvider>();
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
            
            dashboard = productInstance.AddComponent<VehicleDashboard>();
            dashboard.Initialize(this, vehicleData);
            dashboard.InitVehicleDashboard(vehicleDashboardParent);
            
            
            vehicleAnimation.isActive = true;
            vehicleKinematics.isActive = true;
            collisionController.isActive = true;
            
            dashboard.isToggleActive = true;
            
            if (vehicleConfig.isMocapAvaialbe)
            {
                tractorRigidBody = InitOptitrackTractor();
                trailerRigidBody = InitOptitrackTrailer();
            }
            
            rosConnector = InitRosConnector();
        }
        public OptitrackRigidBody InitOptitrackTractor()
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
        public OptitrackRigidBody InitOptitrackTrailer()
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
        public RosConnector InitRosConnector()
        {
            rosComponent = new GameObject("ROSComponent");
            rosComponent.transform.SetParent(vehicleAnimation.vehicleTransform);

            rosConnector = rosComponent.AddComponent<RosConnector>();
            rosConnector.RosBridgeServerUrl = vehicleConfig.rosBridgeServerAddress;
            rosConnector.SecondsTimeout = 10;
            rosConnector.Serializer = RosSocket.SerializerEnum.Microsoft;
            rosConnector.protocol = Protocol.WebSocketSharp;

            
            twistSubscriberManual = rosComponent.AddComponent<TwistSubscriber>();
            //twistSubscriberController = rosComponent.AddComponent<TwistSubscriber>();

            twistSubscriberManual.Topic = vehicleConfig.twistSubscriberTopicManual;
            //twistSubscriberController.Topic = vehicleConfig.twistSubscriberTopicController;
            
            twistPublisherManual = rosComponent.AddComponent<TwistPublisher>();
            twistPublisherManual.Topic = vehicleConfig.twistPublisherTopicManual;

            return rosConnector;
        }
        public void SetSimulationDetail(int vehicleID, int pathID)
        {
            setSimulationServiceProvider.SetSimulationDetail(vehicleID, pathID);
        }
        void OnDestroy()
        {
            if (vehicleConfig.isMocapAvaialbe)
            {
                Destroy(tractorOptitrackComponent);
                Destroy(trailerOptitackComponent);
            }
            Destroy(rosComponent);
            Destroy(vehicleAnimation);
            Destroy(vehicleKinematics);
            Destroy(dashboard);
        }
    }
}

