# Vehicle Kinematics Strategy

This folder contains scripts related to the kinematics strategies used by vehicles. The kinematics strategies define how a vehicle's position and orientation are updated over time based on different models or data sources, such as mathematical models or motion capture data.

## Contents

- [`Kinematics\_Strategy.cs`](#kinematics\_strategycs)
- [`Forward\_Kinematics\_Strategy.cs`](#forward\_kinematics\_strategycs)
- [`Motion\_Capture\_Kinematics\_Strategy.cs`](#motion\_capture\_kinematics\_strategycs)

---

### `Kinematics\_Strategy.cs`

#### Overview

Defines an abstract base class for kinematics strategies. This class provides a framework for updating vehicle kinematics based on specific models or data sources. All kinematics strategies inherit from this class and implement the `UpdateKinematics` method.

#### Key Components

- **Fields:**
  - `string KinematicsStrategyName`: Name of the kinematics strategy, used for identification and debugging.

- **Methods:**
  - `abstract void UpdateKinematics(VE\_Vehicle vehicle, float deltaTime, float inputVelocity, float inputSteerAngle)`: Abstract method that updates the kinematic properties of the specified vehicle.

#### Usage

- This class is intended to be subclassed by concrete kinematics strategies.
- Implementations should provide specific logic in the `UpdateKinematics` method to update the vehicle's state based on the chosen kinematic model.

---

### `Forward\_Kinematics\_Strategy.cs`

#### Overview

A kinematics strategy that uses a mathematical actuation model to calculate the vehicle's state. It is suitable for scenarios involving simulated control inputs, where the vehicle's movement is determined by control commands such as velocity and steering angle.

#### Key Components

- **Inheritance:** Inherits from `Kinematics\_Strategy`.

- **Methods:**
  - `override void UpdateKinematics(VE\_Vehicle vehicle, float deltaTime, float inputVelocity, float inputSteerAngle)`: Updates the vehicle's state based on the provided control inputs and elapsed time.

#### Implementation Details

- Calculates the position of the front axle (`X0`, `Y0`), coupling point (`X1C`, `Y1C`), and trailer (`X2`, `Y2`) based on the vehicle's dimensions and input controls.
- Updates the vehicle's position (`X1`, `Y1`) and orientation (`Psi1`) over time.
- Relies on methods from `Vehicle\_Kinematics\_Controller` to calculate intermediate states and kinematic properties.

#### Usage

- This strategy is used when simulating vehicle motion based on control inputs in the absence of real-world data.
- Available for testing and simulation environments where motion capture data is not available.

---

### `Motion\_Capture\_Kinematics\_Strategy.cs`

#### Overview

A kinematics strategy that updates the vehicle's state using motion capture data. It is designed for scenarios where real-world position and orientation data are available, such as from a motion capture system like Motive.

#### Key Components

- **Inheritance:** Inherits from `Kinematics\_Strategy`.

- **Methods:**
  - `override void UpdateKinematics(VE\_Vehicle vehicle, float deltaTime, float inputVelocity, float inputSteerAngle)`: Updates the vehicle's state based on motion capture data.

#### Implementation Details

- Updates the tractor and trailer positions (`X1`, `Y1`, `X2`, `Y2`) and orientations (`Psi1`, `Psi2`) using data from `OptitrackRigidBody` components (`TractorRigidBody` and `TrailerRigidBody`).
- Calculates the positions of the front axle (`X0`, `Y0`) and coupling point (`X1C`, `Y1C`) based on the updated positions and orientations.
- Does not use `deltaTime`, `inputVelocity`, or `inputSteerAngle` as the motion is driven by external data from the motion capture system.

#### Usage

- This strategy is applied when integrating real-world vehicle movements into the virtual entity.
- Requires motion capture components to provide position and orientation data.
- Suitable for applications where the vehicle's movement is tracked in real time.

---

## Notes

- The kinematics strategies are part of the vehicle's kinematics controller system, allowing for flexible switching between different models based on simulation requirements.
- Developers can create additional kinematics strategies by subclassing `Kinematics\_Strategy` and implementing the `UpdateKinematics` method to accommodate other models or data sources.
- The strategies are designed to be interchangeable, facilitating testing and comparison between different kinematic models.

