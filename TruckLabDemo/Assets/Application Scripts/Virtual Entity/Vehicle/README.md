# Vehicle

This folder contains scripts related to the creation, configuration, and management of vehicle entities within the simulation. Vehicles are complex virtual entities that simulate real-world vehicle behavior, including motion dynamics, collision detection, and user interaction. The scripts in this folder handle initialization, control strategies, data management, and integration with external systems like ROS (Robot Operating System) and motion capture systems.

## Contents

- [`VE_Vehicle.cs`](#ve_vehiclecs)
- [`Vehicle_Config.cs`](#vehicle_configcs)
- [`Vehicle_Creator.cs`](#vehicle_creatorcs)
- [`Vehicle_Data.cs`](#vehicle_datacs)

---

### `VE_Vehicle.cs`

#### Overview

Represents a vehicle entity within the simulation. Inherits from `VE` and encapsulates all components, controllers, and data associated with a vehicle. Handles initialization, integration with motion capture and ROS, and manages dependencies between various vehicle systems.

#### Key Components

- **Fields:**

  - **Vehicle Data:**
    - `Vehicle_Config Config`: Configuration data for the vehicle.
    - `Vehicle_Data Data`: Runtime data instance managing state and physics.

  - **Vehicle Components:**
    - `List<Camera> VehicleCameras`: Cameras attached to the vehicle.
    - `GameObject Tractor`: The tractor component.
    - `GameObject Trailer`: The trailer component.

  - **Controllers:**
    - `Vehicle_Dashboard_Controller VehicleDashboardController`
    - `Vehicle_Animation_Controller AnimationController`
    - `Vehicle_Kinematics_Controller KinematicsController`
    - `Vehicle_Collision_Controller CollisionController`

  - **Middleware Components:**
    - **Motion Capture:**
      - `GameObject TractorMoCapComponentInstance`
      - `GameObject TrailerMoCapComponentInstance`
      - `OptitrackRigidBody TractorRigidBody`
      - `OptitrackRigidBody TrailerRigidBody`
    - **ROS:**
      - `GameObject RosComponentInstance`
      - `RosConnector RosConnector`
      - `TwistPublisher TwistPublisher`
      - `TwistSubscriber SharedTwistSubscriber`

  - **Application Dependencies:**
    - `Transform VeInstanceParentTransform`
    - `Transform VeUiInstanceParentTransform`
    - `Camera_Manager CameraManager`
    - `Space_Creator SpaceCreator`
    - `Vehicle_Creator VehicleCreator`
    - `UI_Controller_SystemLog SystemLogUiController`
    - `VE_OnClick_Getter VeOnClickGetterGetter`

- **Methods:**

  - `void SetDependencies(...)`: Sets the dependencies required for the vehicle's operation.
  - `override void Init()`: Initializes the vehicle and its components.
  - `void InitializeCommonProperties()`: Initializes common properties like ID, name, and basic structure.
  - `void InitializeControllers()`: Initializes controllers such as kinematics, animation, and collision.
  - `void InitializeUiController()`: Initializes the vehicle's UI dashboard.
  - `void CollectVehicleCameras()`: Collects all cameras associated with the vehicle.
  - `void InitRos()`: Initializes ROS components for vehicle communication.
  - `void InitMoCap()`: Initializes motion capture components for the tractor and trailer.
  - `void OnDestroy()`: Cleans up resources when the vehicle is destroyed.

#### Implementation Details

- **Initialization:**

  - Sets up the vehicle's name, tag, and label.
  - Initializes data and controllers necessary for vehicle operation.
  - Configures motion capture and ROS components if enabled in the configuration.

- **Controllers:**

  - **Kinematics Controller:** Manages vehicle motion and dynamics.
  - **Animation Controller:** Handles visual representation and animations.
  - **Collision Controller:** Manages collision detection and response.
  - **Dashboard Controller:** Provides a UI interface for interacting with the vehicle.

- **Middleware Integration:**

  - **Motion Capture:** Integrates with OptiTrack systems for real-time position and orientation data.
  - **ROS:** Enables communication with ROS topics for receiving and sending control messages.

#### Usage

- Instantiated by the `Vehicle_Creator` when creating a new vehicle.
- Interacts with other systems through its controllers and middleware components.
- Provides methods for initialization and cleanup to ensure proper resource management.

---

### `Vehicle_Config.cs`

#### Overview

Defines the configuration data for a vehicle entity as a scriptable object. Inherits from `VE_Config` and contains all the necessary parameters to instantiate and control a vehicle within the simulation. This includes references to prefabs, control strategies, dimensions, and integration settings for ROS and motion capture systems.

#### Key Components

- **Fields:**

  - **Prototype:**
    - `GameObject VehiclePrototypePrefab`: Prefab for the vehicle model.

  - **Vehicle Control:**
    - `Kinematics_Strategy KinematicStrategy`: The kinematics strategy used for vehicle motion.
    - `List<Actuation_Input_Strategy> InputStrategies`: List of available input strategies for actuation.
    - `Dictionary<string, Actuation_Input_Strategy> VehicleInputStrategiesDict`: Dictionary mapping strategy names to their implementations.

  - **Vehicle Dimensions:**
    - `float Scale`: Scaling factor for the vehicle model.
    - `float L1Scaled`: Scaled length from the front axle to the rear axle of the tractor.
    - `float L1CScaled`: Scaled length from the rear axle to the coupling point on the tractor.
    - `float R2Scaled`: Scaled length from the coupling point to the rear axle of the trailer.
    - `float TractorWidthScaled`: Scaled width of the tractor.
    - `float TrailerWidthScaled`: Scaled width of the trailer.

  - **Motion Capture Configuration:**
    - `bool IsMoCapAvailable`: Indicates if motion capture is enabled.
    - `string OptitrackServerAddress`: Server address for OptiTrack.
    - `string OptitrackLocalAddress`: Local address for OptiTrack.
    - `Int32 TractorOptitrackId`: OptiTrack ID for the tractor.
    - `Int32 TrailerOptitrackId`: OptiTrack ID for the trailer.

  - **ROS Configuration:**
    - `bool IsRosAvailable`: Indicates if ROS integration is enabled.
    - `string RosBridgeServerAddress`: ROS bridge server URL.
    - `string TwistSubscriberTopic`: ROS topic for subscribing to twist messages.
    - `string TwistPublisherTopic`: ROS topic for publishing twist messages.

  - **Initial Vehicle States:**
    - `float InitialTractorPosX`: Initial X position of the tractor.
    - `float InitialTractorPosY`: Initial Y position of the tractor.
    - `float InitialTractorAngle`: Initial orientation angle of the tractor.
    - `float InitialTrailerAngle`: Initial orientation angle of the trailer.
    - `float InitialVelocity`: Initial velocity of the vehicle.
    - `float InitialAcceleration`: Initial acceleration of the vehicle.
    - `float InitialSteeringAngle`: Initial steering angle of the vehicle.

- **Methods:**

  - `void OnEnable()`: Initializes the `VehicleInputStrategiesDict` from the list of input strategies when the scriptable object is enabled.

#### Usage

- Create instances of `Vehicle_Config` via the Unity editor to define different vehicle configurations.
- Assign appropriate prefabs, strategies, dimensions, and integration settings based on simulation requirements.
- Used by `Vehicle_Creator` to instantiate and initialize vehicles in the simulation.

---

### `Vehicle_Creator.cs`

#### Overview

A factory class responsible for creating and managing vehicle entities within the simulation. Inherits from `VE_Creator<VE_Vehicle, Vehicle_Config>`, allowing it to handle vehicles specifically. Provides methods to instantiate vehicles based on configurations, register them, and handle their deletion.

#### Key Components

- **Fields:**

  - **Event Channels:**
    - `EventChannel_Void VeVehicleRegistrationEventChannel`: Event channel for notifying vehicle registration events.
    - `EventChannel_Void VeVehicleDeletionEventChannel`: Event channel for notifying vehicle deletion events.

  - **Application Dependencies:**
    - `Vehicle_Creator VehicleCreator`: Reference to the vehicle creator instance.
    - `Space_Creator SpaceCreator`: Reference to the space creator instance.
    - `VE_OnClick_Getter VeOnClickGetter`: Utility for detecting objects via mouse clicks.

- **Methods:**

  - `VE_Vehicle Create_VE(Vehicle_Config vehicle_config)`: Creates a new vehicle entity based on the provided configuration.
  - `void Delete_VE(int ve_id)`: Deletes an existing vehicle entity based on its ID.

#### Implementation Details

- **Vehicle Creation:**

  - Instantiates the vehicle prefab defined in the `Vehicle_Config`.
  - Adds a `VE_Vehicle` component to the instantiated object.
  - Sets dependencies and initializes the vehicle.
  - Registers the vehicle in the lookup table and raises registration events.

- **Vehicle Deletion:**

  - Removes the vehicle from the lookup table based on its ID.
  - Destroys the vehicle's GameObject and associated UI instances.
  - Raises deletion events.

#### Usage

- Used by higher-level systems or UI components to create and manage vehicles within the simulation.
- Listens to events and provides methods to add or remove vehicles dynamically.

---

### `Vehicle_Data.cs`

#### Overview

Holds the state and configuration data for a vehicle entity. Inherits from `MonoBehaviour` and is attached to the vehicle GameObject. Manages kinematic properties, dimensions, and provides methods for calculating bounding boxes, which are useful for collision detection and spatial reasoning.

#### Key Components

- **Fields:**

  - **Identification:**
    - `int Id`: Unique identifier for the vehicle.
    - `string Name`: Name of the vehicle.

  - **Configuration:**
    - `Vehicle_Config Config`: Configuration data associated with the vehicle.
    - `GameObject VehicleInstance`: GameObject instance in the scene.

  - **Kinematic Points and Dimensions:**
    - Positions: `float X0`, `Y0`, `X1`, `Y1`, `X1C`, `Y1C`, `X2`, `Y2`
    - Angles: `float Psi1`, `Psi2`, `Gamma`
    - Dimensions: `float L1`, `L1C`, `L2`, `TractorWidth`, `TrailerWidth`

  - **Kinematic State Variables:**
    - Velocities: `float V1`, `V2`
    - Acceleration: `float a`
    - Steering Angle: `float Delta`

  - **Previous State Variables:**
    - Positions and Orientations: `float X1Prev`, `Y1Prev`, `Psi1Prev`, `Psi2Prev`
    - Derivatives: `float X1dot`, `Y1dot`, `Psi1dot`, `X2dot`, `Y2dot`, `Psi2dot`

- **Methods:**

  - `void Init(int vehicleId, string vehicleName, Vehicle_Config vehicleConfig, GameObject vehicleInstance)`: Initializes the vehicle data based on configuration.
  - `Vector3[] GetTractorBoundingBox()`: Calculates the bounding box of the tractor.
  - `Vector3[] GetTrailerBoundingBox()`: Calculates the bounding box of the trailer.

#### Implementation Details

- **Kinematic Properties:**

  - Manages real-time data for the vehicle's position, orientation, and motion.
  - Provides a central place for other components to access and update the vehicle's state.

- **Dimension Scaling:**

  - Vehicle dimensions are scaled using the `Scale` factor from the configuration to ensure consistency within the simulation environment.

- **Bounding Box Calculation:**

  - Calculates the corners of the tractor and trailer based on their current positions and orientations.
  - Useful for collision detection and spatial queries.

#### Usage

- Attached to the vehicle GameObject and initialized during vehicle creation.
- Accessed by controllers and other components needing vehicle state information.
- Provides utility methods for geometry calculations related to the vehicle.

