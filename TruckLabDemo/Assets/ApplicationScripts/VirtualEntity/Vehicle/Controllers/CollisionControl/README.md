# Vehicle Collision Control

This folder contains scripts related to the collision detection and handling mechanisms for vehicles. The collision control system is responsible for detecting obstacles around the vehicle, managing collision events, and triggering appropriate responses such as braking to prevent collisions.

## Contents

- [`Obstacle_Info.cs`](#obstacle_infocs)
- [`Vehicle_Children_Collision_Forwarder.cs`](#vehicle_children_collision_forwardercs)


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

## Notes

- The collision control system is designed to be modular and extensible. Additional sensors or detection mechanisms can be integrated by following the existing structure.
- The current implementation uses colliders and trigger events for obstacle detection. Alternative methods (e.g., raycasting) can be implemented if required.
- All detected obstacles are tracked with their distance and angle, which can be used for advanced avoidance strategies or integration with other systems (e.g., ROS messages).

