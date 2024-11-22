# Vehicle Collision Control

This folder contains scripts related to the collision detection and handling mechanisms for vehicles within the simulation. The collision control system is responsible for detecting obstacles around the vehicle, managing collision events, and triggering appropriate responses such as braking to prevent collisions.

## Contents

- [`Obstacle_Info.cs`](#obstacle_infocs)
- [`Vehicle_Children_Collision_Forwarder.cs`](#vehicle_children_collision_forwardercs)
- [`Vehicle_Collision_Controller.cs`](#vehicle_collision_controllercs)

---

### `Obstacle_Info.cs`

#### Overview

Defines a serializable class that represents an obstacle detected by the vehicle's sensors. This class encapsulates information about the obstacle necessary for collision handling and avoidance.

#### Key Components

- **Fields:**
  - `GameObject Object`: Reference to the detected obstacle's GameObject.
  - `float Distance`: The distance from the vehicle's sensor to the obstacle.
  - `float Angle`: The angle of the obstacle relative to the vehicle's forward direction.
  - `string Name`: A descriptive name for the obstacle.

#### Usage

Instances of `Obstacle_Info` are created and managed by the `Vehicle_Collision_Controller` to keep track of obstacles currently detected by the vehicle's sensors.

---

### `Vehicle_Children_Collision_Forwarder.cs`

#### Overview

A utility script that forwards collision events from child GameObjects (e.g., sensors, colliders attached to parts of the vehicle) to the parent `Vehicle_Collision_Controller`. This allows centralized handling of collision events without attaching collision logic to each child object.

#### Key Components

- **Fields:**
  - `Vehicle_Collision_Controller parentController`: Reference to the parent collision controller to which events are forwarded.

- **Methods:**
  - `SetParentController(Vehicle_Collision_Controller controller)`: Assigns the parent collision controller.
  - `OnTriggerEnter(Collider other)`: Forwards trigger enter events to the parent controller.
  - `OnTriggerExit(Collider other)`: Forwards trigger exit events to the parent controller.

#### Usage

Attach this script to any child GameObject of the vehicle that needs to report collision events to the `Vehicle_Collision_Controller`. Ensure that the parent controller is assigned using `SetParentController`.

---

### `Vehicle_Collision_Controller.cs`

#### Overview

Manages collision detection and avoidance for the vehicle using sensors (e.g., colliders). It tracks detected obstacles, updates their information, and triggers actions such as braking when obstacles are within certain thresholds.

#### Key Components

- **Fields:**
  - `VE_Vehicle VeVehicle`: Reference to the associated vehicle object.
  - `Vehicle_Data VehicleData`: Reference to the vehicle's data.
  - `Collider TractorBoxCollider`: Collider for the tractor section.
  - `Collider TrailerBoxCollider`: Collider for the trailer section.
  - `SphereCollider SensorSphereCollider`: Sphere collider used as an obstacle detection sensor.
  - `Vector3 VehicleObstacleSensorCenter`: Current center position of the vehicle's obstacle detection sensor.
  - `bool IsActive`: Indicates whether collision detection is active.
  - `bool IsObstacleDetected`: Indicates if an obstacle is currently detected.
  - `float Range`: Maximum detection range of the obstacle sensor.
  - `int NumRays`: Number of rays used in obstacle detection (if applicable).
  - `List<Obstacle_Info> DetectedObstacles`: List of currently detected obstacles.
  - `float BrakingThreshold`: Distance threshold to trigger braking.

- **Methods:**
  - `Init(VE_Vehicle vehicle, Vehicle_Data data)`: Initializes the collision controller with the associated vehicle and data.
  - `FixedUpdate()`: Updates the sensor position and obstacle data at fixed intervals.
  - `OnChildTriggerEnter(GameObject child, Collider other)`: Handles trigger enter events from child objects, detecting new obstacles.
  - `OnChildTriggerExit(GameObject child, Collider other)`: Handles trigger exit events from child objects, removing obstacles from the detection list.
  - `UpdateDetectedObstacles()`: Updates information for detected obstacles, including distance and angle.
  - `IsSelfCollision(Collider collider)`: Determines if a collision is with the vehicle itself.
  - `AttachCollisionForwarder(GameObject child)`: Attaches a collision forwarder to a specified child object.

#### Functionality

- **Obstacle Detection:**
  - Uses a `SphereCollider` as a sensor to detect obstacles within a certain range around the vehicle.
  - When an obstacle enters the sensor's trigger area, an `Obstacle_Info` instance is created and added to the `DetectedObstacles` list.
  - The sensor ignores collisions with the vehicle itself and specific tags (e.g., ground plane).

- **Obstacle Tracking:**
  - Continuously updates the position of the sensor based on the vehicle's current position.
  - Updates the distance and angle of each detected obstacle relative to the vehicle.
  - Removes obstacles from the list when they exit the sensor's trigger area or are destroyed.

- **Collision Response:**
  - Checks if any detected obstacle is within the `BrakingThreshold`.
  - If an obstacle is within the threshold, commands the vehicle's kinematics controller to apply brakes.
  - Releases brakes when no obstacles are within the threshold.

#### Usage

- Initialize the collision controller by calling `Init` with the appropriate vehicle and data references.
- Ensure that the vehicle's tractor and trailer have colliders and that `Vehicle_Children_Collision_Forwarder` is attached to them.
- Set the `IsActive` flag to enable or disable collision detection as needed.

---

## Notes

- The collision control system is designed to be modular and extensible. Additional sensors or detection mechanisms can be integrated by following the existing structure.
- The current implementation uses colliders and trigger events for obstacle detection. Alternative methods (e.g., raycasting) can be implemented if required.
- All detected obstacles are tracked with their distance and angle, which can be used for advanced avoidance strategies or integration with other systems (e.g., ROS messages).

