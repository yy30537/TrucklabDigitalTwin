# Vehicle Actuation Input Strategy

This folder contains scripts related to the actuation input strategies used by vehicles within the simulation. Actuation input strategies define how control inputs (such as velocity and steering angle) are obtained for vehicle movement. This modular approach allows for flexibility in controlling vehicles using different input sources, such as keyboard input, game controllers, or external systems via ROS (Robot Operating System).

## Contents

- [`Actuation_Input_Strategy.cs`](#actuation_input_strategycs)
- [`Keyboard_Input_Strategy.cs`](#keyboard_input_strategycs)
- [`Controller_Input_Strategy.cs`](#controller_input_strategycs)

---

### `Actuation_Input_Strategy.cs`

#### Overview

Defines an abstract base class for actuation input strategies. This class establishes a framework for retrieving control inputs necessary for vehicle actuation. All specific input strategies inherit from this class and implement the `GetInput` method to provide velocity and steering angle data.

#### Key Components

- **Fields:**
  - `string StrategyName`: Name of the strategy, used for identification and debugging purposes.

- **Methods:**
  - `abstract void Initialize(TwistSubscriber twistSubscriber = null)`: Abstract method for initializing the input strategy. Allows for optional configuration with a `TwistSubscriber` for ROS integration.
  - `abstract (float velocity, float steerAngle) GetInput()`: Abstract method to retrieve the current input values for velocity and steering angle.

#### Usage

- This class is intended to be subclassed by concrete actuation input strategies.
- Implementations should provide specific logic in the `GetInput` method to obtain control inputs from various sources.

---

### `Keyboard_Input_Strategy.cs`

#### Overview

An input strategy that captures control inputs from the keyboard. It enables manual control of the vehicle using predefined keys for movement and steering.

#### Key Components

- **Inheritance:** Inherits from `Actuation_Input_Strategy`.

- **Fields:**
  - `float KeyboardInputVelocity`: Fixed velocity value for forward and backward movement using keyboard input.
  - `float SteerRate`: Steering rate (in radians per second) for left and right turns.

- **Methods:**
  - `override void Initialize(TwistSubscriber twistSubscriber = null)`: Initialization method; no additional setup is required for keyboard input.
  - `override (float velocity, float steerAngle) GetInput()`: Retrieves input values based on the current state of keyboard keys.

#### Implementation Details

- **Movement Keys:**
  - `W`: Move forward.
  - `S`: Move backward.

- **Steering Keys:**
  - `A`: Steer left.
  - `D`: Steer right.

- The strategy returns fixed velocity and steering values when the corresponding keys are pressed.

#### Usage

- Suitable for testing and development environments where manual control of the vehicle is needed.
- Does not require any external dependencies or integration with ROS.

---

### `Controller_Input_Strategy.cs`

#### Overview

An input strategy that retrieves control inputs from a ROS topic. Designed to integrate with game controllers or external control systems that publish control commands to a ROS network.

#### Key Components

- **Inheritance:** Inherits from `Actuation_Input_Strategy`.

- **Fields:**
  - `string TopicName`: The name of the ROS topic to subscribe to for receiving control inputs.
  - `TwistSubscriber twistSubscriber`: Reference to a `TwistSubscriber` for receiving ROS `Twist` messages.

- **Methods:**
  - `override void Initialize(TwistSubscriber sharedTwistSubscriber)`: Initializes the input strategy by assigning the ROS topic and subscriber.
  - `override (float velocity, float steerAngle) GetInput()`: Retrieves input values based on the data received from the ROS topic.

#### Implementation Details

- **ROS Integration:**
  - Subscribes to a specified ROS topic using a `TwistSubscriber`.
  - Receives `Twist` messages containing linear and angular velocities.

- **Data Mapping:**
  - `inputVelocity`: Mapped from the linear velocity along the Y-axis (`twistSubscriber.linearVelocity.y`).
  - `inputSteerAngle`: Mapped from the angular velocity around the X-axis (`twistSubscriber.angularVelocity.x`).

#### Usage

- Ideal for simulations that require integration with external control systems or hardware devices that publish control commands over ROS.
- Requires a running ROS environment and appropriate publishers to provide control inputs.

---

## Notes

- The actuation input strategies are designed to be interchangeable, allowing vehicles to switch between different control input sources seamlessly.
- Developers can extend the system by creating new input strategies that inherit from `Actuation_Input_Strategy` to accommodate additional control methods (e.g., joystick input, AI controllers).
- Strategies that require ROS integration should ensure that the ROS environment is properly configured and that the necessary topics are available.

