# Vehicle

This folder contains scripts related to the creation, configuration, and management of vehicle entities. hVeicles are complex virtual entities that simulate real-world vehicle behavior, including motion dynamics, collision detection, and user interaction. The scripts in this folder handle initialization, control strategies, data management, and integration with external systems like ROS (Robot Operating System) and motion capture systems.

## Contents

- [`VE\_Vehicle.cs`](#ve\_vehiclecs)
- [`Vehicle\_Config.cs`](#vehicle\_configcs)
- [`Vehicle\_Creator.cs`](#vehicle\_creatorcs)
- [`Vehicle\_Data.cs`](#vehicle\_datacs)

---

### `VE\_Vehicle.cs`

#### Overview

Represents a vehicle entity within the virtual environment. Inherits from `VE` and encapsulates all components, controllers, and data associated with a vehicle. Handles initialization, integration with motion capture and ROS, and manages dependencies between various vehicle systems.

#### Key Components

- **Fields:**

  - **Vehicle Data:**
    - `Vehicle\_Config Config`: Configuration data for the vehicle.
    - `Vehicle\_Data Data`: Runtime data instance managing state and physics.

  - **Vehicle Components:**
    - `List<Camera> VehicleCameras`: Cameras attached to the vehicle.
    - `GameObject Tractor`: The tractor component.
    - `GameObject Trailer`: The trailer component.

  - **Controllers:**
    - `Vehicle\_Dashboard\_Controller VehicleDashboardController`
    - `Vehicle\_Animation\_Controller AnimationController`
    - `Vehicle\_Kinematics\_Controller KinematicsController`
    - `Vehicle\_Collision\_Controller CollisionController`

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
    - `Camera\_Manager CameraManager`
    - `Space\_Creator SpaceCreator`
    - `Vehicle\_Creator VehicleCreator`
    - `UI\_Controller\_SystemLog SystemLogUiController`
    - `VE\_OnClick\_Getter VeOnClickGetterGetter`

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

- Instantiated by the `Vehicle\_Creator` when creating a new vehicle.
- Interacts with other systems through its controllers and middleware components.
- Provides methods for initialization and cleanup to ensure proper resource management.

---

### `Vehicle\_Config.cs`

#### Overview

Defines the configuration data for a vehicle entity as a scriptable object. Inherits from `VE\_Config` and contains all the necessary parameters to instantiate and control a vehicle. This includes references to prefabs, control strategies, dimensions, and integration settings for ROS and motion capture systems.

#### Key Components

- **Fields:**

  - **Prototype:**
    - `GameObject VehiclePrototypePrefab`: Prefab for the vehicle model.

  - **Vehicle Control:**
    - `Kinematics\_Strategy KinematicStrategy`: The kinematics strategy used for vehicle motion.
    - `List<Actuation\_Input\_Strategy> InputStrategies`: List of available input strategies for actuation.
    - `Dictionary<string, Actuation\_Input\_Strategy> VehicleInputStrategiesDict`: Dictionary mapping strategy names to their implementations.

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

- Create instances of `Vehicle\_Config` via the Unity editor to define different vehicle configurations.
- Assign appropriate prefabs, strategies, dimensions, and integration settings based on simulation requirements.
- Used by `Vehicle\_Creator` to instantiate and initialize vehicles.

---

### `Vehicle\_Creator.cs`

#### Overview

A factory class responsible for creating and managing vehicle entities. Inherits from `VE\_Creator<VE\_Vehicle, Vehicle\_Config>`, allowing it to handle vehicles specifically. Provides methods to instantiate vehicles based on configurations, register them, and handle their deletion.

#### Key Components

- **Fields:**

  - **Event Channels:**
    - `EventChannel\_Void VeVehicleRegistrationEventChannel`: Event channel for notifying vehicle registration events.
    - `EventChannel\_Void VeVehicleDeletionEventChannel`: Event channel for notifying vehicle deletion events.

  - **Application Dependencies:**
    - `Vehicle\_Creator VehicleCreator`: Reference to the vehicle creator instance.
    - `Space\_Creator SpaceCreator`: Reference to the space creator instance.
    - `VE\_OnClick\_Getter VeOnClickGetter`: Utility for detecting objects via mouse clicks.

- **Methods:**

  - `VE\_Vehicle Create\_VE(Vehicle\_Config vehicle\_config)`: Creates a new vehicle entity based on the provided configuration.
  - `void Delete\_VE(int ve\_id)`: Deletes an existing vehicle entity based on its ID.

#### Implementation Details

- **Vehicle Creation:**

  - Instantiates the vehicle prefab defined in the `Vehicle\_Config`.
  - Adds a `VE\_Vehicle` component to the instantiated object.
  - Sets dependencies and initializes the vehicle.
  - Registers the vehicle in the lookup table and raises registration events.

- **Vehicle Deletion:**

  - Removes the vehicle from the lookup table based on its ID.
  - Destroys the vehicle's GameObject and associated UI instances.
  - Raises deletion events.

#### Usage

- Used by higher-level systems or UI components to create and manage vehicles.
- Listens to events and provides methods to add or remove vehicles during runtime as required.

---

### `Vehicle\_Data.cs`

#### Overview

Holds the state and configuration data for a vehicle entity. Inherits from `MonoBehaviour` and is attached to the vehicle GameObject. Manages kinematic properties, dimensions, and provides methods for calculating bounding boxes, which are useful for collision detection and spatial reasoning.

#### Key Components

- **Fields:**

  - **Identification:**
    - `int Id`: Unique identifier for the vehicle.
    - `string Name`: Name of the vehicle.

  - **Configuration:**
    - `Vehicle\_Config Config`: Configuration data associated with the vehicle.
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

  - `void Init(int vehicleId, string vehicleName, Vehicle\_Config vehicleConfig, GameObject vehicleInstance)`: Initializes the vehicle data based on configuration.
  - `Vector3[] GetTractorBoundingBox()`: Calculates the bounding box of the tractor.
  - `Vector3[] GetTrailerBoundingBox()`: Calculates the bounding box of the trailer.

#### Implementation Details

- **Kinematic Properties:**

  - Manages real-time data for the vehicle's position, orientation, and motion.
  - Provides a central place for other components to access and update the vehicle's state.

- **Bounding Box Calculation:**

  - Calculates the corners of the tractor and trailer based on their current positions and orientations.
  - Useful for collision detection and spatial queries.

#### Usage

- Attached to the vehicle GameObject and initialized during vehicle creation.
- Accessed by controllers and other components needing vehicle state information.
- Provides utility methods for geometry calculations related to the vehicle.

